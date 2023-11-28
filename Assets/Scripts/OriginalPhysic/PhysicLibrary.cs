/// -----------------------------------------------------------------
/// PhysicLibrary�@Physic���ʏ��
/// 
/// �쐬���F2023/11/06
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
namespace PhysicLibrary
{
    using UnityEngine;
    //�d�� - ���ʁA�d��
    //���C - ���C��
    //����
    //�X��

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
        /// �R���X�g���N�^
        /// </summary>
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

    namespace Material
    {
        using OriginalMath;

        //��b��������
        [System.Serializable]
        public struct PhysicMaterials
        {
            [HideInInspector] public Transform transform;               //���g��Transform
            [SerializeField, Range(0f, 1f)] public float dynamicDrug;   //�����C�W��
            [SerializeField, Range(0f, 1f)] public float staticDrug;    //�Î~���C�W��
            [SerializeField, Range(0.01f, 1f)] public float bounciness; //�����W��
            [SerializeField] public Combine drugCombine;                //���C�͂̎g�p�l
            [SerializeField] public Combine bounceCombine;              //���C�͂̎g�p�l
        }
    }
}