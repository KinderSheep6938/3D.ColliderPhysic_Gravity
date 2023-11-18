/// -----------------------------------------------------------------
/// ColliderManager.cs�@Collider���萧��
/// 
/// �쐬���F2023/11/06
/// �쐬�ҁFShizuku
/// -----------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColliderLibrary.Manager
{
    using VectorMath;

    public class ColliderManager
    {
        #region �ϐ�
        //��bVector���ۑ��p
        private static readonly Vector3 _vectorZero = Vector3.zero;
        private static readonly Vector2 _vector2Up = Vector2.up;
        private static readonly Vector2 _vector2Right = Vector2.right;

        //�Փ˔���͈͂̍ő�l
        private static readonly float _collisionRange = GetTo.MaxRange;

        //Collider��񋤗L�p
        private static List<ColliderData> _worldInColliders = new();

        #endregion

        #region �v���p�e�B

        #endregion

        #region ���\�b�h
        /// <summary>
        /// <para>SetColliderToWorld</para>
        /// <para>�Ώۂ�Collider�������L���X�g�ɐݒ肵�܂�</para>
        /// </summary>
        /// <param name="target">Collider���</param>
        public static void SetColliderToWorld(ColliderData target)
        {
            //���Ɋi�[����Ă��邩
            if (_worldInColliders.Contains(target))
            {
                //�i�[�����I��
                return;
            }

            //�i�[
            _worldInColliders.Add(target);
        }

        /// <summary>
        /// <para>RemoveColliderToWorld</para>
        /// <para>�Ώۂ�Collider�������L���X�g����폜���܂�</para>
        /// </summary>
        /// <param name="target">Collider���</param>
        public static void RemoveColliderToWorld(ColliderData target)
        {
            //���L���X�g����폜
            _worldInColliders.Remove(target);
        }

        /// <summary>
        /// <para>CheckCollision</para>
        /// <para>Collider��񂩂�Փ˂��Ă��邩�������܂�</para>
        /// </summary>
        /// <param name="collider">Collider���</param>
        /// <returns>�Փ˔���</returns>
        public static CollisionData CheckCollision(ColliderData collider)
        {
            //�ԋp�p
            CollisionData returnData = new();
            //�ԋp�p�̏Փ˂�ݒ�
            returnData.collision = true;

            //�����ΏۂɈ�ԋ߂����_���W
            Vector3 nearEdge;
            //���̒��_���W�̃C���f�b�N�X�ۑ��p
            int nearEdgeIndex;

            //���L���X�g����SCollider�����擾���A�Փˌ������s���܂�
            foreach (ColliderData target in _worldInColliders)
            {
                //�����Ώۂ� ���g �ł���
                if (target.transform == collider.transform)
                {
                    //�������Ȃ�
                    continue;
                }
                //�Փˑ����ݒ�
                returnData.collider = target;

                //���g�̒��S���W �� �����Ώۂ�Collider �̓����ɂ���
                if (CheckPointInCollider(collider.position, target))
                {
                    //Debug.Log("MyCenterCollision");
                    returnData.point = collider.position;
                    //�Փ˔��肪����
                    return returnData;
                }

                //�����Ώ��̒��S���W �� ���g��Collider �̓����ɂ���
                if (CheckPointInCollider(target.position, collider))
                {
                    //Debug.Log("CenterCollision");
                    returnData.point = target.position;
                    //�Փ˔��肪����
                    return returnData;
                }

                //���g�̒��_���W ���� �ł������Ώۂɋ߂����_���W ���i�[
                nearEdge = GetNearEdgeByCollider(target, collider.edgePos);
                //���̒��_���W�̃C���f�b�N�X�擾
                nearEdgeIndex = Array.IndexOf(collider.edgePos, nearEdge);
                //Debug.Log(nearEdgeIndex + "myNearIndex");
                //���g�̍ł��߂����_����ʏ�Ɍ��Ԃ��Ƃ̂ł���� �� �����Ώۂ�Collider �ɏd�Ȃ�
                if (CheckPlaneLineOverlap(nearEdgeIndex, collider.edgePos, target.transform))
                {
                    //Debug.Log("LineCollision ; " + target.name);
                    returnData.point = nearEdge;
                    //�Փ˔��肪����
                    return returnData;
                }

                //�����Ώۂ̒��_���W ���� �ł����g�ɋ߂����_���W ���i�[
                nearEdge = GetNearEdgeByCollider(collider, target.edgePos);
                //���̒��_���W�̃C���f�b�N�X�擾
                nearEdgeIndex = Array.IndexOf(target.edgePos, nearEdge);
                //Debug.Log(nearEdgeIndex + "NearIndex");
                //���g�̍ł��߂����_����ʏ�Ɍ��Ԃ��Ƃ̂ł���� �� �����Ώۂ�Collider �ɏd�Ȃ�
                if (CheckPlaneLineOverlap(nearEdgeIndex, target.edgePos, collider.transform))
                {
                    //Debug.Log("ColliderLineCollision : " + target.name);
                    returnData.point = nearEdge;
                    //�Փ˔��肪����
                    return returnData;
                }

            }

            //�ԋp�p������
            returnData.collision = false;
            returnData.collider = default;
            returnData.point = _vectorZero;
            //Debug.Log("NoCollision");
            //�Փ˔��肪�Ȃ�
            return returnData;
        }

        /// <summary>
        /// <para>GetNearEdgeByCollider</para>
        /// <para>�Ώۂ�Collider���瑊�ΓI�ɍł��߂����_���W���擾���܂�</para>
        /// </summary>
        /// <param name="target">�I�u�W�F�N�g���</param>
        /// <param name="edges">���_���W���X�g</param>
        /// <returns>�ł��߂����_���W</returns>
        private static Vector3 GetNearEdgeByCollider(ColliderData target, Vector3[] edges)
        {
            //�Z�o���ʕۑ��p
            float distance;
            //�ł��߂������ۑ��p�@�����l�Ƃ���-1���i�[����
            float minDistance = float.MaxValue;

            //�I�u�W�F�N�g�ۑ��p
            Transform localObj = target.transform;
            //���[�J���ϊ��p
            Vector3 localEdge;

            //�ԋp�p
            Vector3 returnPos = default;

            //���_����ł��߂����_���W������
            foreach (Vector3 edge in edges)
            {
                //���[�J���ϊ�
                localEdge = localObj.InverseTransformPoint(edge);
                //�����Z�o
                distance = Vector3.Distance(_vectorZero, localEdge);
                //Debug.Log(edge + ":" + localEdge + ":" + distance);
                //Debug.Log(distance + "dis " + localEdge + "local " + edge + "edge");
                //�Z�o���ʂ��ۑ�����Ă��鋗�����傫�� �܂��� ����łȂ� �ꍇ�͉������Ȃ�
                if (minDistance < distance)
                {
                    continue;
                }

                //�����X�V
                minDistance = distance;
                //���W�ݒ�
                returnPos = edge;
            }
            //Debug.Log(returnPos + "edge ");
            //�����I��
            return returnPos;
        }

        /// <summary>
        /// <para>CheckPointInCollider</para>
        /// <para>�����Ώۍ��W��Collider�����ɂ��邩�������܂�</para>
        /// </summary>
        /// <param name="check">�����Ώۍ��W</param>
        /// <param name="collider">�����Ώ�Collider</param>
        /// <returns>��������</returns>
        private static bool CheckPointInCollider(Vector3 check, ColliderData collider)
        {
            //�����Ώۖڐ��̃��[�J�����W
            Vector3 localPos = collider.transform.InverseTransformPoint(check);
            //Debug.Log(localPos + "local");

            //Debug.Log(localPos + "localedge");

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
        private static bool CheckPlaneLineOverlap(int edge, Vector3[] edgePos, Transform collider)
        {
            //���_�ɑ΂��āA�ʏ�Ɍ��Ԃ��Ƃ̂ł��钸�_���������܂�
            foreach (int lineEdge in EdgeLineManager.GetEdgeFromPlaneLine(edge))
            {
                //Debug.Log(lineEdge + "lineEdge" + edgePos[lineEdge]);
                //���_�Ɗe�Ώۂ̒��_�����Ԑ� �� Collider �ɏd�Ȃ邩����
                if (CheckLineOverlapByCollider(edgePos[edge], edgePos[lineEdge], collider))
                {
                    //�d�Ȃ��Ă���Ɣ���
                    return true;
                }
                //Debug.Log("--------------------------");
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
        private static bool CheckLineOverlapByCollider(Vector3 startPoint, Vector3 endPoint, Transform collider)
        {
            //Debug.Log(startPoint + " SP" + endPoint + " EP");
            //���̎n�_�ƏI�_�����[�J���ϊ�
            Vector3 localStart = collider.InverseTransformPoint(startPoint);
            Vector3 localEnd = collider.InverseTransformPoint(endPoint);

            //�e�������Ƃ̖ʂŔ�����s��
            //Z�ʁiX�� �� Y���j
            Vector2 vec2Start = _vector2Right * localStart.x + _vector2Up * localStart.y;
            Vector2 vec2End = _vector2Right * localEnd.x + _vector2Up * localEnd.y;
            //Debug.Log("Z:" + localStart + " lS" + localEnd + " lE" + vec2Start + " veS" + vec2End + " veE");
            //�ʂɏd�Ȃ�Ȃ��ꍇ�́ACollider�ɏd�Ȃ�Ȃ��Ɣ��肷��
            if (!CheckLineOverlapByPlane(vec2Start, vec2End))
            {
                return false;
            }
            //X�ʁiY�� �� Z���j
            vec2Start = _vector2Right * localStart.z + _vector2Up * localStart.y;
            vec2End = _vector2Right * localEnd.z + _vector2Up * localEnd.y;
            //Debug.Log("X:" + localStart + " lS" + localEnd + " lE" + vec2Start + " veS" + vec2End + " veE");
            //�ʂ��d�Ȃ�Ȃ��ꍇ�́ACollider�ɏd�Ȃ�Ȃ��Ɣ��肷��
            if (!CheckLineOverlapByPlane(vec2Start, vec2End))
            {
                return false;
            }
            //Y�ʁiX�� �� Z���j
            vec2Start = _vector2Right * localStart.x + _vector2Up * localStart.z;
            vec2End = _vector2Right * localEnd.x + _vector2Up * localEnd.z;
            //Debug.Log("Y:" + localStart + " lS" + localEnd + " lE" + vec2Start + " veS" + vec2End + " veE");
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
        /// <para>�Ώۃx�N�g���̌X�������_�x�N�g���̌X���͈͓̔��E�Ԃɑ��݂��邩�������܂�</para>
        /// </summary>
        /// <param name="lineSlope">�Ώۃx�N�g��</param>
        /// <param name="edges">���_���W</param>
        /// <param name="edgeMaxSlope">���_�x�N�g���̍ő�l�̌X��</param>
        /// <param name="edgeMinSlope">���_�x�N�g���̍ŏ��l�̌X��</param>
        /// <returns>�͈͓�����</returns>
        private static bool CheckSlopeByEdgeSlope(Vector2 start, Vector2 end, Vector2[] edges)
        {
            //�X�����Z�o
            Vector2 lineSlope = end - start;
            Vector2 edgeSlope1 = edges[0] - start;
            Vector2 edgeSlope2 = edges[1] - start;
            //Debug.Log(lineSlope + " lS" + edgeSlope1 + " eS1" + edgeSlope2 + " eS2");

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

            //Debug.Log("Nomal: " + lineSlope + " lS" + edgeMaxSlope + " eMaS" + edgeMinSlope + " eMiS");

            //�͈͌��� ---------------------------------------------------------------------------------
            ////�����̐�������p�itrue:�� false:���j
            //bool direSign;
            //Debug.Log("edge:" + edges[0] + "|" + edges[1]);
            //���_���W�����݂��ɓ���������ɑ��݂��Ȃ���
            if (edges[0].x != edges[1].x && edges[0].y != edges[1].y)
            {
                //Debug.Log("NoXY");
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
                //Debug.Log("X");
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
                //Debug.Log("Y");
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
            //Debug.Log(start + "|" + end);
            //�n�_���猩���ʂւ̊�b�x�N�g�����擾
            Vector2 slopeDire = GetTo.SlopeByPointToOrigin(start);
            //�n�_����ʂɓ�����ŏ��X�������Z�o
            //Debug.Log(start + " - " + GetProjection(slopeDire, start));
            Vector2 centorMinSlope = start - GetTo.Projection(slopeDire, start);
            //�n�_����I�_�ւ̌X���𒆐S�����֎ˉe
            Vector2 centorSlope = end - start;

            //�n�_���猩�����S�ւ̕�����������
            bool xDire = 0 < -start.x;
            bool yDire = 0 < -start.y;
            //���ꂼ��̌X�����ŏ��l�𒴂��邩�i���l�ł�������Ɣ��肷��j
            bool xSlope = (xDire && centorMinSlope.x <= centorSlope.x) || (!xDire && centorSlope.x <= centorMinSlope.x);
            bool ySlope = (yDire && centorMinSlope.y <= centorSlope.y) || (!yDire && centorSlope.y <= centorMinSlope.y);

            //Debug.Log(centorMinSlope + "|" + centorSlope + "|" + GetProjection(_vector2Right, _vector2Right + _vector2Up));
            //Debug.Log(xDire + ":" + yDire);
            //�n�_���ʂ��猩�Ď΂߂̈ʒu�ł���
            if (slopeDire.x != 0 && slopeDire.y != 0)
            {
                //Debug.Log("toXY");
                //�n�_����I�_�ւ̐����ŏ��X���ȏ�ł��邩
                if (xSlope && ySlope)
                {
                    return true;
                }
            }
            //�n�_���ʂ��猩��X�������ɂ���
            else if (slopeDire.x != 0)
            {
                //Debug.Log("toX");
                //X���ɂ����āA�n�_����I�_�ւ̐����ŏ��X���ȏ�ł��邩
                if (xSlope)
                {
                    return true;
                }
            }
            //�n�_���ʂ��猩��Y�������ɂ���
            else
            {
                //Debug.Log("toY");
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
