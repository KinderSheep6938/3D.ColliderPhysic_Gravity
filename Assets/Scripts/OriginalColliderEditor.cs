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
        [SerializeField] public ColliderEdge edgePos;
    }

    //�I�u�W�F�N�g�̒��_���W�ۑ��p
    [System.Serializable]
    public struct ColliderEdge
    {
        [SerializeField] public Vector3 flontRightUp;
        [SerializeField] public Vector3 flontRightDown;
        [SerializeField] public Vector3 flontLeftUp;
        [SerializeField] public Vector3 flontLeftDown;
        [SerializeField] public Vector3 backRightUp;
        [SerializeField] public Vector3 backRightDown;
        [SerializeField] public Vector3 backLeftUp;
        [SerializeField] public Vector3 backLeftDown;
    }
    #endregion

    #region �N���X
    public static class ColliderEditor
    {
        #region �ϐ�
        //�񕪊��p�萔
        private const int HALF = 2;

        //���_���W���ʗp
        private enum Edge
        {
            flontRightUp,
            flontRightDown,
            flontLeftUp,
            flontLeftDown,
            backRightUp,
            backRightDown,
            backLeftUp,
            backLeftDown
        }
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
        /// <returns></returns>
        private static ColliderEdge GetObjectEdgePos(Vector3 Origin, Vector3 scale)
        {
            //�ԋp�p
            ColliderEdge returnEdge = new();

            //���_���W�擾
            returnEdge.flontRightUp = Origin + GetEdgeDistanceByScale(scale, Edge.flontRightUp);
            returnEdge.flontRightDown = Origin + GetEdgeDistanceByScale(scale, Edge.flontRightDown);
            returnEdge.flontLeftUp = Origin + GetEdgeDistanceByScale(scale, Edge.flontLeftUp);
            returnEdge.flontLeftDown = Origin + GetEdgeDistanceByScale(scale, Edge.flontLeftDown);
            returnEdge.backRightUp = Origin + GetEdgeDistanceByScale(scale, Edge.backRightUp);
            returnEdge.backRightDown = Origin + GetEdgeDistanceByScale(scale, Edge.backRightDown);
            returnEdge.backLeftUp = Origin + GetEdgeDistanceByScale(scale, Edge.backLeftUp);
            returnEdge.backLeftDown = Origin + GetEdgeDistanceByScale(scale, Edge.backLeftDown);

            return returnEdge;
        }

        /// <summary>
        /// <para>GetScaleEdgeDis</para>
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        private static Vector3 GetEdgeDistanceByScale(Vector3 scale, Edge edge)
        {
            //�ԋp�p
            Vector3 returnPos;

            //Scale�̔����𑝉����Ƃ��Đݒ�
            scale /= HALF;

            //�w�肳�ꂽ���_�̍��W���擾
            switch (edge)
            {
                //�O���E�� ----------------------------------------------------------------
                case Edge.flontRightUp:
                    returnPos = Vector3.right * scale.x + Vector3.up * scale.y + Vector3.forward * scale.z;
                    break;

                //�O���E�� ----------------------------------------------------------------
                case Edge.flontRightDown:
                    returnPos = Vector3.right * scale.x + Vector3.down * scale.y + Vector3.forward * scale.z;
                    break;

                //�O������ ----------------------------------------------------------------
                case Edge.flontLeftUp:
                    returnPos = Vector3.left * scale.x + Vector3.up * scale.y + Vector3.forward * scale.z;
                    break;

                //�O������ ----------------------------------------------------------------
                case Edge.flontLeftDown:
                    returnPos = Vector3.left * scale.x + Vector3.down * scale.y + Vector3.forward * scale.z;
                    break;

                //����E�� ----------------------------------------------------------------
                case Edge.backRightUp:
                    returnPos = Vector3.right * scale.x + Vector3.up * scale.y + Vector3.back * scale.z;
                    break;

                //����E�� ----------------------------------------------------------------
                case Edge.backRightDown:
                    returnPos = Vector3.right * scale.x + Vector3.down * scale.y + Vector3.back * scale.z;
                    break;

                //������� ----------------------------------------------------------------
                case Edge.backLeftUp:
                    returnPos = Vector3.left * scale.x + Vector3.up * scale.y + Vector3.back * scale.z;
                    break;

                //������� ----------------------------------------------------------------
                case Edge.backLeftDown:
                    returnPos = Vector3.left * scale.x + Vector3.down * scale.y + Vector3.back * scale.z;
                    break;

                //��O --------------------------------------------------------------------
                default:
                    returnPos = Vector3.zero;
                    break;
            }

            return returnPos;
        }
        #endregion
    }
    #endregion
}