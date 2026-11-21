using UnityEngine;

[CreateAssetMenu(fileName = "SwordSlashConfig",menuName = "ScriptableObject/刀光配置",order = 2)]
public class ParticleStat : EntityStat<ParticleStat> {
    public SkillConfig[] comboAttackConfig; //多段普攻用数组
    //生成的预制体
    [Header("Prefab")] public GameObject prefab;
    //生成时的音效
    [Header("音效")] public AudioClip audioClip;
    //位置
    [Header("位置(local)")] public Vector3 position;
    //旋转
    [Header("Rotation")] public Vector3 rotation;
    //缩放
    [Header("缩放")] public float scale;
    //延迟时间
    [Header("延迟时间(秒)")] public float time;


}
