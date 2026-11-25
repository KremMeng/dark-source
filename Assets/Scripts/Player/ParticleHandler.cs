using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Player))]
//封装粒子特效的播放暂停、各种被动特效播放时机等
public class ParticleHandler : MonoBehaviour {
    
    protected Player m_player;
    protected PlayerParticleStatManager stat;

    protected void Start(){
        InitializePlayer();
        InitializeStat();
        InitializeCallbacks();
    }
    /// <summary>
    /// Update()里放Play相关的特效，如脚步尘土等持续播放的
    /// </summary>
    protected void Update(){
        
    }
    protected void InitializePlayer() => m_player = GetComponent<Player>();
    protected void InitializeStat() => stat = GetComponent<PlayerParticleStatManager>();

    protected virtual void InitializeCallbacks(){
        //var effectObj = stat.current.effectObjectData;
        m_player.playerEvents.OnComboA.AddListener(() =>CreateGO(stat.current.slashDatas[0]) );
        m_player.playerEvents.OnComboB.AddListener(() =>CreateGO(stat.current.slashDatas[1]) );
        m_player.playerEvents.OnComboC.AddListener(() =>CreateGO(stat.current.slashDatas[2]) );
    }
    
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
    /// 协程生成刀光等技能特效对象物体
    /// </summary>
    protected virtual void CreateGO(SkillEffectObject effectObject){
        Debug.Log("创建刀光");
        //如果有刀光go和prefab，就延迟生成
        if (effectObject != null && effectObject.prefab != null) {
            //var routine = InitializeGO(effectObject);
            StartCoroutine(InitializeGO(effectObject));//再启动协程
        }
    }

    /// <summary>
    /// 实例化刀光、协程延迟
    /// </summary>
    protected IEnumerator InitializeGO(SkillEffectObject effectObject){
        //先挂起、暂停协程
        yield return new WaitForSeconds(effectObject.delayedTime);
        //实例化go
        GameObject swordSlash = Instantiate(effectObject.prefab, null);//父物体直接为空
        //给技能各项变量赋值，local rot/pos/scale
        swordSlash.transform.position = m_player.transform.position + effectObject.position;
        swordSlash.transform.eulerAngles = m_player.transform.eulerAngles + effectObject.rotation;
        swordSlash.transform.localScale = effectObject.prefab.transform.localScale * effectObject.scale;
    }
    
    /// <summary>
    /// 根据anim param参数判断n段攻击，播放粒子特效，难以控制位置缩放等故舍弃
    /// </summary>
    // protected virtual void HandleSwordSlash(){
    //     Vector3 position = Vector3.zero;
    //     Vector3 height = new Vector3(0, 0.5f, 0);
    //     //position = m_player.transform.position + 0.5f * m_player.transform.forward + height;
    //     if (m_player.comboCount == 1) {
    //         //Play();
    //         Debug.Log("播放攻击一段特效");
    //     }
    //
    //     if (m_player.comboCount == 2) {
    //         Play(prefabSlashB,prefabSlashB.transform.position);
    //         Debug.Log("播放攻击二段特效");
    //     }
    //     
    //     if (m_player.comboCount == 3) {
    //         Play(prefabSlashC,prefabSlashC.transform.position);
    //         Debug.Log("播放攻击三段特效");
    //     }
    // }
}
