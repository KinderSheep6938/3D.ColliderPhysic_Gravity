/// -----------------------------------------------------------------
/// PhysicManager.cs
/// 
/// 作成日：2023/11/17
/// 作成者：Shizuku
/// -----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;

namespace PhysicLibrary.Manager
{
    using UnityEngine;
    using ColliderLibrary;
    using VectorMath;

    public class PhysicManager
    {
        #region 変数
        private static readonly Vector3 _gravityScale = new(0f, -0.981f, 0f);
        private static readonly Vector3 _vectorZero = Vector3.zero;
        private static readonly Vector3 _vectorRight = Vector3.right;
        private static readonly Vector3 _vectorUp = Vector3.up;
        private static readonly Vector3 _vectorForward = Vector3.forward;
        #endregion

        #region プロパティ

        #endregion

        #region メソッド
        /// <summary>
        /// <para>Gravity</para>
        /// <para>対象の物質に対する重力加速度を算出します</para>
        /// </summary>
        /// <param name="physic">対象のPhysicData</param>
        /// <returns>対象の重力加速度</returns>
        public static Vector3 Gravity(PhysicData physic)
        {
            //重力加速度を算出 : 質量 x 重力
            Vector3 acceleration = physic.mass * _gravityScale;
            //重力加速度を時間積分 : 重力加速度 x 経過時間
            Vector3 gravityForce = physic.force + (acceleration * Time.fixedDeltaTime);
            //返却
            return gravityForce;
        }

        /// <summary>
        /// <para>ForceToVelocityByCollider</para>
        /// <para>対象の力を与えられたCollider情報を加味した速度に変換します</para>
        /// </summary>
        /// <param name="physic">対象の物質</param>
        /// <param name="collision">衝突したCollider</param>
        /// <returns>実際の速度</returns>
        public static Vector3 RepulsionForceByCollider(PhysicData physic, CollisionData collision)
        {
            Debug.Log("-------------------------------------------------------------------------");
            //各軸の力を法線ベクトルに射影
            //X軸
            Vector3 normalX = ForceByNormal(_vectorRight * physic.force.x, collision);
            //Y軸
            Vector3 normalY = ForceByNormal(_vectorUp * physic.force.y, collision);
            //Z軸
            Vector3 normalZ = ForceByNormal(_vectorForward * physic.force.z, collision);

            //合計値
            Debug.Log("no" + normalX + ":" + normalY + ":" + normalZ);
            Vector3 sumNormal = normalX + normalY + normalZ;

            //反発力を算出します
            Debug.Log(sumNormal + ":" + VerticalForceBySurface(collision));
            //反発力
            Vector3 repulsionForce = -(physic.reboundRatio * GetTo.V3Projection(sumNormal,VerticalForceBySurface(collision)));
            Debug.Log("Re" + repulsionForce);
            //反発力を加算
            sumNormal = AddRepulsionForce(sumNormal, repulsionForce);

            return sumNormal;
        }

        /// <summary>
        /// <para>ForceByNormal</para>
        /// <para>力を各法線ベクトルに射影し、</para>
        /// </summary>
        /// <param name="force">射影する力</param>
        /// <param name="collision">法線ベクトル</param>
        /// <returns></returns>
        private static Vector3 ForceByNormal(Vector3 force, CollisionData collision)
        {
            //力がない場合は 0 を返す
            if(force.x == 0 && force.y == 0 && force.z == 0)
            {
                return _vectorZero;
            }

            Debug.Log("trVe:" + collision.collider.transform.up + ":" + collision.collider.transform.right + ":" + collision.collider.transform.forward);
            //力を各軸の法線ベクトルに射影
            Vector3 normalForceUD = GetTo.V3Projection(force, collision.collider.transform.up);
            Vector3 normalForceRL = GetTo.V3Projection(force, collision.collider.transform.right);
            Vector3 normalForceFB = GetTo.V3Projection(force, collision.collider.transform.forward);

            Debug.Log("no^" + normalForceUD + ":" + normalForceRL + ":" + normalForceFB);
            //射影ベクトルを合計
            Vector3 sumForce = normalForceUD + normalForceRL + normalForceFB;

            return sumForce;
        }

        /// <summary>
        /// <para>AddRepulsion</para>
        /// <para>反発力を加味した物体の力を算出します</para>
        /// </summary>
        /// <param name="force">現在の物体の力</param>
        /// <param name="repulsion">反発力</param>
        /// <returns>反発力を加味した物体の力</returns>
        private static Vector3 AddRepulsionForce(Vector3 force, Vector3 repulsion)
        {
            //返却用
            Vector3 returnVector = _vectorZero;

            //反発力の各軸に力がある場合は、その軸の力を反発力に置き換える
            //ない場合は、そのままの力を格納する

            //そもそも反発力がない
            if(repulsion == _vectorZero)
            {
                //その場で止まる
                return returnVector;
            }

            //X軸に対して反発力がある
            if(repulsion.x != 0)
            {
                //置き換え
                returnVector += _vectorRight * repulsion.x;
            }
            //反発力がない
            else
            {
                //そのまま
                returnVector += _vectorRight * force.x;
            }

            //Y軸に対して反発力がある
            if(repulsion.y != 0)
            {
                //置き換え
                returnVector += _vectorUp * repulsion.y;
            }
            //反発力がない
            else
            {
                //そのまま
                returnVector += _vectorUp * force.y;
            }

            //Z軸に対して反発力がある
            if(repulsion.z != 0)
            {
                //置き換え
                returnVector += _vectorForward * repulsion.z;
            }
            //反発力がない
            else
            {
                //そのまま
                returnVector += _vectorForward * force.z;
            }

            //返却
            return returnVector;
        }

        /// <summary>
        /// <para>VerticalForceBySurface</para>
        /// <para>面に対する垂直方向を取得します</para>
        /// </summary>
        /// <param name="collision">対象のCollider</param>
        /// <returns>面に対する垂直方向</returns>
        private static Vector3 VerticalForceBySurface(CollisionData collision)
        {
            //簡易衝突地点を取得
            Vector3 collsionPoint = collision.collider.transform.InverseTransformPoint(collision.point - collision.interpolate);

            //正負関係なしに一番高い方向を判定する
            //各成分の絶対値を取得
            float norX = Mathf.Abs(collsionPoint.x);
            float norY = Mathf.Abs(collsionPoint.y);
            float norZ = Mathf.Abs(collsionPoint.z);
            //各絶対値の最大値を取得
            float maxNor = Mathf.Max(norX, norY, norZ);
            Debug.Log(collsionPoint + "xyz" + norX + ":" + norY + ":" + norZ);

            //X軸が一番高い
            if(maxNor == norX)
            {
                //正の値である
                if(0 < collsionPoint.x)
                {
                    return -collision.collider.transform.right;
                }
                //負の値である
                else
                {
                    return collision.collider.transform.right;
                }
            }
            //Y軸が一番高い
            else if(maxNor == norY)
            {
                //正の値である
                if (0 < collsionPoint.y)
                {
                    return -collision.collider.transform.up;
                }
                //負の値である
                else
                {
                    return collision.collider.transform.up;
                }
            }
            //Z軸が一番高い
            else
            {
                //正の値である
                if (0 < collsionPoint.z)
                {
                    return -collision.collider.transform.forward;
                }
                //負の値である
                else
                {
                    return collision.collider.transform.forward;
                }
            }
        }

        
        #endregion
    }
}
