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
    using OriginalMath;
    using PhysicLibrary.Material;
    using PhysicLibrary.DataManager;

    public class PhysicManager
    {
        #region 変数
        //重力
        private static readonly Vector3 _gravityScale = new(0f, -9.81f, 0f);
        //基礎ベクトル
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
        /// <para>RepulsionForceByCollider</para>
        /// <para>対象の力を物質情報を考慮した値に変換します</para>
        /// </summary>
        /// <param name="physic">対象の物質</param>
        /// <returns>実際の力</returns>
        public static Vector3 ChangeForceByPhysicMaterials(PhysicData physic)
        {
            //Debug.Log("-------------------------------------------------------------------------");
            //返却用
            Vector3 returnForce = physic.force;
            //自身の物理挙動情報を取得
            PhysicMaterials myMaterial = physic.colliderInfo.material;
            //衝突先の物理挙動情報を取得
            PhysicMaterials collisionMaterial = PhysicDataManager.SearchPhysicByCollider(physic);

            //Debug.Log(myMaterial.bounciness == collisionMaterial.bounciness);

            //垂直方向を取得
            Vector3 vertical = VerticalForceBySurface(physic.colliderInfo);
            //水平方向を取得
            Vector3 horizontal = HorizontalForceBySurface(physic.colliderInfo.Collision.collider, vertical);
            //垂直抗力を算出
            Vector3 verticalResistance = GetTo.V3Projection(physic.force, vertical);
            //面に対して水平に加わっている力を算出
            Vector3 horizontalForce = GetTo.V3Projection(physic.force, horizontal);
            //Debug.Log("f:" + returnForce + "s:" + (verticalResistance + horizontalForce));

            //動摩擦係数を算出
            float combineDynamicDrug = GetTo.ValueCombine(myMaterial.dynamicDrug, collisionMaterial.dynamicDrug, myMaterial.drugCombine);
            //静止摩擦係数を算出
            float combineStaticDrug = GetTo.ValueCombine(myMaterial.staticDrug, collisionMaterial.staticDrug, myMaterial.drugCombine);
            //摩擦力を考慮した物質にかかる力を算出
            returnForce += AddDrug(horizontalForce, verticalResistance, combineDynamicDrug, combineStaticDrug);

            //Debug.Log(returnForce + ":" + VerticalForceBySurface(collision));
            //反発係数を算出
            float combineRep = GetTo.ValueCombine(myMaterial.bounciness, collisionMaterial.bounciness, myMaterial.bounceCombine);
            //反発力を算出
            Vector3 repulsionForce = -(combineRep * GetTo.V3Projection(physic.force,vertical));
            //Debug.Log("Re" + repulsionForce);
            //反発力を考慮した物質にかかる力を算出
            returnForce = AddRepulsionForce(returnForce, repulsionForce);

            return returnForce;
        }

        private static Vector3 AddDrug(Vector3 horizontalForce, Vector3 verticalResistance, float dynamicDrug, float staticDrug)
        {
            //Debug.Log(horizontalForce + ":" + verticalResistance);
            //Vector3をfloat変換
            float horizontalValue = horizontalForce.sqrMagnitude;

            //最大静止摩擦力を算出
            float maxStaticDrug = (verticalResistance * staticDrug).sqrMagnitude;

            //水平に加わる力が最大静止摩擦力以下である
            if (horizontalValue <= maxStaticDrug)
            {
                return -horizontalForce;
            }
            //動摩擦力を返却
            return -horizontalForce * dynamicDrug;
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
        private static Vector3 VerticalForceBySurface(IColliderInfoAccessible collision)
        {
            //簡易衝突地点を取得
            Vector3 collsionPoint = collision.Collision.collider.InverseTransformPoint(collision.Point);

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
                    return -collision.Collision.collider.right;
                }
                //負の値である
                else
                {
                    return collision.Collision.collider.right;
                }
            }
            //Y軸が一番高い
            else if(maxNor == norY)
            {
                //正の値である
                if (0 < collsionPoint.y)
                {
                    return -collision.Collision.collider.up;
                }
                //負の値である
                else
                {
                    return collision.Collision.collider.up;
                }
            }
            //Z軸が一番高い
            else
            {
                //正の値である
                if (0 < collsionPoint.z)
                {
                    return -collision.Collision.collider.forward;
                }
                //負の値である
                else
                {
                    return collision.Collision.collider.forward;
                }
            }
        }

        /// <summary>
        /// <para>HorizontalForceBySurface</para>
        /// <para>面に対する水平方向を取得します</para>
        /// </summary>
        /// <param name="surface">面方向を取得できるTransform</param>
        /// <param name="vertical">垂直方向</param>
        /// <returns>垂直方向以外の方向の和</returns>
        private static Vector3 HorizontalForceBySurface(Transform surface, Vector3 vertical)
        {
            //返却用
            Vector3 sumHorizontal = _vectorZero;

            //垂直方向が上下である
            if (vertical == surface.up || vertical == -surface.up)
            {
                sumHorizontal += surface.right;
                sumHorizontal += surface.forward;
                return sumHorizontal;
            }
            //垂直方向が左右である
            else if (vertical == surface.right || vertical == -surface.right)
            {
                sumHorizontal += surface.up;
                sumHorizontal += surface.forward;
                return sumHorizontal;
            }
            //垂直方向が前後である
            else
            {
                sumHorizontal += surface.right;
                sumHorizontal += surface.up;
                return sumHorizontal;
            }
        }

        #endregion 
    }
}
