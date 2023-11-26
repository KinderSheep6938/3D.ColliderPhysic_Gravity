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
        [SerializeField, ReadOnly] public Vector3 force;            //���̂ɉ������Ă����
        [SerializeField, ReadOnly] public Vector3 velocity;         //�����x
        public IColliderInfoAccessible colliderInfo;                //Collider���ւ̐ڑ��p�C���^�[�t�F�C�X
    }

    namespace Material
    {
        using OriginalMath;

        //��b��������
        [System.Serializable]
        public struct PhysicMaterials
        {
            public Transform transform;                                 //���g��Transform
            [SerializeField, Range(0f, 1f)] public float dynamicDrug;   //�����C�W��
            [SerializeField, Range(0f, 1f)] public float staticDrug;    //�Î~���C�W��
            [SerializeField, Range(0.01f, 1f)] public float bounciness; //�����W��
            [SerializeField] public Combine drugCombine;                //���C�͂̎g�p�l
            [SerializeField] public Combine bounceCombine;              //���C�͂̎g�p�l
        }
    }
}