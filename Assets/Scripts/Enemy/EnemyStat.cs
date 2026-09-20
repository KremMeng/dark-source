using UnityEngine;
[CreateAssetMenu(fileName = "EnemyStat",menuName = "ScriptableObject/敌人数据资源文件",order = 0)]
public class EnemyStat : EntityStat<EnemyStat> {
    
    
    [Header("Attack Stats")]
    //==============攻击相关参数==================
    public float viewRange = 5.0f;

    public float attackRange = 1.0f;

    public float contactAllowance = 0.05f;   //近身碰撞检测的容差

    public float chaseSpeed = 5.0f;  //追逐角色的速度
    
    public float turningSpeed = 25.0f;  //转向的速度
    
    public float turningDrag = 15.0f;  //敌人转向的阻尼
    
    public float acceleration = 6.0f;  //敌人加速度
    public float deceleration = 6.0f;

    public float maxSpeed = 7.0f; //最大行动速度

    public int health = 10; //敌人血量

}
