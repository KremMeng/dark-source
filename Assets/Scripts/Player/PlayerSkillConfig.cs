using System;
using System.Collections.Generic;
using UnityEngine;

//专门的Skill数据配置类，
[CreateAssetMenu(fileName = "PlayerSkillConfig",menuName = "ScriptableObject/玩家技能配置",order = 2)]
public class PlayerSkillConfig : EntityStat<PlayerSkillConfig> {

    //技能释放时实例化的对象
    [Header("实例化刀光配置")] 
    public List<SkillEffectObject> slashDatas = new List<SkillEffectObject>();

    //以防万一技能释放时有音效
    //public AudioClip audioClip;
}
//特效数据类
[Serializable]
public class SkillEffectObject {
    public String skillName;
    //生成的预制体
    public GameObject prefab;
    //生成时的音效
    //public AudioClip audioClip;
    //位置
    public Vector3 position;
    //旋转
    public Vector3 rotation;
    //缩放
    public float scale;
    //延迟时间
    public float delayedTime;
}

// public class Playerr {
//     public PlayerSkillConfig[] comboAttackConfig; //多段普攻用数组
//     public PlayerSkillConfig CurrentPlayerSkillConfig;
//     //Player里里传入config文件
//     public void StartAttack(PlayerSkillConfig playerSkillConfig){
//         CurrentPlayerSkillConfig = playerSkillConfig;
//         //播放动画，此处已有状态机Change<Attack01>
//         //技能释放音效
//         //SpawnSkillObject(skillConfig.releaseData.audioClip);
//         //技能释放物体
//         //PlayAudio(currentSkillConfig.releaseData.audioClip);
//     }
// }

// public class attackState {
//     public Playerr player;
//     //把释放技能作为一个单独类，里面有普攻standAttack、魔法攻击等
//     //在对应的攻击状态类里,调用类似player.Attack()函数，函数传参传入配置文件，此处传入PlayerParticle里的实例化函数
//     protected void StandAttack(){
//         player.StartAttack(player.comboAttackConfig[0]);
//     }
// }
/// <summary>
/// 技能释放时的数据
/// </summary>
// [System.Serializable]
// public class SkillReleaseData {
//     
// }
/// <summary>
/// 技能产生物体
/// </summary>
    
