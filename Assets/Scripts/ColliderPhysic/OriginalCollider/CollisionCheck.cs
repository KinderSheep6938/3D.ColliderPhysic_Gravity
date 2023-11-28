/// -----------------------------------------------------------------
/// CollisionCheck.cs �Փ˔���擾
/// 
/// �쐬���F2023/11/27
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
namespace ColliderLibrary.Collision
{
    using UnityEngine;
    using OriginalMath;

    /// <summary>
    /// <para>CollisionCheck</para>
    /// <para>Collider���Փ˂��Ă��邩�m�F���܂�</para>
    /// </summary>
    public class CollisionCheck
    {
        #region �ϐ�
        //��bVector���ۑ��p
        private static readonly Vector3 _vectorZero = Vector3.zero;
        private static readonly Vector2 _vector2Up = Vector2.up;
        private static readonly Vector2 _vector2Right = Vector2.right;

        //�Փ˔���͈͂̍ő�l
        private static readonly float _collisionRange = GetTo.MaxRange;
        #endregion

        #region ���\�b�h
        /// <summary>
        /// <para>CheckPointInCollider</para>
        /// <para>�����Ώۍ��W��Collider�����ɂ��邩�������܂�</para>
        /// </summary>
        /// <param name="check">�����Ώۍ��W</param>
        /// <param name="collider">�����Ώ�Collider</param>
        /// <returns>��������</returns>
        public static bool CheckPointInCollider(Vector3 check, Transform collider)
        {
            //�����Ώۖڐ��̃��[�J�����W
            Vector3 localPos = collider.InverseTransformPoint(check);

            //Collider�̊e�������ɊO���ɂ��邩�𔻒肷��
            //Collider�� X�� �ɂ����ĊO���ł���
            if (_collisionRange < localPos.x || localPos.x < -_collisionRange)
            {
                //�����ɂ��Ȃ�
                return false;
            }
            //Collider�� Y�� �ɂ����ĊO���ł���
            if (_collisionRange < localPos.y || localPos.y < -_collisionRange)
            {
                //�����ɂ��Ȃ�
                return false;
            }
            //Collider�� Z�� �ɂ����ĊO���ł���
            if (_collisionRange < localPos.z || localPos.z < -_collisionRange)
            {
                //�����ɂ��Ȃ�
                return false;
            }

            //�����ł���
            return true;
        }

        /// <summary>
        /// <para>CheckPlaneLineOverlap</para>
        /// <para>���_�ɑ΂��Ėʏ�Ō��Ԃ��Ƃ̂ł������Collider�ɏd�Ȃ��Ă��邩�������܂�</para>
        /// </summary>
        /// <param name="edge">�w�肷�钸�_</param>
        /// <param name="edgePos">���_���W</param>
        /// <param name="collider">�Ώ�Collider</param>
        /// <returns></returns>
        public static bool CheckPlaneLineOverlap(int edge, Vector3[] edgePos, Transform collider)
        {
            //���_�ɑ΂��āA�ʏ�Ɍ��Ԃ��Ƃ̂ł��钸�_���������܂�
            foreach (int lineEdge in EdgeLineManager.GetEdgeFromPlaneLine(edge))
            {
                //���_�Ɗe�Ώۂ̒��_�����Ԑ� �� Collider �ɏd�Ȃ邩����
                if (CheckLineOverlapByCollider(edgePos[edge], edgePos[lineEdge], collider))
                {
                    //�d�Ȃ��Ă���Ɣ���
                    return true;
                }

            }
            //�d�Ȃ��Ă��Ȃ��Ɣ���
            return false;
        }

