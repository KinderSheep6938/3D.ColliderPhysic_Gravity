/// -----------------------------------------------------------------
/// ColliderDataLibrary�@Collider���
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
        //Transform���
        [SerializeField,ReadOnly] public Vector3 position;
        [SerializeField,ReadOnly] public Quaternion rotation;
        [SerializeField,ReadOnly] public Vector3 localScale;
        //���_���W�ۊ�
        [SerializeField,ReadOnly] public Vector3[] edgePos;
    }

    //���_���W���ʎ��ʗp
    //�O�� > �E�� > �㑤 �̊֌W��ID��U�蕪����
    public struct EdgeData
    {
        //�e���_
        public const int flontRightUp   = 0;
        public const int flontRightDown = 1;
        public const int flontLeftUp    = 2;
        public const int flontLeftDown  = 3;
        public const int backRightUp    = 4;
        public const int backRightDown  = 5;
        public const int backLeftUp     = 6;
        public const int backLeftDown   = 7;
        //�ő咸�_��
        public const int maxEdgeCnt     = 8;
    }
    #endregion
}