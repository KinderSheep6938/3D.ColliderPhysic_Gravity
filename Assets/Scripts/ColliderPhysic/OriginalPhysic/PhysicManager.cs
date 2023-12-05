/// -----------------------------------------------------------------
/// PhysicManager.cs 物理挙動制御
/// 
/// 作成日：2023/11/17
/// 作成者：Shizuku
/// -----------------------------------------------------------------
namespace PhysicLibrary.Manager
{
    using UnityEngine;
    using OriginalMath;
    using PhysicLibrary.Material;
    using PhysicLibrary.CollisionPhysic;

    /// <summary>
    /// <para>PhysicManager</para>
    /// <para>物理挙動を管理します</para>
    /// </summary>
    public class PhysicManager
    {
        #region 変数
        //めり込み反発倍率
        private const float REPULESSION_RATIO = 1.02f;
        //重力
        private static readonly Vector3 _gravityScale = new(0f, -9.81f, 0f);
        //基礎ベクトル
        private static readonly Vector3 _vectorZero = Vector3.zero;
        private static readonly Vector3 _vectorRight = Vector3.right;
        private static readonly Vector3 _vectorUp = Vector3.up;
        private static readonly Vector3 _vectorForward = Vector3.forward;
        #endregion

        #region プロパティ
        //初期重量
        public static float CommonMass { get => 1f; }
        //初期重力
        public static Vector3 CommonGravity { get => _gravityScale; }
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
            //重力が０の場合は処理しない
            if (physic.gravity == _vectorZero)
            {
                return _vectorZero;
            }

            //重力加速度を算出 : 質量 x 重力
            Vector3 acceleration = physic.mass * physic.gravity;
            //重力加速度を時間積分 : 重力加速度 x 経過時間
            Vector3 gravityForce = acceleration * Time.fixedDeltaTime;
            //返却
            return gravityForce;
        }

        /// <summary>
        /// <para>NoForceToCollision</para>
        /// <para>物体にかかる力がないめり込み制御を行います</para>
        /// </summary>
        /// <param name="physic">対象の物質</param>
        /// <param name="otherPhysic">めり込み情報</param>
        /// <param name="AirResistance">空気抵抗</param>
        /// <returns>めり込み制御用の力</returns>
        public static Vector3 NoForceToCollision(PhysicData physic, OtherPhysicData otherPhysic)
        {
            //Debug.Log(physic.colliderInfo.material.transform + ":" + otherPhysic.collision.transform);
            //垂直方向の面までの距離ベクトル
            Vector3 surfaceVerticalDis = VerticalDirectionBySurface(otherPhysic,otherPhysic.collision.transform.lossyScale);
            //衝突地点までの垂直な距離ベクトル
            Vector3 collisionVerticalDis = GetTo.V3Projection(physic.colliderInfo.Edge[otherPhysic.edgeId] - otherPhysic.collision.transform.position, surfaceVerticalDis);
            //衝突地点から面までの距離ベクトルを取得
            Vector3 collisionToSurface = surfaceVerticalDis - collisionVerticalDis;
            //Debug.Log(surfaceVerticalDis + ":" + collisionVerticalDis + ":" + collisionToSurface);

            //距離ベクトルがある程度同じである場合は何もしない
            //if(Mathf.Approximately(surfaceVerticalDis.sqrMagnitude, collisionVerticalDis.sqrMagnitude))
            //{
            //    return _vectorZero;
            //}

            //衝突地点の距離ベクトルが面までの距離ベクトル以上である
            //if (surfaceVerticalDis.sqrMagnitude < collisionVerticalDis.sqrMagnitude)
            //{
            //    //めり込んでいないので、引き戻すようにくっつける
            //    //空気抵抗分を考慮して反発力を算出
            //    Vector3 pullForce = -collisionToSurface;
            //    return pullForce;
            //}
            //めり込んでいるので、押し出すようにくっつける
            //空気抵抗分を考慮して反発力を算出
            Vector3 pushForce = collisionToSurface;

            return pushForce;
        }