        /// <summary>
        /// <para>CheckLineOverlapByCollider</para>
        /// <para>����Collider�ɏd�Ȃ邩�������܂�</para>
        /// </summary>
        /// <param name="startPoint">���̎n�_</param>
        /// <param name="endPoint">���̏I�_</param>
        /// <param name="collider">�Ώ�Collider</param>
        /// <returns>�d�Ȃ蔻��</returns>
        public static bool CheckLineOverlapByCollider(Vector3 startPoint, Vector3 endPoint, Transform collider)
        {
            //Debug.Log(startPoint + " SP" + endPoint + " EP");
            //���̎n�_�ƏI�_�����[�J���ϊ�
            Vector3 localStart = collider.InverseTransformPoint(startPoint);
            Vector3 localEnd = collider.InverseTransformPoint(endPoint);

            //�e�������Ƃ̖ʂŔ�����s��
            //Z�ʁiX�� �� Y���j
            Vector2 vec2Start = _vector2Right * localStart.x + _vector2Up * localStart.y;
            Vector2 vec2End = _vector2Right * localEnd.x + _vector2Up * localEnd.y;
            //�ʂɏd�Ȃ�Ȃ��ꍇ�́ACollider�ɏd�Ȃ�Ȃ��Ɣ��肷��
            if (!CheckLineOverlapByPlane(vec2Start, vec2End))
            {
                return false;
            }
            //X�ʁiY�� �� Z���j
            vec2Start = _vector2Right * localStart.z + _vector2Up * localStart.y;
            vec2End = _vector2Right * localEnd.z + _vector2Up * localEnd.y;
            //�ʂ��d�Ȃ�Ȃ��ꍇ�́ACollider�ɏd�Ȃ�Ȃ��Ɣ��肷��
            if (!CheckLineOverlapByPlane(vec2Start, vec2End))
            {
                return false;
            }
            //Y�ʁiX�� �� Z���j
            vec2Start = _vector2Right * localStart.x + _vector2Up * localStart.z;
            vec2End = _vector2Right * localEnd.x + _vector2Up * localEnd.z;
            //�ʂ��d�Ȃ�Ȃ��ꍇ�́ACollider�ɏd�Ȃ�Ȃ��Ɣ��肷��
            if (!CheckLineOverlapByPlane(vec2Start, vec2End))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// <para>CheckLineOverlapByPlane</para>
        /// <para>�����ʂɏd�Ȃ邩�������܂�</para>
        /// </summary>
        /// <param name="startPoint">���̎n�_</param>
        /// <param name="endPoint">���̏I�_</param>
        /// <returns>�d�Ȃ蔻��</returns>
        private static bool CheckLineOverlapByPlane(Vector2 startPoint, Vector2 endPoint)
        {
            //���̎n�_�ƏI�_�ŁA�Е��ł��ʏ�ɂ���ꍇ�́A�ʂɏd�Ȃ�Ɣ��肷��
            //�n�_���ʏ�ɂ���
            if (CheckPointInPlane(startPoint))
            {
                return true;
            }
            //�I�_���ʏ�ɂ���
            if (CheckPointInPlane(endPoint))
            {
                return true;
            }

            //�����ʂɏd�Ȃ邩�������n�߂�
            //���߂Ɍ����ɕK�v�Ȓ��_���W���擾����
            Vector2[] edges = GetTo.PlaneEdgeByPoint(startPoint);
            //�����̌X�����n�_����e���_���W�����Ԑ��̌X���ȓ��ł���ꍇ�́A�d�Ȃ�Ɣ��肷��
            if (CheckLineSlopeByPlane(startPoint, endPoint, edges))
            {
                return true;
            }

            //�d�Ȃ�Ȃ�
            return false;

        }

        /// <summary>
        /// <para>CheckPointInPlane</para>
        /// <para>�������W���ʏ�ɂ��邩�������܂�</para>
        /// </summary>
        /// <param name="point">�������W</param>
        /// <returns>�ʏ㔻��</returns>
        private static bool CheckPointInPlane(Vector2 point)
        {
            //���W���ʂ̉����O�ł���
            if (point.x < -_collisionRange || _collisionRange < point.x)
            {
                return false;
            }

            //���W���ʂ̏c���O�ł���
            if (point.y < -_collisionRange || _collisionRange < point.y)
            {
                return false;
            }

            //�͈͓��ł���
            return true;
        }

        /// <summary>
        /// <para>CheckLineSlopeByEdge</para>
        /// <para>�n�_�ƏI�_�����Ԑ��̌X�� �� �ʂɓ�����X�� �ł��邩�������܂�</para>
        /// </summary>
        /// <param name="start">���̎n�_</param>
        /// <param name="end">���̏I�_</param>
        /// <param name="edges">���_���W���X�g</param>
        /// <returns>�͈͓�����</returns>
        private static bool CheckLineSlopeByPlane(Vector2 start, Vector2 end, Vector2[] edges)
        {
            //�X��������ʂ� ���� ���̑傫�����ʂɏd�Ȃ邩
            if (CheckSlopeByEdgeSlope(start, end, edges) && CheckSlopeOverlapPlane(start, end))
            {
                //�͈͓��ł���
                return true;
            }

            //�͈͊O�ł���
            return false;
        }


        /// <summary>
        /// <para>CheckSlopeByEdgeSlope</para>
        /// <para>���̌X�� �� ���_�x�N�g���̌X���͈͓̔��E�Ԃɑ��݂��邩�������܂�</para>
        /// </summary>
        /// <param name="start">���̎n�_</param>
        /// <param name="end">���̏I�_</param>
        /// <param name="edges">���_���W</param>
        /// <returns>�͈͓�����</returns>
        private static bool CheckSlopeByEdgeSlope(Vector2 start, Vector2 end, Vector2[] edges)
        {
            //�X�����Z�o
            Vector2 lineSlope = end - start;
            Vector2 edgeSlope1 = edges[0] - start;
            Vector2 edgeSlope2 = edges[1] - start;

            //�e�x�N�g�������𐳋K��
            lineSlope = lineSlope.normalized;
            edgeSlope1 = edgeSlope1.normalized;
            edgeSlope2 = edgeSlope2.normalized;

            //�n�_�ƒ��_���W�����Ԑ��̌X���ɂ����� �e�����̍ő�l�E�ŏ��l �����o��
            Vector2 edgeMaxSlope = _vectorZero;
            Vector2 edgeMinSlope = _vectorZero;
            //���_���W�x�N�g���̌X����X������
            if (edgeSlope1.x < edgeSlope2.x)
            {
                edgeMaxSlope += _vector2Right * edgeSlope2.x;
                edgeMinSlope += _vector2Right * edgeSlope1.x;
            }
            else
            {
                edgeMaxSlope += _vector2Right * edgeSlope1.x;
                edgeMinSlope += _vector2Right * edgeSlope2.x;
            }
            //���_���W�x�N�g���̌X����Y������
            if (edgeSlope1.y < edgeSlope2.y)
            {
                edgeMaxSlope += _vector2Up * edgeSlope2.y;
                edgeMinSlope += _vector2Up * edgeSlope1.y;
            }
            else
            {
                edgeMaxSlope += _vector2Up * edgeSlope1.y;
                edgeMinSlope += _vector2Up * edgeSlope2.y;
            }

            //�͈͌��� ---------------------------------------------------------------------------------
            //���_���W�����݂��ɓ���������ɑ��݂��Ȃ���
            if (edges[0].x != edges[1].x && edges[0].y != edges[1].y)
            {
                //�Ώۃx�N�g���̌X�����ő�l�E�ŏ��l�͈͓̔��ł���
                if ((edgeMinSlope.x <= lineSlope.x && lineSlope.x <= edgeMaxSlope.x)
                    && (edgeMinSlope.y <= lineSlope.y && lineSlope.y <= edgeMaxSlope.y))
                {
                    //�͈͓��ł���
                    return true;
                }
                //�͈͊O�ł���
                return false;
            }
            //���_���W������X����ɑ��݂���
            else if (edges[0].x == edges[1].x)
            {
                //�Ώۃx�N�g���̌X����Y�����ő�l�E�ŏ��l�͈͓̔��ł��邩
                if (edgeMinSlope.y <= lineSlope.y && lineSlope.y <= edgeMaxSlope.y)
                {
                    //�͈͓��ł���
                    return true;
                }
                //�͈͊O�ł���
                return false;
            }
            //���_���W������Y����ɑ��݂���
            else
            {
                //�Ώۃx�N�g���̌X����Y�����ő�l�E�ŏ��l�͈̔͊O�ł��邩
                if (edgeMinSlope.x <= lineSlope.x && lineSlope.x <= edgeMaxSlope.x)
                {
                    //�͈͓��ł���
                    return true;
                }
                //�͈͊O�ł���
                return false;
            }
        }

        /// <summary>
        /// <para>CheckSlopeOverlapPlane</para>
        /// <para>�n�_�ƏI�_�����Ԑ��̌X�����ʂɏd�Ȃ邩�������܂�</para>
        /// </summary>
        /// <param name="start">���̎n�_</param>
        /// <param name="end">���̏I�_</param>
        /// <returns>�d�Ȃ蔻��</returns>
        private static bool CheckSlopeOverlapPlane(Vector2 start, Vector2 end)
        {
            //�n�_���猩���ʂւ̊�b�x�N�g�����擾
            Vector2 slopeDire = GetTo.SlopeByPointToOrigin(start);
            //�n�_����ʂɓ�����ŏ��X�������Z�o
            Vector2 centorMinSlope = GetTo.V2Projection(-start, slopeDire) + GetTo.V2Projection(start, slopeDire);
            //�n�_����I�_�ւ̌X��
            Vector2 centorSlope = GetTo.V2Projection(end - start, slopeDire);

            //�n�_���猩�����S�ւ̕�����������
            bool xDire = 0 < -start.x;
            bool yDire = 0 < -start.y;
            //���ꂼ��̌X�����ŏ��l�𒴂��邩�i���l�ł�������Ɣ��肷��j
            bool xSlope = (xDire && centorMinSlope.x <= centorSlope.x) || (!xDire && centorSlope.x <= centorMinSlope.x);
            bool ySlope = (yDire && centorMinSlope.y <= centorSlope.y) || (!yDire && centorSlope.y <= centorMinSlope.y);

            //�n�_���ʂ��猩�Ď΂߂̈ʒu�ł���
            if (slopeDire.x != 0 && slopeDire.y != 0)
            {
                //�n�_����I�_�ւ̐����ŏ��X���ȏ�ł��邩
                if (xSlope && ySlope)
                {
                    return true;
                }
            }
            //�n�_���ʂ��猩��X�������ɂ���
            else if (slopeDire.x != 0)
            {
                //X���ɂ����āA�n�_����I�_�ւ̐����ŏ��X���ȏ�ł��邩
                if (xSlope)
                {
                    return true;
                }
            }
            //�n�_���ʂ��猩��Y�������ɂ���
            else
            {
                //Y���ɂ����āA�n�_����I�_�ւ̐����ŏ��X���ȏ�ł��邩
                if (ySlope)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
