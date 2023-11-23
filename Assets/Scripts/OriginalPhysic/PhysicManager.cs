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
        /// <para>対象の力を与えられたCollider情報を加味した力に変換します</para>
        /// </summary>
        /// <param name="physic">対象の物質</param>
        /// <param name="collision">衝突したCollider</param>
        /// <returns>実際の力</returns>
        public static Vector3 RepulsionForceByCollider(PhysicData physic, CollisionData collision)
        {
            //Debug.Log("-------------------------------------------------------------------------");
            //各軸の力を法線ベクトルに射影
            //X軸
            Vector3 normalX = ForceByNormal(_vectorRight * physic.force.x, collision);
            //Y軸
            Vector3 normalY = ForceByNormal(_vectorUp * physic.force.y, collision);
            //Z軸
            Vector3 normalZ = ForceByNormal(_vectorForward * physic.force.z, collision);

            //合計値を仮として代入
            //Debug.Log("no" + normalX + ":" + normalY + ":" + normalZ);
            Vector3 returnForce = normalX + normalY + normalZ;

            //反発力を算出します
            //Debug.Log(returnForce + ":" + VerticalForceBySurface(collision));
            //反発力
            Vector3 repulsionForce = -(physic.reboundRatio * GetTo.V3Projection(returnForce,VerticalForceBySurface(collision)));
            //Debug.Log("Re" + repulsionForce);

            //垂直抗力を算出
            float verticalResistance = GetTo.V3Projection(Gravity(physic), VerticalForceBySurface(collision)).magnitude;
            //摩擦力を算出
            float drugPower = verticalResistance * physic.drug;

            //最終
            returnForce = AddRepulsionForce(returnForce, repulsionForce, drugPower);

            return returnForce;
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

            //Debug.Log("trVe:" + collision.collider.transform.up + ":" + collision.collider.transform.right + ":" + collision.collider.transform.forward);
            //力を各軸の法線ベクトルに射影
            Vector3 normalForceUD = GetTo.V3Projection(force, collision.collider.transform.up);
            Vector3 normalForceRL = GetTo.V3Projection(force, collision.collider.transform.right);
            Vector3 normalForceFB = GetTo.V3Projection(force, collision.collider.transform.forward);

            //Debug.Log("no^" + normalForceUD + ":" + normalForceRL + ":" + normalForceFB);
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
        private static Vector3 AddRepulsionForce(Vector3 force, Vector3 repulsion, float moveDrugScale)
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

                //動摩擦力を加算させる
                returnVector += MoveDrugToVector3(force.x, _vectorRight, moveDrugScale);
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

                //動摩擦力を加算させる
                returnVector += MoveDrugToVector3(force.y,_vectorUp, moveDrugScale);
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

                //動摩擦力を加算させる
                returnVector += MoveDrugToVector3(force.z, _vectorForward, moveDrugScale);
            }

            //返却
            return returnVector;
        }

        /// <summary>
        /// <para>MoveDrugToVector3</para>
        /// <para>動摩擦力をVector3に変換します</para>
        /// </summary>
        /// <param name="nowForceScale">現在の物体にかかる力</param>
        /// <param name="vector">方向</param>
        /// <param name="moveDrugScale">動摩擦力</param>
        /// <returns>Vector3に変換された動摩擦力</returns>
        private static Vector3 MoveDrugToVector3(float nowForceScale,Vector3 vector, float moveDrugScale)
        {
            //返却用
            Vector3 returnVector;

            Debug.Log("dr:" + nowForceScale + ":" + vector + ":" + moveDrugScale);

            //現在の力が摩擦力より低い
            if(Mathf.Abs(nowForceScale) <= moveDrugScale)
            {
                //後の算出が０となるように設定
                returnVector = -vector * nowForceScale;
            }
            //正の方向に働いている
            else if (0 < nowForceScale)
            {
                //摩擦力を設定
                returnVector = -vector * moveDrugScale;
            }
            //負の方向に働いている
            else
            {
                //摩擦力を設定
                returnVector = vector * moveDrugScale;
            }

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
            //Debug.Log(collsionPoint + "xyz" + norX + ":" + norY + ":" + norZ);

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
