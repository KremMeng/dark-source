using UnityEditor.VersionControl;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyAnimator : MonoBehaviour{

    public string healthName = "health";
    public string attackName = "attack";
    public string horizonSpeedName = "horizon speed";
    public string verticalSpeedName = "vertical speed";
    public string currentStateName = "current state";
    public string lastStateName = "last state";
    
    //哈希值，省内存
    protected int m_healthHash;
    protected int m_attackHash;
    protected int m_horizonSpeedHash;
    protected int m_verticalSpeedHash;
    protected int m_currentStateHash;
    protected int m_lastStateHash;

    protected Animator anim;
    protected EnemyStateManager states;
    public Enemy enemy;

    protected void Start(){
        InitializeAnim();
        InitializeHashParams();
        InitializeEnemy();
        InitializeStatesManager();
    }

    protected void LateUpdate(){
        UpdateAnimParams();
    }

    protected virtual void InitializeAnim() => anim = GetComponent<Animator>();
    protected virtual void InitializeEnemy() => enemy = GetComponent<Enemy>();
    protected virtual void InitializeStatesManager() => states = GetComponent<EnemyStateManager>();
    
    protected virtual void InitializeHashParams(){
        //状态的last和current index
        //水平竖直速度、然后用waypoint作为输入控制
        m_horizonSpeedHash = Animator.StringToHash(horizonSpeedName);
        m_verticalSpeedHash = Animator.StringToHash(verticalSpeedName);
        m_attackHash = Animator.StringToHash(attackName);
        m_currentStateHash = Animator.StringToHash(currentStateName);
        m_lastStateHash = Animator.StringToHash(lastStateName);
    }

    protected virtual void UpdateAnimParams(){
        var horizonSpeed = enemy.horizontalVelocity.magnitude;
        var verticalSpeed = enemy.verticalVelocity.magnitude;
        anim.SetFloat(m_horizonSpeedHash,horizonSpeed);
        anim.SetFloat(m_verticalSpeedHash,verticalSpeed);
        anim.SetBool(m_attackHash,enemy.isInAttackRange);//?不用trigger-攻击间隔？累积？player攻击打断？
        anim.SetInteger(m_lastStateHash,states.lastIndex);
        anim.SetInteger(m_currentStateHash,states.curIndex);
    }

}
