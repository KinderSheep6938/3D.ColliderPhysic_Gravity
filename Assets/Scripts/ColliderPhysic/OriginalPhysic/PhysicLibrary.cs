/// -----------------------------------------------------------------
/// PhysicLibrary�@Physic���ʏ��
/// 
/// �쐬���F2023/11/06
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
namespace PhysicLibrary
{
    using UnityEngine;

    #region �����f�[�^
    //������b�f�[�^
    [System.Serializable]
    public struct PhysicData
    {
        [SerializeField] public float mass;                         //����
        [SerializeField, Range(0, 1)] public float airResistance;   //��C��R
        [SerializeField] public Vector3 gravity;                    //���̂ɂ�����d��
        [SerializeField, ReadOnly] public Vector3 force;            //���̂ɉ������Ă����
        [SerializeField, ReadOnly] public Vector3 velocity;         //�����x
        public IColliderInfoAccessible colliderInfo;                //Collider���ւ̐ڑ��p�C���^�[�t�F�C�X

        /// <summary>
        /// �����f�[�^�̋��ʕ�������p
        /// </summary>
        /// <param name="mass">����</param>
        /// <param name="gravity">���ʏd��</param>
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

        #region �����f�[�^
        //�ގ�����
        [System.Serializable]
        public struct PhysicMaterials
        {
            public readonly bool rigid;                                 //���g�̕���������
            public readonly Transform transform;                        //���g��Transform
            [SerializeField, Range(0f, 1f)] public float dynamicDrug;   //�����C�W��
            [SerializeField, Range(0f, 1f)] public float staticDrug;    //�Î~���C�W��
            [SerializeField, Range(0.01f, 1f)] public float bounciness; //�����W��
            [SerializeField] public Combine drugCombine;                //���C�͂̎g�p�l
            [SerializeField] public Combine bounceCombine;              //���C�͂̎g�p�l

            /// <summary>
            /// <para>PhysicMaterials [readonly]</para>
            /// </summary>
            /// <param name="rigid">���̉�����</param>
            /// <param name="transform">���g��Transform</param>
            /// <param name="dynamicDrug">���͒l</param>
            /// <param name="staticDrug">���͒l</param>
            /// <param name="bounciness">���͒l</param>
            /// <param name="drugCombine">���͍���</param>
            /// <param name="bounceCombine">���͍���</param>
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
            /// <param name="copy">�R�s�[��</param>
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