        /// <summary>
        /// <para>VerticalForceByCollider</para>
        /// <para>面に対し垂直方向の力を取得します</para>
        /// </summary>
        /// <param name="physic">対象の物質</param>
        /// <param name="otherPhysic">衝突情報</param>
        /// <returns>実際の力</returns>
        public static Vector3 VerticalForceByPhysicMaterials(PhysicData physic, OtherPhysicData otherPhysic)
        {
            //Debug.Log("-------------------------------------------------------------------------");
            //返却用
            Vector3 returnForce = physic.force;
            //自身の物理挙動情報を取得
            PhysicMaterials myMaterial = physic.colliderInfo.material;
            //衝突先の物理挙動情報を取得
            PhysicMaterials collisionMaterial = otherPhysic.collision;

            //垂直方向を取得
            Vector3 vertical = VerticalDirectionBySurface(otherPhysic);
            //現在の力の垂直方向に対しての力を取得
            Vector3 verticalForce = GetTo.V3Projection(returnForce, vertical);
            //Debug.Log("v:" + vertical + "f:" + verticalForce);

            //垂直方向に対しての力は面に対して表側に働く力か（めり込む方向ではない）
            bool isForceToVertical =
                Mathf.Sign(vertical.x) == Mathf.Sign(verticalForce.x)
                && Mathf.Sign(vertical.y) == Mathf.Sign(verticalForce.y)
                && Mathf.Sign(vertical.z) == Mathf.Sign(verticalForce.z);
            //垂直方向に対しての力はあるか
            bool isExistVerticalForce = verticalForce != _vectorZero;

            //表側に働く または 垂直に対しての力がない
            if (isForceToVertical || !isExistVerticalForce)
            {
                //Debug.Log("noRepu" + vertical + ":" + verticalForce);
                //反発しない
                return verticalForce;
            }

            //反発する
            //Debug.Log(returnForce + ":" + VerticalForceBySurface(collision));
            //反発係数を算出
            float combineRep = GetTo.ValueCombine(myMaterial.bounciness, collisionMaterial.bounciness, myMaterial.bounceCombine);
            //反発力を算出
            Vector3 repulsionForce = -(combineRep * verticalForce);
            //Debug.Log("f:" + returnForce + "sr:" + verticalForce);
            //Debug.Log("Re" + repulsionForce);
            //反発力を考慮した物質にかかる力を算出
            returnForce = repulsionForce;

            return returnForce;
        }

        /// <summary>
        /// <para>HorizontalForceByCollider</para>
        /// <para>面に対し水平方向の力を取得します</para>
        /// </summary>
        /// <param name="physic">対象の物質</param>
        /// <param name="otherPhysic">衝突情報</param>
        /// <returns>実際の力</returns>
        public static Vector3 HorizontalForceByPhysicMaterials(PhysicData physic, OtherPhysicData otherPhysic)
        {
            //Debug.Log("-------------------------------------------------------------------------");
            //返却用
            Vector3 returnForce = physic.force;
            //自身の物理挙動情報を取得
            PhysicMaterials myMaterial = physic.colliderInfo.material;
            //衝突先の物理挙動情報を取得
            PhysicMaterials collisionMaterial = otherPhysic.collision;

            //垂直方向を取得
            Vector3 vertical = VerticalDirectionBySurface(otherPhysic);
            //垂直抗力を算出
            Vector3 verticalResistance = GetTo.V3Projection(Gravity(physic), vertical);
            //水平に働く力を取得
            Vector3 horizontalForce = HorizontalForceBySurface(otherPhysic.collision.transform, vertical, physic.force);
            
            Debug.DrawLine(physic.colliderInfo.material.transform.position, physic.colliderInfo.material.transform.position + horizontalForce, Color.red);
            //Debug.Log("s:" + horizontalForce + "r:" + returnForce);

            //動摩擦係数を算出
            float combineDynamicDrug = GetTo.ValueCombine(myMaterial.dynamicDrug, collisionMaterial.dynamicDrug, myMaterial.drugCombine);
            //静止摩擦係数を算出
            float combineStaticDrug = GetTo.ValueCombine(myMaterial.staticDrug, collisionMaterial.staticDrug, myMaterial.drugCombine);
            //Debug.Log("v:" + vertical + verticalResistance + " :h" + horizontalForce + " | " + physic.force);
            //摩擦力を考慮した物質にかかる力を算出
            returnForce = horizontalForce + AddDrug(horizontalForce, verticalResistance, combineDynamicDrug, combineStaticDrug);
            //Debug.Log("s:" + horizontalForce + "r:" + returnForce);

            return returnForce;
        }



