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
    //实例化特效
    public GameObject InstantiateEffect(GameObject effectPrefab,Vector3 position){
        GameObject effectInstance = Instantiate(effectPrefab, position, Quaternion.identity);
        return effectInstance;
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

    protected virtual void HandleSwordSlash(){
        Vector3 position = Vector3.zero;
        Vector3 height = new Vector3(0, 0.5f, 0);
        position = m_player.transform.position + 0.5f * m_player.transform.forward + height;
        if (m_player.comboCount == 1) {
            Play(swordSlashA,position);
            Debug.Log("播放攻击一段特效");
        }

        if (m_player.comboCount == 2) {
            Play(swordSlashB,position);
            Debug.Log("播放攻击二段特效");
        }
        
        if (m_player.comboCount == 3) {
            Play(swordSlashC,position);
            Debug.Log("播放攻击三段特效");
        }
    }
}
