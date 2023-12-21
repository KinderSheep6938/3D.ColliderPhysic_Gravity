/// -----------------------------------------------------------------
/// ColliderDataManager.cs Collider情報管理
/// 
/// 作成日：2023/11/06
/// 作成者：Shizuku
/// -----------------------------------------------------------------
namespace ColliderLibrary.DataManager
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>ColliderDataManager</para>
    /// <para>当たり判定データを管理します</para>
    /// </summary>
    public class ColliderDataManager
    {
        #region 変数
        //Collider情報共有用
        private static List<ColliderData> _collidersInWorld = new();
        #endregion

        #region プロパティ

        #endregion

        #region メソッド
        /// <summary>
        /// <para>SetColliderToWorld</para>
        /// <para>対象のCollider情報を共有リストに設定します</para>
        /// </summary>
        /// <param name="target">Collider情報</param>
        public static void SetColliderToWorld(ref ColliderData target)
        {
            //既に格納されているか
            if (_collidersInWorld.Contains(target))
            {
                //既にあるデータを削除
                RemoveColliderToWorld(target);
            }

            //格納
            _collidersInWorld.Add(target);

        }

        /// <summary>
        /// <para>RemoveColliderToWorld</para>
        /// <para>対象のCollider情報を共有リストから削除します</para>
        /// </summary>
        /// <param name="target">Collider情報</param>
        public static void RemoveColliderToWorld(ColliderData target)
        {
            //共有リストから削除
            _collidersInWorld.Remove(target);
        }

        /// <summary>
        /// <para>GetColliderToWorld</para>
        /// <para>使用可能な保存されている全Colliderを返却します</para>
        /// </summary>
        /// <returns>全Collider</returns>
        public static ref List<ColliderData> GetColliderToWorld()
        {
            //余分なデータを削除する
            for(int i = 0;i < _collidersInWorld.Count;i++)
            { 
                //検査対象が削除されている
                if(_collidersInWorld[i].physic.transform == default)
                {
                    //削除する
                    RemoveColliderToWorld(_collidersInWorld[i]);
                    i--;
                }
            }

            //返却
            return ref _collidersInWorld;
        }
        #endregion
    }
}

