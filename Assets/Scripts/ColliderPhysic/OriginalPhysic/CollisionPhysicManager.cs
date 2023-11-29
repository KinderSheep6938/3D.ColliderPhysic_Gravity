/// -----------------------------------------------------------------
/// CollisionPhysicManager.cs
/// 
/// 作成日：2023/11/06
/// 作成者：Shizuku
/// -----------------------------------------------------------------
namespace PhysicLibrary.CollisionPhysic
{
    using System.Collections.Generic;
    using UnityEngine;
    using PhysicLibrary.Material;

    #region データ管理用
    public enum PhysicThis
    {
        A,      //PhysicA
        B,      //PhysicB
        None    //どちらでもない
    }
    #endregion

    #region 衝突データ
    [System.Serializable]
    public struct PhysicCollision
    {
        public readonly PhysicMaterials physicA;  //Aの材質データ
        public readonly PhysicMaterials physicB;  //Bの材質データ
        public readonly Vector3 point;            //衝突地点
        public readonly Vector3 velocity;         //補完速度

        /// <summary>
        /// <para>PhysicCollision</para>
        /// <para>衝突管理用データ</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="point">衝突地点</param>
        /// <param name="velocity">補完速度</param>
        public PhysicCollision(PhysicMaterials a, PhysicMaterials b, Vector3 point, Vector3 velocity)
        {
            this.physicA = a;
            this.physicB = b;
            this.point = point;
            this.velocity = velocity;
        }

        /// <summary>
        /// <para>Contains</para>
        /// <para>検査対象が含まれるかつ、対処済みではないか確認します</para>
        /// </summary>
        /// <param name="search">検査対象</param>
        /// <returns>検査結果</returns>
        public bool Contains(PhysicMaterials search)
        {
            //どちらかには存在する
            if(Which(search) != PhysicThis.None)
            {
                return true;
            }
            //どちらでもない
            return false;
        }

        /// <summary>
        /// <para>Which</para>
        /// <para>検査対象がどっちのPhysicであるか返します</para>
        /// <para>どちらでもない時は、Noneを返します</para>
        /// </summary>
        /// <param name="search">検査対象</param>
        /// <returns>検査結果</returns>
        public PhysicThis Which(PhysicMaterials search)
        {
            //PhysicAである
            if (physicA.transform == search.transform)
            {
                return PhysicThis.A;
            }
            //PhysicBである
            else if (physicB.transform == search.transform)
            {
                return PhysicThis.B;
            }
            //どちらでもない
            return PhysicThis.None;
        }

        /// <summary>
        /// <para>OtherPhysic</para>
        /// <para>検査対象と別のPhysicMaterialsを返します</para>
        /// <para>存在しない場合は、PhysicMaterialsのデフォルト値が返却されます</para>
        /// </summary>
        /// <param name="search">検査対象</param>
        /// <returns>もう片方のPhysicMaterials</returns>
        public PhysicMaterials OtherPhysic(PhysicMaterials search)
        {
            //存在しない
            if (!Contains(search))
            {
                return default;
            }

            //PhysicAである
            if(physicA.transform == search.transform)
            {
                return physicB;
            }
            //PhysicBである
            return physicA;
        }
    }
    #endregion

    #region 物理挙動使用データ
    public struct OtherPhysicData
    {
        public readonly PhysicMaterials collision;  //相手の材質データ
        public readonly Vector3 point;              //衝突地点
        public readonly Vector3 velocity;           //補完速度

        /// <summary>
        /// <para>UsePhysucData</para>
        /// <para>物理挙動に必要なデータ</para>
        /// </summary>
        /// <param name="collision">相手の材質</param>
        /// <param name="point">衝突地点</param>
        /// <param name="velocity">補完速度</param>
        public OtherPhysicData(PhysicMaterials collision,Vector3 point,Vector3 velocity)
        {
            this.collision = collision;
            this.point = point;
            this.velocity = velocity;
        }
    }
    #endregion

    /// <summary>
    /// <para>CollisionPhysicManager</para>
    /// <para>衝突判定があった物質を管理します</para>
    /// </summary>
    public class CollisionPhysicManager
    {
        #region 変数
        //衝突データ管理リスト
        [SerializeField]private static List<PhysicCollision> _collisionData = new();
        private static List<bool> _physicAChecks = new();
        private static List<bool> _physicBChecks = new();
        //衝突データの確認待ち物質リスト
        private static List<PhysicMaterials> _physicLogs = new();
        #endregion

        #region メソッド
        /// <summary>
        /// <para>SetCollision</para>
        /// <para>衝突があった物質をデータとして登録します</para>
        /// </summary>
        /// <param name="physicA"></param>
        /// <param name="physicB"></param>
        /// <param name="point">衝突地点</param>
        public static void SetCollision(PhysicMaterials physicA, PhysicMaterials physicB, Vector3 point, Vector3 velocity)
        {
            //衝突している物質が同じである
            if(physicA.transform == physicB.transform)
            {
                //誤データである可能性のため処理しない
                return;
            }

            //記録されている衝突を確認
            foreach (PhysicCollision collision in _collisionData)
            {
                //既にある衝突か
                if (collision.Contains(physicA) && collision.Contains(physicB))
                {
                    //格納せず終了する
                    return;
                }
            }

            //PhysicAが登録ログに記録されていない
            if (!_physicLogs.Contains(physicA))
            {
                //PhysicAを記録
                _physicLogs.Add(physicA);
            }
            //PhysicBが登録ログに記録されていない
            if (!_physicLogs.Contains(physicB))
            {
                //PhysicBを記録
                _physicLogs.Add(physicB);
            }

            //データ追加
            _collisionData.Add(new PhysicCollision(physicA, physicB, point, velocity));
            //対象の物質にRigidbodyがついていない場合
            // -> 相手に考慮する必要はないので確認済みと判定する
            _physicAChecks.Add(!physicA.rigid);
            _physicBChecks.Add(!physicB.rigid);
        }

