using UnityEngine;
[CreateAssetMenu(fileName = "EnemyStat",menuName = "ScriptableObject/敌人数据资源文件",order = 0)]
public class EnemyStat : EntityStat<EnemyStat> {
    
    
    [Header("Attack Stats")]
    //==============攻击相关参数==================
    public float viewRange = 5.0f;

    public float contactAllowance = 0.1f;   //近身碰撞检测的容差

}
