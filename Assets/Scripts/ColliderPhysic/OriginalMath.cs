/// -----------------------------------------------------------------
/// OriginalMath.cs ����v�Z�Ǘ��p
/// 
/// �쐬���F2023/11/17
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;


namespace OriginalMath
{
    using UnityEngine;

    //�v�Z���@
    public enum Combine
    {
        Maximum,
        Minimum,
        Average,
        Multiplty
    }

    /// <summary>
    /// <para>GetTo</para>
    /// <para>����Ȍv�Z�����s���܂�</para>
    /// </summary>
    public class GetTo
    {
        #region �ϐ�
        //�񕪊��p�萔
        private const int HALF = 2;
        //�ʂ̍ő�͈�
        private const float MAXRANGE = 0.5f;
        
        //��b�x�N�g��
        private static readonly Vector2 _vectorZero = Vector2.zero;
        private static readonly Vector2 _vectorRight = Vector2.right;
        private static readonly Vector2 _vectorUp = Vector2.up;

        //�ԋp�p���������X�g
        private static readonly Vector2[] _resetReturnList = new Vector2[2];
        //�ʂɂ����钸�_���W
        private static readonly Vector2[] _planeEdge =
        {
        new Vector2(0.5f,0.5f),         //�E��
        new Vector2(0.5f,-0.5f),        //�E��
        new Vector2(-0.5f,0.5f),        //����
        new Vector2(-0.5f,-0.5f)        //����
        };
        //�ʂɂ����钸�_���W��ID�i��L�̒��_���W�ɑΉ��j
        private struct EdgeByPlane
        {
            public const int rU = 0;        //�E��
            public const int rD = 1;        //�E��
            public const int lU = 2;        //����
            public const int lD = 3;        //����
        }
        #endregion

        #region �v���p�e�B
        public static float Half { get => HALF; }
        public static float MaxRange { get => MAXRANGE; }
        #endregion

        #region ���\�b�h
        /// <summary>
        /// <para>ValueCombine</para>
        /// <para>�^����ꂽ�l��ݒ肳�ꂽ�v�Z�ł̌��ʂ�Ԃ��܂�</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="combine">�v�Z���@</param>
        /// <returns>�v�Z����</returns>
        public static float ValueCombine(float a,float b, Combine combine)
        {
            //�ݒ肳�ꂽ�v�Z���@�Ɋ�Â��Ēl��ԋp
            switch (combine)
            {
                case Combine.Maximum:   //�ő�l
                    return Mathf.Max(a, b);

                case Combine.Minimum:   //�ŏ��l
                    return Mathf.Min(a, b);

                case Combine.Average:   //���ϒl
                    return (a + b) / HALF;

                case Combine.Multiplty: //�|���Z
                    return (a * b);

                default:                //��O��0�ŕԂ�
                    return 0;
                    
            }
        }

        /// <summary>
        /// <para>V2Projection</para>
        /// <para>�Ώۃx�N�g����n�ʃx�N�g���ɑ΂��A�ˉe���s�����x�N�g�����o�͂��܂�</para>
        /// <para>Vector2��p</para>
        /// </summary>
        /// <param name="target">�Ώۃx�N�g��</param>
        /// <param name="ground">�n�ʃx�N�g��</param>
        /// <returns>�ˉe�x�N�g��</returns>
        public static Vector2 V2Projection(Vector2 target, Vector2 ground)
        {
            //�ˉe���Z�o
            Vector2 multiTG = target * ground;
            Vector2 powG = ground * ground;
            Vector2 projection = multiTG / powG;
            //Debug.Log("Pro:" + multiTG + "|" + powG + "|" + projection);
            //�v�Z
            Vector2 returnPro = ground * projection;

            return returnPro;
        }

        /// <summary>
        /// <para>V3Projection</para>
        /// <para>�Ώۃx�N�g����n�ʃx�N�g���ɑ΂��A�ˉe���s�����x�N�g�����o�͂��܂�</para>
        /// <para>Vector3��p</para>
        /// </summary>
        /// <param name="target">�Ώۃx�N�g��</param>
        /// <param name="ground">�n�ʃx�N�g��</param>
        /// <returns>�ˉe�x�N�g��</returns>
        public static Vector3 V3Projection(Vector3 target, Vector3 ground)
        {
            //�ˉe���Z�o
            float multiTG = Vector3.Dot(target,ground);
            float powG = Vector3.Dot(ground,ground);
            float projection = multiTG / powG;
            //Debug.Log("Pro:" + multiTG + "|" + powG + "|" + projection);
            //�v�Z
            Vector3 returnPro = ground * projection;

            return returnPro;
        }

