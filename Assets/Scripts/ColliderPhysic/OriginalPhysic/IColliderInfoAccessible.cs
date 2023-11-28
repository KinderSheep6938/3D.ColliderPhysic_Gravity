/// -----------------------------------------------------------------
/// IColliderInfoAccessible.Interface Physic������Collier���ւ̃A�N�Z�X����
/// 
/// �쐬���F2023/11/27
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColliderLibrary;
using PhysicLibrary.Material;

public interface IColliderInfoAccessible
{
    #region �v���p�e�B
    //���g��Collider��Transform
    public Transform Collider { get; }

    //�Փˏ��
    public CollisionData Collision { get; }
    //�Փˈʒu
    public Vector3 Point { get; }

    //�����������
    public PhysicMaterials material { get; }
    #endregion

    #region ���\�b�h
    //�⊮����̏Փ˔���
    public bool CheckCollisionToInterpolate(Vector3 velocity);
    #endregion
}
