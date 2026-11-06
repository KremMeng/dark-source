using UnityEngine;
using UnityEngine.Playables;

//刀光行为：clip激活的首帧，通过轨道绑定的播放器实例化刀光，生成位置在此时动态赋值
public class SwordTrailBehavior : PlayableBehaviour
{
    public GameObject slashPrefab;
    public bool attachToSpawnPoint;
    public float slashDuration;
    public float slashFadeTime;

    protected bool spawned;//clip有效期内只生成一次刀光

    public override void OnBehaviourPlay(Playable playable, FrameData info){
        spawned = false;
    }

    //绑定对象通过playerData传入，相当于clip与场景里刀光播放器的连接通道
    public override void ProcessFrame(Playable playable, FrameData info, object playerData){
        if (spawned) {
            return;
        }

        var trailPlayer = playerData as PlayerSwordTrail;
        if (trailPlayer == null) {
            return;
        }

        //编辑器里拖动时间轴时不实例化，只在运行时生效
        if (!Application.isPlaying) {
            return;
        }

        spawned = true;
        //clip上的prefab为空时回退到播放器上的默认prefab
        var prefab = slashPrefab != null ? slashPrefab : trailPlayer.slashPrefab;
        if (prefab == null) {
            return;
        }
        trailPlayer.Play(prefab, attachToSpawnPoint, slashDuration, slashFadeTime);
    }
}