        /// <summary>
        /// <para>CheckRemoveCollision</para>
        /// <para>検査対象の衝突データが削除対象か検査します</para>
        /// <para>また削除対象の場合は、削除します</para>
        /// </summary>
        /// <param name="remove">検査対象</param>
        private static void CheckRemoveCollision(PhysicCollision remove)
        {
            //要素番号取得
            int listIndex = _collisionData.IndexOf(remove);

            //まだ両方とも確認済みではない
            if (!_physicAChecks[listIndex] || !_physicBChecks[listIndex])
            {
                //削除しない
                return;
            }

            //データ削除
            _collisionData.Remove(remove);
            _physicAChecks.RemoveAt(listIndex);
            _physicBChecks.RemoveAt(listIndex);
        }

        /// <summary>
        /// <para>CheckRemoveLog</para>
        /// <para>検査対象の物質が削除対象か検査します</para>
        /// <para>また削除対象の場合は、削除します</para>
        /// </summary>
        /// <param name="remove"></param>
        private static void CheckRemoveLog(PhysicMaterials remove)
        {
            //衝突データを確認
            foreach (PhysicCollision collision in _collisionData)
            {
                //関与している衝突データである かつ まだ確認済みではない
                if (collision.Contains(remove) && !PhysicToCheckLog(collision, remove))
                {
                    //削除せず
                    return;
                }
            }
            //記録削除
            _physicLogs.Remove(remove);
        }

        /// <summary>
        /// <para>PhysicToCheckLog</para>
        /// <para>検査対象が対象の衝突データを確認済みであるか返します</para>
        /// </summary>
        /// <param name="check">対象の衝突データ</param>
        /// <param name="physic">検査対象</param>
        /// <returns>確認済み判定</returns>
        private static bool PhysicToCheckLog(PhysicCollision check, PhysicMaterials physic)
        {
            //要素番号取得
            int listIndex = _collisionData.IndexOf(check);
            //どちらのPhysicか取得
            PhysicThis ans = check.Which(physic);
            
            //PhysicAである かつ まだ確認済みではない
            if(ans == PhysicThis.A && !_physicAChecks[listIndex])
            {
                return false;
            }
            //PhysicBである かつ まだ確認済みではない
            else if (ans == PhysicThis.B && !_physicBChecks[listIndex])
            {
                return false;
            }
            //確認済みではある
            return true;
        }

        /// <summary>
        /// <para>CheckIn</para>
        /// <para>対象の衝突データに対応した確認済み判定を設定します</para>
        /// </summary>
        /// <param name="target">対象の衝突データ</param>
        /// <param name="physic">対応したPhysic</param>
        private static void CheckIn(PhysicCollision target, PhysicThis physic)
        {
            //要素番号取得
            int listIndex = _collisionData.IndexOf(target);

            //PhysicAである
            if (physic == PhysicThis.A)
            {
                _physicAChecks[listIndex] = true;
            }
            //PhysicBである
            else
            {
                _physicBChecks[listIndex] = true;
            }
        }
        
        /// <summary>
        /// <para>GetCollision</para>
        /// <para>検査対象に対して対応する物理挙動データを返します</para>
        /// <para>対応するデータがない場合は、defaultを返します</para>
        /// </summary>
        /// <param name="search">検査対象</param>
        /// <returns>物理挙動データ</returns>
        public static OtherPhysicData GetCollision(PhysicMaterials search)
        {
            Debug.Log(_collisionData.Count + ":" + _physicLogs.Count);

            //記録されている衝突を確認
            foreach(PhysicCollision collision in _collisionData)
            {
                //関与している衝突データである かつ まだ確認済みではない
                if (collision.Contains(search) && !PhysicToCheckLog(collision,search))
                {
                    //使用するデータをまとめる
                    OtherPhysicData returnData = new OtherPhysicData(collision.OtherPhysic(search), collision.point, collision.velocity);
                    //確認済みと設定する
                    CheckIn(collision, collision.Which(search));
                    //衝突データを削除検査
                    CheckRemoveCollision(collision);
                    //確認待ち記録を削除検査
                    CheckRemoveLog(search);
                    return returnData;
                }
                //衝突データを削除検査
                CheckRemoveCollision(collision);
            }
            //衝突記録を削除検査
            CheckRemoveLog(search);
            //見つからない
            return default;
        }

        /// <summary>
        /// <para>CheckWaitContains</para>
        /// <para>対象の物質が確認待ちに登録されていないかチェックします</para>
        /// </summary>
        /// <param name="target">対象の物質</param>
        /// <returns>登録判定</returns>
        public static bool CheckWaitContains(PhysicMaterials target)
        {
            //確認待ちがいない または 登録されていない
            if (_physicLogs.Count == 0 || !_physicLogs.Contains(target))
            {
                //登録されていない
                return false;
            }

            //対象の物質にRigidbodyが付与されていない
            if (!target.rigid)
            {
                //確認待ちから解除する
                _physicLogs.Remove(target);
            }
            //登録されている
            return true;
        }
        
        #endregion
    }
}
