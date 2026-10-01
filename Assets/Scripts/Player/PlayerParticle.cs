using System;
using UnityEngine;
//封装粒子特效的播放暂停、各种被动特效播放时机等
public class PlayerParticle : MonoBehaviour {

    protected void Start(){
        
    }

    protected void Update(){
        
    }

    public virtual void Play(ParticleSystem particle){
        if (!particle.isPlaying) {
            particle.Play();
        }
    }

    public virtual void Stop(ParticleSystem particle){
        if (particle.isPlaying) {
            var mode = ParticleSystemStopBehavior.StopEmitting;
            particle.Stop(true,mode);
        }
    }
}