        /// <summary>
        /// <para>GetSlopeByStartToOrigin</para>
        /// <para>����_���璆�S�Ɍ�������b�X�����擾���܂�</para>
        /// </summary>
        /// <param name="point">�ΏۂƂȂ�_</param>
        /// <returns>���S�ւ̊�b�X��</returns>
        public static Vector2 SlopeByPointToOrigin(Vector2 point)
        {
            //�ԋp�p
            Vector2 returnSlope = _vectorZero;

            //����
            //�͈͂��ォ
            if (MAXRANGE < point.x)
            {
                returnSlope += -_vectorRight;
            }
            //�͈͂�艺��
            else if (point.x < -MAXRANGE)
            {
                returnSlope += _vectorRight;
            }

            //�c��
            //�͈͂��ォ
            if (MAXRANGE < point.y)
            {
                returnSlope += -_vectorUp;
            }
            //�͈͂�艺��
            else if (point.y < -MAXRANGE)
            {
                returnSlope += _vectorUp;
            }

            //�ԋp
            return returnSlope;
        }

        /// <summary>
        /// <para>GetPlaneEdgeByPoint</para>
        /// <para>�ʂ̒��_���W���擾���܂�</para>
        /// </summary>
        /// <param name="point">�������W</param>
        /// <returns>�������W�ɑ΂���K�v�Ȓ��_���W���X�g</returns>
        public static Vector2[] PlaneEdgeByPoint(Vector2 point)
        {
            //�ԋp�p
            Vector2[] returnEdges = _resetReturnList;

            //�ʂ��猩���������W���ǂ̈ʒu�ɑ��݂��邩�ɂ���Ē��_���W��ύX����

            //�������W�� �E�� �܂��� ���� �ɂ���
            if ((MAXRANGE < point.x && MAXRANGE < point.y)
                || (point.x < -MAXRANGE && point.y < -MAXRANGE))
            {
                //����ƉE����ԋp
                returnEdges[0] = _planeEdge[EdgeByPlane.lU];
                returnEdges[1] = _planeEdge[EdgeByPlane.rD];
            }
            //�������W�� �E�� �܂��� ���� �ɂ���
            else if ((MAXRANGE < point.x && point.y < -MAXRANGE)
                || (point.x < -MAXRANGE && MAXRANGE < point.y))
            {
                //�E��ƍ�����ԋp
                returnEdges[0] = _planeEdge[EdgeByPlane.rU];
                returnEdges[1] = _planeEdge[EdgeByPlane.lD];
            }
            //�����Ώۂ� �����͈͓̔� �ł���
            else if (-MAXRANGE <= point.x && point.x <= MAXRANGE)
            {
                //�����Ώۂ� �㑤 �ɂ���
                if (MAXRANGE < point.y)
                {
                    //�E��ƍ����ԋp
                    returnEdges[0] = _planeEdge[EdgeByPlane.rU];
                    returnEdges[1] = _planeEdge[EdgeByPlane.lU];
                }
                //���� �ɂ���
                else
                {
                    //�E���ƍ�����ԋp
                    returnEdges[0] = _planeEdge[EdgeByPlane.rD];
                    returnEdges[1] = _planeEdge[EdgeByPlane.lD];
                }
            }
            //�����Ώۂ� �c���͈͓̔� �ł���
            else
            {
                //�����Ώۂ� �E�� �ɂ���
                if (MAXRANGE < point.x)
                {
                    //�E��ƉE����ԋp
                    returnEdges[0] = _planeEdge[EdgeByPlane.rU];
                    returnEdges[1] = _planeEdge[EdgeByPlane.rD];
                }
                //���� �ɂ���
                else
                {
                    //����ƍ�����ԋp
                    returnEdges[0] = _planeEdge[EdgeByPlane.lU];
                    returnEdges[1] = _planeEdge[EdgeByPlane.lD];
                }
            }

            //�ԋp
            return returnEdges;
        }
        #endregion
    }
}
