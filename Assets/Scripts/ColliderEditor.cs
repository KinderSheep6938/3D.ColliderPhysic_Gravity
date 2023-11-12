/// -----------------------------------------------------------------
/// ColliderEditor.cs�@Collider��񐶐�
/// 
/// �쐬���F2023/11/06
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using UnityEngine;
using ColliderLibrary;

public static class ColliderEditor
{
    #region �ϐ�
    //�񕪊��p�萔
    private const int HALF = 2;
    //�ő咸�_���W��
    private const int MAX_EDGE = EdgeData.maxEdgeCnt;

    //���_���W�̊e�����̐������ʗp
    private const int EDGE_JUDGE_AXISX = 0;
    private const int EDGE_JUDGE_AXISY = 0;
    private const int EDGE_JUDGE_AXISZ = 4;
    #endregion

    #region �v���p�e�B

    #endregion

    #region ���\�b�h
    /// <summary>
    /// <para>SetData</para>
    /// <para>�Ώۂ̏�񂩂�Collider�����擾���܂�</para>
    /// </summary>
    /// <param name="targetObj">�Ώۂ̃I�u�W�F�N�g</param>
    /// <returns>Collider���</returns>
    public static ColliderData SetColliderDataByCube(Transform targetObj)
    {
        //�ԋp�p
        ColliderData returnData = new();

        //�A�N�Z�X���ȗ��ɂ���
        returnData.position = targetObj.position;
        returnData.rotation = targetObj.rotation;
        returnData.localScale = targetObj.localScale;

        //�I�u�W�F�N�g�̒��_���W�ݒ�
        returnData.edgePos = GetObjectEdgePos(targetObj);

        //��������
        return returnData;
    }

    /// <summary>
    /// <para>GetObjectEdgePos</para>
    /// <para>�Ώۂ̃I�u�W�F�N�g��񂩂璸�_���W���擾���܂�</para>
    /// </summary>
    /// <param name="target">�ΏۃI�u�W�F�N�g</param>
    /// <returns>���_���W�i�[���X�g</returns>
    private static Vector3[] GetObjectEdgePos(Transform target)
    {
        //�ԋp�p
        Vector3[] returnEdge = new Vector3[MAX_EDGE];
        //�I�u�W�F�N�g�̒��S���W
        Vector3 origin = target.position;
        //�I�u�W�F�N�g�̑傫��
        Vector3 scale = target.localScale;

        //�S�Ē��_���W���擾
        for(int edge = 0;edge < EdgeData.maxEdgeCnt; edge++)
        {
            returnEdge[edge] = origin + GetEdgeDistanceByScale(scale, edge, target);
        }

        //�擾����
        return returnEdge;
    }

    /// <summary>
    /// <para>GetEdgeDistanceByScale</para>
    /// <para>�w�肳�ꂽ���_���W���擾���܂�</para>
    /// </summary>
    /// <param name="scale">�I�u�W�F�N�g�̑傫��</param>
    /// <param name="edge">�w�肳�ꂽ���_</param>
    /// <param name="localObj">���[�J���ϊ��p�I�u�W�F�N�g</param>
    /// <returns>�w�肳�ꂽ���_���W</returns>
    private static Vector3 GetEdgeDistanceByScale(Vector3 scale, int edge, Transform localObj)
    {
        //�ԋp�p
        Vector3 returnPos;

        //Scale�̔����𑝉����Ƃ��Đݒ�
        scale /= HALF;

        //�e�����̍��ق��Z�o
        Vector3 scaleDisX = (localObj.right   * scale.x) * JudgeEdgeAxisX(edge);
        Vector3 scaleDisY = (localObj.up      * scale.y) * JudgeEdgeAxisY(edge);
        Vector3 scaleDisZ = (localObj.forward * scale.z) * JudgeEdgeAxisZ(edge);

        //�Z�o���ʂ����v����
        returnPos = scaleDisX + scaleDisY + scaleDisZ;
        
        //�Z�o����
        return returnPos;
    }

    /// <summary>
    /// <para>JudgeEdgeAxisX</para>
    /// <para>�w�肳�ꂽ���_���W��X���̐������擾���܂�</para>
    /// </summary>
    /// <param name="edge">���_���W</param>
    /// <returns>���_���W��X���l�̐���</returns>
    private static int JudgeEdgeAxisX(int edge)
    {
        //�E���𔻒� �i�O�����W�Q�ł���j
        bool isRight = (edge / HALF == EDGE_JUDGE_AXISX);

        //�O�q�̔�������p��
        //�E���𔻒� ���� ������W�Q�ł���
        isRight = isRight || ((edge - EDGE_JUDGE_AXISZ) / HALF == EDGE_JUDGE_AXISX && EDGE_JUDGE_AXISZ <= edge);

        //�E�����ł���
        if (isRight)
        {
            return 1;
        }
        //�������ł���
        return -1;
    }


    /// <summary>
    /// <para>JudgeEdgeAxisY</para>
    /// <para>�w�肳�ꂽ���_���W��Y���̐������擾���܂�</para>
    /// </summary>
    /// <param name="edge">���_���W</param>
    /// <returns>���_���W��Y���l�̐���</returns>
    private static int JudgeEdgeAxisY(int edge)
    {
        //�㑤�𔻒�
        bool isUp = (edge % HALF == EDGE_JUDGE_AXISY);

        //������ł���
        if (isUp)
        {
            return 1;
        }
        //�������ł���
        return -1;
    }

    /// <summary>
    /// <para>EdgeJudgeAxisZ</para>
    /// <para>�w�肳�ꂽ���_���W��Z���̐������擾���܂�</para>
    /// </summary>
    /// <param name="edge">���_���W</param>
    /// <returns>���_���W��Z���l�̐���</returns>
    private static int JudgeEdgeAxisZ(int edge)
    {
        //�O�����𔻒�
        bool isFlont = (edge < EDGE_JUDGE_AXISZ);

        //�O���ł���
        if (isFlont)
        {
            return 1;
        }
        //����ł���
        return -1;
    }
    #endregion
}
