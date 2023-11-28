/// -----------------------------------------------------------------
/// ColliderDataManager.cs Collider情報管理
/// 
/// 作成日：2023/11/06
/// 作成者：Shizuku
/// -----------------------------------------------------------------
namespace ColliderLibrary.DataManager
{
    using System.Collections.Generic;

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
        public static ref List<ColliderData> ColliderInWorld { get => ref _collidersInWorld; }
        #endregion

        #region メソッド
        /// <summary>
        /// <para>SetColliderToWorld</para>
        /// <para>対象のCollider情報を共有リストに設定します</para>
        /// </summary>
        /// <param name="target">Collider情報</param>
        public static void SetColliderToWorld(ColliderData target)
        {
            //既に格納されているか
            if (_collidersInWorld.Contains(target))
            {
                //格納せず終了
                return;
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
        #endregion
    }
}

