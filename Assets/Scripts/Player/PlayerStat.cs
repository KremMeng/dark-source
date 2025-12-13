
using UnityEngine;
//加 CreateAssetMenu 特性，用于在编辑器里Create
[CreateAssetMenu(fileName = "PlayerStat", menuName = "ScriptableObject/玩家数据资源文件", order = 0)]
public class PlayerStat : EntityStat<PlayerStat> {
    
    //=========================基础属性=======================
    [Header("General Stats")]
    public float pushForce = 4f;        // 推力
    public float snapForce = 15f;       // 黏到地面的吸附力
    public float slideForce = 10f;      // 下坡的额外推力
    public float rotationSpeed = 970f;  // 角色旋转速度（度/秒）
    public float gravity = 9.8f;         // 普通重力加速度
    public float fallGravity = 12f;     // 下落时额外重力加速度  
    public float gravityMaxSpeed = 30f; // 重力作用下的最大下落速度
    
    //=========================运动属性=======================
    [Header("Motion Stats")] 
    public bool applySlopeFactor = true;  //是否考虑坡度因子
    public float acceleration = 3f;      //加速度
    public float airAcceleration = 13f;   //空中加速度
    public float deceleration = 28f;      //减速度
    public float groundFriction = 28f;    //地面摩擦力
    public float slopeFriction = 28f;     //斜坡摩擦力
    public float maxSpeed = 2.0f;           //最大速度
    public float turningDrag = 5f;       //转向时的阻尼
    public float brakeThreshold = -0.8f;  //刹车判定阈值
    public float slopeUpwardForce = 25f;  //上坡的额外推力
    public float slopeDownwardForce = 28f;//下坡的额外推力
}
