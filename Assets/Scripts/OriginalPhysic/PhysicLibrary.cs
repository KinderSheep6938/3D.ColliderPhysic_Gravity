/// -----------------------------------------------------------------
/// PhysicLibrary　Physic共通情報
/// 
/// 作成日：2023/11/06
/// 作成者：Shizuku
/// -----------------------------------------------------------------
namespace PhysicLibrary
{
    using UnityEngine;
    //重力 - 質量、重力
    //摩擦 - 摩擦力
    //反発
    //傾き

    //物理基礎データ
    [System.Serializable]
    public struct PhysicData
    {
        [SerializeField] public float mass;                         //質量
        [SerializeField] public float drug;                         //摩擦力
        [SerializeField, Range(0, 1)] public float reboundRatio;    //反発係数
        [SerializeField, ReadOnly] public Vector3 force;            //物体に加えられている力
        [SerializeField, ReadOnly] public Vector3 velocity;         //加速度
    }
}