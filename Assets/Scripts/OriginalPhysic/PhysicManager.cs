/// -----------------------------------------------------------------
/// PhysicManager.cs
/// 
/// �쐬���F2023/11/17
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhysicLibrary;

public class PhysicManager : MonoBehaviour
{
    #region �ϐ�
    private static readonly Vector3 _gravityScale = new Vector3(0f, -0.981f, 0f);
    #endregion

    #region �v���p�e�B

    #endregion

    #region ���\�b�h
    public static Vector3 Gravity(PhysicData physic)
    {
        //�d�͉����x���Z�o : ���� x �d��
        Vector3 acceleration = physic.mass * _gravityScale;
        //�d�͉����x�����Ԑϕ� : �d�͉����x x �o�ߎ���
        Vector3 velocity = physic.velocity + (acceleration * Time.fixedDeltaTime);
        //�ԋp
        return velocity;
    }
    #endregion
}
