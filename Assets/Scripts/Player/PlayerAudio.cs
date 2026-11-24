using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Player))]
public class PlayerAudio : MonoBehaviour {
    [Header("Combo Attack")] 
    public AudioClip comboA;
    public AudioClip comboB;
    public AudioClip comboC;
    [Header("Delayed time")]
    [SerializeField] protected float time = 0.15f;
    [SerializeField] protected float time1 = 2f;

    protected Player m_player;
    protected AudioSource m_audio;//Audio Data
    //protected float timer;//time+=Time.dt与reset

    protected void Start(){
        InitializePlayer();
        InitializeAudioData();
        InitializeCallbacks();
    }

    protected virtual void InitializePlayer() => m_player = GetComponent<Player>();
    
    protected virtual void InitializeAudioData(){
        if (!TryGetComponent(out m_audio)) {//避免Component不存在时在Editor中额外分配内存的情况
            m_audio = gameObject.AddComponent<AudioSource>();
        }
    }
    /// <summary>
    /// 初始化订阅事件：一次订阅一直都有效，触发时事件系统会主动调用
    /// 放在update()里反而会每帧重复订阅
    /// </summary>
    protected virtual void InitializeCallbacks(){
        //float time = m_player.particleStat.current.effectObjectData.delayedTime;
        m_player.playerEvents.OnComboA.AddListener(() =>Play(comboA,time) );
        m_player.playerEvents.OnComboB.AddListener(() =>Play(comboB,time) );
        m_player.playerEvents.OnComboC.AddListener(() =>Play(comboC,time1) );
    }
    /// <summary>
    /// 延迟音效播放
    /// </summary>
    protected virtual void Play(AudioClip audioClip,float delayedTime,bool stopLastAudio=true){
        if (audioClip == null) return;
        if(stopLastAudio) m_audio.Stop(); //播新音效前，先把上一个停掉
        m_audio.clip = audioClip;
        m_audio.PlayDelayed(delayedTime);//以秒
    }
    /// <summary>
    /// 即时音效播放,每次调用播放一次，连续播放会叠加
    /// </summary>
    /// <param name="audioClip"></param>
    /// <param name="stopLastAudio"></param>
    protected virtual void Play(AudioClip audioClip,bool stopLastAudio=true){
        if (audioClip == null) return;
        if(stopLastAudio) m_audio.Stop(); //播新音效前，先把上一个停掉
        m_audio.PlayOneShot(audioClip);
    }
    /// <summary>
    /// 随机播放几种音效中的一种，如攻击、跳跃角色的声音需要不同
    /// </summary>
    protected virtual void PlayRandom(AudioClip[] clips){
        if (clips != null && clips.Length > 0) {
            int index = Random.Range(0,clips.Length);
            var clip = clips[index];
            if (clip) {
                Play(clip);
            }
        }
    }
}
