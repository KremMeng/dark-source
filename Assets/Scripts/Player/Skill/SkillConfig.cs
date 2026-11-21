using UnityEngine;
//另一套技能配置框架，草稿
[CreateAssetMenu(fileName = "SkillConfig",menuName = "ScriptableObject/技能配置",order = 3)]
public class SkillConfig : EntityStat<SkillConfig> {
    //技能动画名称
    public string AnimationName;
    public SkillReleaseData releaseData;
}
/// <summary>
/// 技能释放时的数据
/// </summary>
[System.Serializable]
public class SkillReleaseData {
    //播放粒子
    public SkillSpawnObj SpawnObj;
    //以防万一技能释放时有音效
    public AudioClip audioClip;
}
/// <summary>
/// 技能产生物体
/// </summary>
[System.Serializable]
public class SkillSpawnObj {
    //生成的预制体
    public GameObject prefab;
    //生成时的音效
    public AudioClip audioClip;
    //位置
    public Vector3 position;
    //旋转
    public Vector3 rotation;
    //缩放
    public float scale;
    //延迟时间
    public float time;
}

public class Playerr {
    public SkillConfig[] comboAttackConfig; //多段普攻用数组
    public SkillConfig currentSkillConfig;
    //Player里里传入config文件
    public void StartAttack(SkillConfig skillConfig){
        currentSkillConfig = skillConfig;
        //播放动画，此处已有状态机Change<Attack01>
        //技能释放音效
        
        //技能释放物体
    }
}

public class attackState {
    public Playerr player;
    //把释放技能作为一个单独类，里面有普攻standAttack、魔法攻击等
    //在对应的攻击状态类里,调用类似player.Attack()函数，函数传参传入配置文件
    protected void StandAttack(){
        player.StartAttack(player.comboAttackConfig[0]);
    }
}
   
    
