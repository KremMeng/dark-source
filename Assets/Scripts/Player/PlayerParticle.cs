using System.Collections;
using UnityEngine;
//封装粒子特效的播放暂停、各种被动特效播放时机等
public class PlayerParticle : MonoBehaviour {

    [Header("刀光Prefab引用")]
    public ParticleSystem swordSlashA;
    public ParticleSystem swordSlashB;
    public ParticleSystem swordSlashC;
    

    protected Player m_player;

    protected void Start(){
        InitializePlayer();
    }

    protected void Update(){
        HandleSwordSlash();
    }
    protected void InitializePlayer() => m_player = GetComponent<Player>();
    
    //播放特效
    public virtual void Play(ParticleSystem particle,Vector3 position){
        if (!particle.isPlaying) {
            particle.Play();
            particle.transform.SetPositionAndRotation(position,Quaternion.identity);
        }
    }
    public virtual void Stop(ParticleSystem particle){
        if (particle.isPlaying) {
            var mode = ParticleSystemStopBehavior.StopEmitting;
            particle.Stop(true,mode);
        }
    }
    /// <summary>
    /// 生成刀光
    /// </summary>
    protected virtual void SpawnSlashObject(SkillEffectObject effectObject){
        //如果有刀光go和prefab，就延迟生成
        if (effectObject != null && effectObject.prefab != null) {
            var routine = DoSpawnObject(effectObject);
            StartCoroutine(routine);
        }
    }

    /// <summary>
    /// 实例化刀光、协程延迟
    /// </summary>
    protected IEnumerator DoSpawnObject(SkillEffectObject effectObject){
        //先延迟时间
        yield return new WaitForSeconds(effectObject.delayedTime);
        //再实例化go
        GameObject swordSlash = GameObject.Instantiate(effectObject.prefab, null);//父物体直接为空
        //给技能各项变量赋值，local rot/pos
        swordSlash.transform.position = m_player.transform.position + effectObject.position;
        swordSlash.transform.eulerAngles = m_player.transform.eulerAngles + effectObject.rotation;
        swordSlash.transform.localScale = effectObject.prefab.transform.localScale * effectObject.scale;
        //PlayAudio(spawnObj.audioClip);
    }
    protected virtual void HandleSwordSlash(){
        Vector3 position = Vector3.zero;
        Vector3 height = new Vector3(0, 0.5f, 0);
        //position = m_player.transform.position + 0.5f * m_player.transform.forward + height;
        if (m_player.comboCount == 1) {
            //Play();
            Debug.Log("播放攻击一段特效");
        }

        if (m_player.comboCount == 2) {
            Play(swordSlashB,swordSlashB.transform.position);
            Debug.Log("播放攻击二段特效");
        }
        
        if (m_player.comboCount == 3) {
            Play(swordSlashC,swordSlashC.transform.position);
            Debug.Log("播放攻击三段特效");
        }
    }
}
