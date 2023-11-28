/// -----------------------------------------------------------------
/// PhysicLibrary　Physic共通情報
/// 
/// 作成日：2023/11/06
/// 作成者：Shizuku
/// -----------------------------------------------------------------
namespace PhysicLibrary
{
    using UnityEngine;

    #region 物理データ
    //物理基礎データ
    [System.Serializable]
    public struct PhysicData
    {
        [SerializeField] public float mass;                         //質量
        [SerializeField, Range(0, 1)] public float airResistance;   //空気抵抗
        [SerializeField] public Vector3 gravity;                    //物体にかかる重力
        [SerializeField, ReadOnly] public Vector3 force;            //物体に加えられている力
        [SerializeField, ReadOnly] public Vector3 velocity;         //加速度
        public IColliderInfoAccessible colliderInfo;                //Collider側への接続用インターフェイス

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="gravity">共通重力</param>
        public PhysicData(float mass, Vector3 gravity)
        {
            this.mass = mass;
            this.airResistance = 0;
            this.gravity = gravity;
            this.force = default;
            this.velocity = default;
            this.colliderInfo = default;
        }
    }
    #endregion

    namespace Material
    {
        using OriginalMath;

        #region 物理材質
        //材質挙動
        [System.Serializable]
        public struct PhysicMaterials
        {
            [HideInInspector] public Transform transform;               //自身のTransform
            [SerializeField, Range(0f, 1f)] public float dynamicDrug;   //動摩擦係数
            [SerializeField, Range(0f, 1f)] public float staticDrug;    //静止摩擦係数
            [SerializeField, Range(0.01f, 1f)] public float bounciness; //反発係数
            [SerializeField] public Combine drugCombine;                //摩擦力の使用値
            [SerializeField] public Combine bounceCombine;              //摩擦力の使用値
        }
        #endregion
    }
}