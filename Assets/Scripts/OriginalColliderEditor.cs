/// -----------------------------------------------------------------
/// OriginalColliderEditor.cs
/// 
/// �쐬���F2023/11/06
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using UnityEngine;
using UnityEditor;


namespace OriginalColliderEditor
{
    #region �ϐ� - ����
    //�I�u�W�F�N�g���ۑ��p
    [System.Serializable]
    public struct ColliderData
    {
        [SerializeField] public Vector3 position;
        [SerializeField] public Vector3 rotation;
        [SerializeField] public Vector3 localScale;
        [SerializeField] public Vector3[] edgePos;
    }

    //���_���W���ʎ��ʗp
    //�O�� > �E�� > �㑤 �̊֌W��ID��U�蕪����
    public struct EdgeData
    {
        public const int flontRightUp = 0;
        public const int flontRightDown = 1;
        public const int flontLeftUp = 2;
        public const int flontLeftDown = 3;
        public const int backRightUp = 4;
        public const int backRightDown = 5;
        public const int backLeftUp = 6;
        public const int backLeftDown = 7;
        public const int maxEdgeCnt = 8;
    }
    #endregion

    #region �N���X
    public static class ColliderEditor
    {
        #region �ϐ�
        //�񕪊��p�萔
        private const int HALF = 2;
        //�ő咸�_���W��
        private const int MAX_EDGE = EdgeData.maxEdgeCnt;
        //Collider�`�揈���p�i�I�u�W�F�N�g�̕ӁA���_�ƒ��_�����ԁj
        private static readonly int[,] _drowLineIndex =
        {
            //�O���S��
            {EdgeData.flontRightUp,EdgeData.flontRightDown },
            {EdgeData.flontLeftUp,EdgeData.flontLeftDown },
            {EdgeData.flontRightUp,EdgeData.flontLeftUp },
            {EdgeData.flontRightDown,EdgeData.flontLeftDown },
            //�O���������ւ̂S��
            {EdgeData.flontRightUp,EdgeData.backRightUp },
            {EdgeData.flontRightDown,EdgeData.backRightDown },
            {EdgeData.flontLeftUp,EdgeData.backLeftUp },
            {EdgeData.flontLeftDown,EdgeData.backLeftDown },
            //����S��
            {EdgeData.backRightUp,EdgeData.backRightDown },
            {EdgeData.backLeftUp,EdgeData.backLeftDown },
            {EdgeData.backRightUp,EdgeData.backLeftUp },
            {EdgeData.backRightDown,EdgeData.backLeftDown }
        };
        //Collider�`����c������
        private const float LINE_VIEWTIME = 0.01f;

        //���_���W�̊e�����̐������ʗp
        private const int EDGE_JUDGE_AXISX = 0;
        private const int EDGE_JUDGE_AXISY = 0;
        private const int EDGE_JUDGE_AXISZ = 4;

        //��bVector���ۑ��p
        private static readonly Vector3 _vector3Up = Vector3.up;
        private static readonly Vector3 _vector3Right = Vector3.right;
        private static readonly Vector3 _vector3Flont = Vector3.forward;
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
            returnData.rotation = targetObj.rotation.eulerAngles;
            returnData.localScale = targetObj.localScale;

            //�I�u�W�F�N�g�̒��_���W�ݒ�
            returnData.edgePos = GetObjectEdgePos(returnData.position, returnData.localScale);

            Debug.Log("Create");
            return returnData;
        }

        /// <summary>
        /// <para>GetObjectEdgePos</para>
        /// <para>�Ώۂ̃I�u�W�F�N�g��񂩂璸�_���W���擾���܂�</para>
        /// </summary>
        /// <param name="Origin">�I�u�W�F�N�g�̒��S</param>
        /// <param name="scale">�I�u�W�F�N�g�̑傫��</param>
        /// <returns>���_���W�i�[���X�g</returns>
        private static Vector3[] GetObjectEdgePos(Vector3 Origin, Vector3 scale)
        {
            //�ԋp�p
            Vector3[] returnEdge = new Vector3[MAX_EDGE];

            //���_���W�擾
            returnEdge[EdgeData.flontRightUp] = Origin + GetEdgeDistanceByScale(scale, EdgeData.flontRightUp);
            returnEdge[EdgeData.flontRightDown] = Origin + GetEdgeDistanceByScale(scale, EdgeData.flontRightDown);
            returnEdge[EdgeData.flontLeftUp] = Origin + GetEdgeDistanceByScale(scale, EdgeData.flontLeftUp);
            returnEdge[EdgeData.flontLeftDown] = Origin + GetEdgeDistanceByScale(scale, EdgeData.flontLeftDown);
            returnEdge[EdgeData.backRightUp] = Origin + GetEdgeDistanceByScale(scale, EdgeData.backRightUp);
            returnEdge[EdgeData.backRightDown] = Origin + GetEdgeDistanceByScale(scale, EdgeData.backRightDown);
            returnEdge[EdgeData.backLeftUp] = Origin + GetEdgeDistanceByScale(scale, EdgeData.backLeftUp);
            returnEdge[EdgeData.backLeftDown] = Origin + GetEdgeDistanceByScale(scale, EdgeData.backLeftDown);

            return returnEdge;
        }

        /// <summary>
        /// <para>GetEdgeDistanceByScale</para>
        /// <para>�w�肳�ꂽ���_���W���擾���܂�</para>
        /// </summary>
        /// <param name="scale">�I�u�W�F�N�g�̑傫��</param>
        /// <param name="edge">�w�肳�ꂽ���_</param>
        /// <returns>�w�肳�ꂽ���_���W</returns>
        private static Vector3 GetEdgeDistanceByScale(Vector3 scale, int edge)
        {
            //�ԋp�p
            Vector3 returnPos;

            //Scale�̔����𑝉����Ƃ��Đݒ�
            scale /= HALF;

            //�e�����̍��ق��Z�o
            Vector3 scaleDisX = _vector3Right * scale.x * JudgeEdgeAxisX(edge);
            Vector3 scaleDisY = _vector3Up * scale.y * JudgeEdgeAxisY(edge);
            Vector3 scaleDisZ = _vector3Flont * scale.z * JudgeEdgeAxisZ(edge);

            //�Z�o���ʂ����v����
            returnPos = scaleDisX + scaleDisY + scaleDisZ;

            return returnPos;
        }

        #region ���_���WID����e���̐������擾
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

        /// <summary>
        /// <para>DrowCollider</para>
        /// <para>�R���C�_�[��`�悵�܂�</para>
        /// </summary>
        /// <param name="edge"></param>
        public static void DrowCollider(ColliderData col)
        {
            //���_���W
            Vector3[] edgePoss = col.edgePos;
            //�`�惊�X�g�Q�Ɨp
            int startPosIndex = 0;
            int endPosIndex = 1;
            //�`��Ώۂ̒��_���W
            Vector3 startPos;
            Vector3 endPos;

            Debug.Log(edgePoss.Length + "----");
            //Collider�̕ӂ�`��
            for(int lineIndex = 0;lineIndex < _drowLineIndex.GetLength(0); lineIndex++)
            {
                Debug.Log(lineIndex);
                //���_���W��ݒ�
                startPos = edgePoss[_drowLineIndex[lineIndex, startPosIndex]];
                endPos = edgePoss[_drowLineIndex[lineIndex, endPosIndex]];

                //�`��
                Debug.DrawLine(startPos, endPos, Color.green,LINE_VIEWTIME);
            }
        }
        #endregion
    }
    #endregion
}