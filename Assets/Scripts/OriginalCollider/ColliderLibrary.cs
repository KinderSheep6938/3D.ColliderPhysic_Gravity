/// -----------------------------------------------------------------
/// ColliderDataLibrary�@Collider���ʏ��
/// 
/// �쐬���F2023/11/06
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using UnityEngine;

namespace ColliderLibrary
{
    #region �f�[�^�\��
    //�I�u�W�F�N�g���ۑ��p
    [System.Serializable]
    public struct ColliderData
    {
        //���[�J���ϊ��pTransform
        [SerializeField, ReadOnly] public Transform transform;
        //Transform���
        [SerializeField, ReadOnly] public Vector3 position;
        [SerializeField, ReadOnly] public Quaternion rotation;
        [SerializeField, ReadOnly] public Vector3 localScale;
        //���_���W�ۊ�
        [SerializeField, ReadOnly] public Vector3[] edgePos;
    }

    //���_���W���ʎ��ʗp
    //�O�� > �E�� > �㑤 �̊֌W��ID��U�蕪����
    public struct EdgeData
    {
        //�e���_
        public const int fRU    = 0; //�O���E��
        public const int fRD    = 1; //�O���E��
        public const int fLU    = 2; //�O������
        public const int fLD    = 3; //�O������
        public const int bRU    = 4; //����E��
        public const int bRD    = 5; //����E��
        public const int bLU    = 6; //�������
        public const int bLD    = 7; //�������
    }

    //�Փˏ��ۊǗp
    public struct CollisionData
    {
        //�Փ˔���
        public bool collision;
        //�Փˑ���
        public ColliderData collider;
        //�ȈՏՓˏꏊ
        public Vector3 point;
    }
    #endregion


    /// <summary>
    /// <para>EdgeLine</para>
    /// <para>���_���W�����Ԑ����Ǘ����܂�</para>
    /// </summary>
    public class EdgeLineManager
    {
        #region �ϐ�
        //�ԋp�p�z�񏉊����p
        private static readonly int[] _resetReturnList = new int[6];
        //�ő咸�_���W��
        private const int MAX_EDGEINDEX = 7;
        #endregion

        #region �v���p�e�B
        //���_���W�f�[�^�̍ő�C���f�b�N�X
        public static int MaxEdge { get => MAX_EDGEINDEX + 1; }
        #endregion

        #region ���\�b�h
        /// <summary>
        /// <para>GetEdgeFromPlaneLine</para>
        /// <para>�w�肵�����_�ɑ΂��āA�ʏ�Ō��Ԃ��Ƃ̂ł��钸�_��z��Ŏ擾����</para>
        /// </summary>
        /// <param name="edge">�w�肵�����_</param>
        /// <returns>�ʏ�Ō��Ԃ��Ƃ̂ł��钸�_���W�z��</returns>
        public static int[] GetEdgeFromPlaneLine(int edge)
        {
            //�ԋp�p�z��
            int[] returnEdges = _resetReturnList;
            //�w�肵�����_����^���΂̒��_�ȊO���i�[
            int listIndex = 0;
            int checkEdge = -1;
            //���_������������
            while(checkEdge < MAX_EDGEINDEX)
            {
                //�����C���f�b�N�X���Z
                checkEdge++;
                //Debug.Log(edge + ":" + checkEdge + ":" + listIndex);
                //�w�肵�����_���g �܂��� �^���΂̒��_ �ł��鎞�͏�����߂�
                if (edge == checkEdge || MAX_EDGEINDEX - edge == checkEdge)
                {
                    continue;
                }
                //�Ώۂ̒��_���i�[
                returnEdges[listIndex] = checkEdge;
                listIndex++;
            }
            //Debug.Log("return" + returnEdges.Length) ;
            //�ԋp
            return returnEdges;
        }
        #endregion
    }
}