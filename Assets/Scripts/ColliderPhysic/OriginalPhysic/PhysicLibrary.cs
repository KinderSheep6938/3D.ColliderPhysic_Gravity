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
        /// 物理データの共通部分代入用
        /// </summary>
        /// <param name="mass">質量</param>
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

        #region 物質データ
        //材質挙動
        [System.Serializable]
        public struct PhysicMaterials
        {
            public readonly bool rigid;                                 //自身の物質化判定
            public readonly Transform transform;                        //自身のTransform
            [SerializeField, Range(0f, 1f)] public float dynamicDrug;   //動摩擦係数
            [SerializeField, Range(0f, 1f)] public float staticDrug;    //静止摩擦係数
            [SerializeField, Range(0.01f, 1f)] public float bounciness; //反発係数
            [SerializeField] public Combine drugCombine;                //摩擦力の使用値
            [SerializeField] public Combine bounceCombine;              //摩擦力の使用値

            /// <summary>
            /// <para>PhysicMaterials [readonly]</para>
            /// </summary>
            /// <param name="rigid">物体化判定</param>
            /// <param name="transform">自身のTransform</param>
            /// <param name="dynamicDrug">入力値</param>
            /// <param name="staticDrug">入力値</param>
            /// <param name="bounciness">入力値</param>
            /// <param name="drugCombine">入力項目</param>
            /// <param name="bounceCombine">入力項目</param>
            public PhysicMaterials(bool rigid, ref Transform transform, ref float dynamicDrug, ref float staticDrug, ref float bounciness, ref Combine drugCombine, ref Combine bounceCombine)
            {
                this.rigid = rigid;
                this.transform = transform;
                this.dynamicDrug = dynamicDrug;
                this.staticDrug = staticDrug;
                this.bounciness = bounciness;
                this.drugCombine = drugCombine;
                this.bounceCombine = bounceCombine;
            }

            /// <summary>
            /// <para>PhysicMaterials [copy]</para>
            /// </summary>
            /// <param name="copy">コピー元</param>
            public PhysicMaterials(PhysicMaterials copy)
            {
                this.rigid = copy.rigid;
                this.transform = copy.transform;
                this.dynamicDrug = copy.dynamicDrug;
                this.staticDrug = copy.staticDrug;
                this.bounciness = copy.bounciness;
                this.drugCombine = copy.drugCombine;
                this.bounceCombine = copy.bounceCombine;
            }
        }
        #endregion
    }

}