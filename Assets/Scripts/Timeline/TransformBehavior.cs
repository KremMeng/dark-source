using UnityEngine;
using UnityEngine.Playables;
//behavior，行为逻辑
//继承自PlayableBehaviour，用于自定义可播放项行为
public class TransformBehavior : PlayableBehaviour
{
    public Transform endLocation; //Data数据模板

    //相当于MonoBehavior的Update()
    public override void ProcessFrame(Playable playable, FrameData info, object playerData){
        Transform actor = playerData as Transform;//类型转换
        float velocity = 2.0f;
        actor.position = Vector3.Lerp(actor.position, endLocation.position, info.deltaTime * velocity);
    }
}