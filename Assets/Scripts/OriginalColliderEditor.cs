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

    //���_���W���ʗp
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
            {EdgeData.flontRightUp,EdgeData.flontRightDown },
            {EdgeData.flontLeftUp,EdgeData.flontLeftDown },
            {EdgeData.flontRightUp,EdgeData.flontLeftUp },
            {EdgeData.flontRightDown,EdgeData.flontLeftDown },
            {EdgeData.flontRightUp,EdgeData.backRightUp },
            {EdgeData.flontRightDown,EdgeData.backRightDown },
            {EdgeData.flontLeftUp,EdgeData.backLeftUp },
            {EdgeData.flontLeftDown,EdgeData.backLeftDown },
            {EdgeData.backRightUp,EdgeData.backRightDown },
            {EdgeData.backLeftUp,EdgeData.backLeftDown },
            {EdgeData.backRightUp,EdgeData.backLeftUp },
            {EdgeData.backRightDown,EdgeData.backLeftDown }
        };
        //Collider�`����c������
        private const float LINE_VIEWTIME = 0.01f;

        //��bVector���ۑ��p
        private static readonly Vector3 _vector3Up = Vector3.up;
        private static readonly Vector3 _vector3Right = Vector3.right;
        private static readonly Vector3 _vector3Left = Vector3.left;
        private static readonly Vector3 _vector3Down = Vector3.down;
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

            //�w�肳�ꂽ���_�̍��W���擾
            switch (edge)
            {
                //�O���E�� ----------------------------------------------------------------
                case EdgeData.flontRightUp:
                    returnPos = Vector3.right * scale.x + Vector3.up * scale.y + Vector3.forward * scale.z;
                    break;

                //�O���E�� ----------------------------------------------------------------
                case EdgeData.flontRightDown:
                    returnPos = Vector3.right * scale.x + Vector3.down * scale.y + Vector3.forward * scale.z;
                    break;

                //�O������ ----------------------------------------------------------------
                case EdgeData.flontLeftUp:
                    returnPos = Vector3.left * scale.x + Vector3.up * scale.y + Vector3.forward * scale.z;
                    break;

                //�O������ ----------------------------------------------------------------
                case EdgeData.flontLeftDown:
                    returnPos = Vector3.left * scale.x + Vector3.down * scale.y + Vector3.forward * scale.z;
                    break;

                //����E�� ----------------------------------------------------------------
                case EdgeData.backRightUp:
                    returnPos = Vector3.right * scale.x + Vector3.up * scale.y + Vector3.back * scale.z;
                    break;

                //����E�� ----------------------------------------------------------------
                case EdgeData.backRightDown:
                    returnPos = Vector3.right * scale.x + Vector3.down * scale.y + Vector3.back * scale.z;
                    break;

                //������� ----------------------------------------------------------------
                case EdgeData.backLeftUp:
                    returnPos = Vector3.left * scale.x + Vector3.up * scale.y + Vector3.back * scale.z;
                    break;

                //������� ----------------------------------------------------------------
                case EdgeData.backLeftDown:
                    returnPos = Vector3.left * scale.x + Vector3.down * scale.y + Vector3.back * scale.z;
                    break;

                //��O --------------------------------------------------------------------
                default:
                    returnPos = Vector3.zero;
                    break;
            }

            return returnPos;
        }

        /// <summary>
        /// <para>CheckColliderFromTransform</para>
        /// <para>Collider���X�V����K�v�����邩�������܂�</para>
        /// </summary>
        /// <returns>�X�V�۔���</returns>
        public static bool CheckColliderFromTransform(Transform transform, ColliderData collider)
        {
            //Transform����Collider�����r���A�ύX���������ꍇ�͍X�V���K�v�ł���Ɣ��肵�܂�

            //���W��r
            if(transform.position != collider.position)
            {
                return true;
            }

            //�p�x��r
            if(transform.rotation.eulerAngles != collider.rotation)
            {
                return true;
            }

            //�傫����r
            if(transform.localScale != collider.localScale)
            {
                return true;
            }

            //���ׂē����ł���
            return false;
        }


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