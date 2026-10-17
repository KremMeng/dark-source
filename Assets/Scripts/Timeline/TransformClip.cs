using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TransformClip : PlayableAsset,ITimelineClipAsset {
    //template只是模板，运行时才有数据
    public TransformBehavior template = new TransformBehavior();
    public ExposedReference<Transform> endLocation; //目标位置的“引用”占位符，运行时解析实际值
    public ClipCaps clipCaps => ClipCaps.None; //融合方式
   
    //工厂方法，创建一种特殊的Playable对象,运行时填好引用
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner){
        //要将 PlayableBehaviour 用作自定义可播放项，还必须将其封装在 ScriptPlayable<> 对象内
        var playable = ScriptPlayable<TransformBehavior>.Create(graph,template);//创建scriptPlayable<>对象
        var clone = playable.GetBehaviour(); //从ScriptPlayable里获取PlayableBehaviour对象
        clone.endLocation = endLocation.Resolve(graph.GetResolver());//运行时从graph里解压成真正的引用
        return playable;
    }
}
