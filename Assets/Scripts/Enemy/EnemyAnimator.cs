using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyAnimator : MonoBehaviour{

    public string healthName = "health";
    public string attackName = "attack";
    public string horizonSpeedName = "horizon speed";
    public string verticalSpeedName = "vertical speed";
    
    protected float m_healthHash;
    protected float m_attackHealth;
    protected float m_horizonSpeedHash;
    protected float m_verticalSpeeHash;

    protected void Start(){
        InitializeHashParams();
    }

    protected void LateUpdate(){
        UpdateAnimParams();
    }

    protected virtual void InitializeHashParams(){
        
    }

    protected virtual void UpdateAnimParams(){
        
    }

}
