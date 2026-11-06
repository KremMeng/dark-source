using UnityEngine;

//挥剑刀光播放器：攻击时实例化刀光prefab，生成位置由挂点动态赋值
//可被状态机直接调用，也可作为自定义Timeline轨道的绑定对象
public class PlayerSwordTrail : MonoBehaviour {

    [Header("Sword Trail VFX")]
    [Tooltip("刀光特效prefab，例如 SwordSlashA")]
    public GameObject slashPrefab;
    [Tooltip("刀光生成挂点（如右手腕骨骼下的空物体），留空时自动查找角色 Wrist_R")]
    public Transform slashSpawnPoint;
    [Tooltip("刀光存活时长，超时停止发射粒子（prefab 的粒子是循环播放，必须由代码控制销毁）")]
    public float slashDuration = 0.6f;
    [Tooltip("粒子停止发射后的残留淡出时间，之后销毁特效实例")]
    public float slashFadeTime = 0.4f;

    protected Transform cachedSpawnPoint;//避免每次攻击都遍历骨骼

    /// <summary>
    /// 状态机攻击入口：用组件上配置的刀光prefab与时长播放
    /// </summary>
    public virtual GameObject Play() {
        return Play(slashPrefab, true, slashDuration, slashFadeTime);
    }

    /// <summary>
    /// 实例化刀光，位置/旋转由挂点当前值动态赋值；
    /// attachToSpawnPoint 为 true 时刀光作为挂点子物体，跟随挥剑轨迹
    /// </summary>
    public virtual GameObject Play(GameObject prefab, bool attachToSpawnPoint, float duration, float fadeTime) {
        if (prefab == null) {
            return null;
        }
        var spawn = ResolveSpawnPoint();
        if (spawn == null) {
            return null;
        }

        GameObject instance;
        if (attachToSpawnPoint) {
            //worldPositionStays=false：刀光取prefab原始本地变换，正好落在挂点上
            instance = Instantiate(prefab, spawn, false);
        }
        else {
            //不附着时取生成瞬间的世界位置，固定在世界空间
            instance = Instantiate(prefab, spawn.position, spawn.rotation);
        }

        //生命周期与特效实例绑定，由特效自己负责停止粒子并销毁
        instance.AddComponent<SwordTrailEffect>().Play(duration, fadeTime);
        return instance;
    }

    /// <summary>
    /// 解析刀光挂点：优先用Inspector指定值，否则在子层级中查找右手腕骨骼
    /// </summary>
    protected virtual Transform ResolveSpawnPoint() {
        if (slashSpawnPoint != null) {
            return slashSpawnPoint;
        }
        if (cachedSpawnPoint != null) {
            return cachedSpawnPoint;
        }

        var bones = GetComponentsInChildren<Transform>(true);
        foreach (var bone in bones) {
            if (bone.name == "Wrist_R") {
                cachedSpawnPoint = bone;
                return cachedSpawnPoint;
            }
        }
        return null;
    }
}