        /// <summary>
        /// <para>AddDrug</para>
        /// <para>摩擦力を算出します</para>
        /// </summary>
        /// <param name="horizontalForce">水平方向の力</param>
        /// <param name="verticalResistance">垂直抗力</param>
        /// <param name="dynamicDrug">動摩擦係数</param>
        /// <param name="staticDrug">静止摩擦係数</param>
        /// <returns>摩擦力</returns>
        private static Vector3 AddDrug(Vector3 horizontalForce, Vector3 verticalResistance, float dynamicDrug, float staticDrug)
        {
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

            //反発力と物質にかかる力がない
            if(repulsion == _vectorZero && force == _vectorZero)
            {
                return _vectorZero;
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
        /// <para>面に対する表の垂直方向を取得します</para>
        /// <para>大きさが設定された場合は、垂直方向をスケーリング</para>
        /// </summary>
        /// <param name="otherPhysic">衝突対象のCollider</param>
        /// <param name="scale">Colliderの大きさ</param>
        /// <returns>面に対する垂直方向</returns>
        private static Vector3 VerticalDirectionBySurface(OtherPhysicData otherPhysic, Vector3 scale = default)
        {
            //簡易衝突地点を取得
            Vector3 collsionPoint = otherPhysic.collision.transform.InverseTransformPoint(otherPhysic.point);
            //Debug.Log(otherPhysic.collision.transform + ":" + otherPhysic.point);

            //正負関係なしに一番高い方向を判定する
            //各成分の絶対値を取得
            float norX = Mathf.Abs(collsionPoint.x);
            float norY = Mathf.Abs(collsionPoint.y);
            float norZ = Mathf.Abs(collsionPoint.z);
            //各絶対値の最大値を取得
            float maxNor = Mathf.Max(norX, norY, norZ);
            //Debug.Log(collsionPoint + "xyz" + norX + ":" + norY + ":" + norZ + " colname:" + collision.Collision.collider.name);


            Vector3 transformDire;
            float scaleRatio = 0;
            //X軸が一番高い
            if(maxNor == norX)
            {
                //Scaleが設定されている
                if (scale != default)
                {
                    scaleRatio = scale.x;
                }

                //正の値である
                if (0 < collsionPoint.x)
                {
                    transformDire = otherPhysic.collision.transform.right;
                }
                //負の値である
                else
                {
                    transformDire = -otherPhysic.collision.transform.right;
                }
            }
            //Y軸が一番高い
            else if(maxNor == norY)
            {
                //Scaleが設定されている
                if (scale != default)
                {
                    scaleRatio = scale.y;
                }

                //正の値である
                if (0 < collsionPoint.y)
                {
                    transformDire = otherPhysic.collision.transform.up;
                }
                //負の値である
                else
                {
                    transformDire = -otherPhysic.collision.transform.up;
                }
            }
            //Z軸が一番高い
            else
            {
                //Scaleが設定されている
                if (scale != default)
                {
                    scaleRatio = scale.z;
                }

                //正の値である
                if (0 < collsionPoint.z)
                {
                    transformDire = otherPhysic.collision.transform.forward;
                }
                //負の値である
                else
                {
                    transformDire = -otherPhysic.collision.transform.forward;
                }
            }

            //Scale倍率が設定されている
            if(scaleRatio != 0)
            {
                //Scale倍率の絶対値の半分を方向ベクトルにかけた値 が 中心から面までの垂直な距離ベクトル となる
                transformDire *= (Mathf.Abs(scaleRatio) / GetTo.Half);
            }
            //返却
            return transformDire;
        }

        /// <summary>
        /// <para>HorizontalForceBySurface</para>
        /// <para>面に対する水平方向を取得します</para>
        /// </summary>
        /// <param name="surface">面方向を取得できるTransform</param>
        /// <param name="vertical">垂直方向</param>
        /// <returns>垂直方向以外の方向の和</returns>
        private static Vector3 HorizontalForceBySurface(Transform surface, Vector3 vertical, Vector3 force)
        {
            //返却用
            Vector3 sumHorizontal = _vectorZero;

            //垂直方向が上下である
            if (vertical == surface.up || vertical == -surface.up)
            {
                sumHorizontal += GetTo.V3Projection(force, surface.right);
                sumHorizontal += GetTo.V3Projection(force, surface.forward);
            }
            //垂直方向が左右である
            else if (vertical == surface.right || vertical == -surface.right)
            {
                sumHorizontal += GetTo.V3Projection(force, surface.up);
                sumHorizontal += GetTo.V3Projection(force, surface.forward);
            }
            //垂直方向が前後である
            else
            {
                sumHorizontal += GetTo.V3Projection(force, surface.right);
                sumHorizontal += GetTo.V3Projection(force, surface.up);
            }

            //斜面の場合は少し上に反発を付加する
            //上の計算結果で各軸のベクトルで０が存在しない（斜面である）


            return sumHorizontal;
        }

        #endregion 
    }
}
