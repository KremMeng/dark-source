
using UnityEngine;
//加 CreateAssetMenu 特性，用于在编辑器里Create
[CreateAssetMenu(fileName = "PlayerStat", menuName = "ScriptableObject/玩家数据资源文件", order = 0)]
public class PlayerStat : EntityStat<PlayerStat> {
    //=========================运动属性=======================
    [Header("Motion Stats")] 
    public bool applySlopeFactor = true;  //是否考虑坡度因子
    public float acceleration = 13f;      //加速度
    public float airAcceleration = 13f;   //空中加速度
    public float deceleration = 28f;      //减速度
    public float groundFriction = 28f;    //地面摩擦力
    public float slopeFriction = 28f;     //斜坡摩擦力
    public float maxSpeed = 6f;           //最大速度
    public float turningDrag = 28f;       //转向时的阻尼
    public float brakeThreshold = -0.8f;  //刹车判定阈值
    public float slopeUpwardForce = 25f;  //上坡的额外推力
    public float slopeDownwardForce = 28f;//下坡的额外推力
}
