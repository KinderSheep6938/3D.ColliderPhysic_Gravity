/// -----------------------------------------------------------------
/// PhysicLibrary�@Physic���ʏ��
/// 
/// �쐬���F2023/11/06
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using UnityEngine;

namespace PhysicLibrary
{
    //�d�� - ���ʁA�d��
    //���C - ���C��
    //����
    //�X��
    
    //������b�f�[�^
    [System.Serializable]
    public struct PhysicData
    {
        [SerializeField] public float mass;                 //����
        [SerializeField] public float drug;                 //���C��
        [SerializeField, ReadOnly] public Vector3 velocity; //�����x
    }
}