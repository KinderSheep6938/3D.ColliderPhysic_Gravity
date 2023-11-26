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
    //���g��Collider��Transform
    public Transform Collider { get; }

    //�Փˏ��
    public CollisionData Collision { get; }
    //�Փˈʒu
    public Vector3 Point { get; }

    //�����������
    public PhysicMaterials material { get; }
}
