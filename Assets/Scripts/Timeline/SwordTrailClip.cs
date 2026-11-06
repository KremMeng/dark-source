using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SwordTrailClip : PlayableAsset, ITimelineClipAsset {
    //刀光特效prefab，留空时回退到绑定的播放器上的默认prefab
    [SerializeField] public GameObject slashPrefab;
    //是否附着挂点、跟随挥剑轨迹
    [SerializeField] public bool attachToSpawnPoint = true;
    //刀光存活时长与淡出时长
    [SerializeField] public float slashDuration = 0.6f;
    [SerializeField] public float slashFadeTime = 0.4f;

    public SwordTrailBehavior Template = new SwordTrailBehavior(); //template只是模板，运行时才有数据

    public ClipCaps clipCaps => ClipCaps.None;

    //工厂方法：创建ScriptPlayable包裹的行为对象，把clip面板上的参数赋值给运行时行为
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner){
        var playable = ScriptPlayable<SwordTrailBehavior>.Create(graph,Template);
        var clone = playable.GetBehaviour();

        clone.slashPrefab = slashPrefab;
        clone.attachToSpawnPoint = attachToSpawnPoint;
        clone.slashDuration = slashDuration;
        clone.slashFadeTime = slashFadeTime;

        return playable;
    }
}
