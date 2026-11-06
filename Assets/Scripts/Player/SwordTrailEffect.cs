using System.Collections;
using UnityEngine;

//附加到刀光实例上：限时停止粒子并自动销毁
//生命周期完全绑定在特效实例自身，玩家对象被禁用或销毁也不影响特效正常消散
public class SwordTrailEffect : MonoBehaviour {

    protected float duration;
    protected float fadeTime;

    public virtual void Play(float effectDuration, float stopFadeTime) {
        duration = effectDuration;
        fadeTime = stopFadeTime;
        StartCoroutine(StopAndDestroy());
    }

    protected virtual IEnumerator StopAndDestroy() {
        yield return new WaitForSeconds(duration);

        //停止所有粒子的发射，已发出的粒子自然消散
        var particles = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var particle in particles) {
            if (particle.isPlaying) {
                particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        yield return new WaitForSeconds(fadeTime);
        Destroy(gameObject);
    }
}
