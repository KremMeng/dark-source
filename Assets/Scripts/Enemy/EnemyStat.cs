using UnityEngine;
[CreateAssetMenu(fileName = "EnemyStat", menuName = "ScriptableObject/敌人数据资源文件", order = 1)]
public class EnemyStat :EntityStat<EnemyStat> {
    //=========================基础属性=======================
    [Header("General Stats")]
    public float turningDragMulti = 20f;
    public float accelerationMulti = 10f;
    public float maxSpeedMulti = 1.0f;
    public float decelerMulti = 5.0f;
    public float rotationSpeed = 180f;

}

