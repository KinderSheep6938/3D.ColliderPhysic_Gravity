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
    public Vector3[] Edge { get; }

    //�����������
    public PhysicMaterials Material { get; }
    #endregion

    #region ���\�b�h
    //�⊮����̏Փ˔���
    bool CheckCollisionToInterpolate(Vector3 velocity, bool saveCollision = false);
    #endregion
}
