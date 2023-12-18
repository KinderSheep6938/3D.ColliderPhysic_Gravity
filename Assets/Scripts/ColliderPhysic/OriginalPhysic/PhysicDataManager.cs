/// -----------------------------------------------------------------
/// PhysicDataManager.cs Physic情報管理
/// 
/// 作成日：2023/11/27
/// 作成者：Shizuku
/// -----------------------------------------------------------------
namespace PhysicLibrary.DataManager
{
    using System.Collections.Generic;
    using PhysicLibrary.Material;

    /// <summary>
    /// <para>PhysicDataManager</para>
    /// <para>物理データを管理します</para>
    /// </summary>
    public class PhysicDataManager
    {
        #region 変数
        //Physic情報共有用
        private static List<PhysicMaterials> _physicsInWorld = new();
        #endregion

        #region メソッド
        /// <summary>
        /// <para>SetPhysicToWorld</para>
        /// <para>対象のPhysic情報を共有リストに設定します</para>
        /// </summary>
        /// <param name="target">追加予定のPhysic情報</param>
        public static void SetPhysicToWorld(PhysicMaterials target)
        {
            //既に格納されているか
            if (_physicsInWorld.Contains(target))
            {
                //格納せず終了
                return;
            }

            //格納
            _physicsInWorld.Add(target);
        }

        /// <summary>
        /// <para>RemovePhysicToWorld</para>
        /// <para>対象のPhysic情報を共有リストから削除します</para>
        /// </summary>
        /// <param name="target">削除対象のPhysic情報</param>
        public static void RemovePhysicToWorld(PhysicMaterials target)
        {
            //共有リストから削除
            _physicsInWorld.Remove(target);
        }

        /// <summary>
        /// <para>SearchPhysicByCollider</para>
        /// <para>共有リストから衝突情報を基に合致するPhysic情報を検索します</para>
        /// <para>見つからない場合は、自身のPhysic情報を返します</para>
        /// </summary>
        /// <param name="myData">自身のPhysic情報</param>
        /// <returns>合致したPhysic情報</returns>
        public static PhysicMaterials SearchPhysicByCollider(PhysicData myData)
        {
            //共有リストから検索
            foreach (PhysicMaterials search in _physicsInWorld)
            {
                //Debug.Log(search.transform + ":" + myData.colliderInfo.Collision.collider + "a:" + (search.transform == myData.colliderInfo.Collision.collider));
                

            }

            //見つからない場合は計算処理簡略化のため、自身のPhysic情報を返却する
            return myData.colliderInfo.Material;
        }
        #endregion

    }
}
