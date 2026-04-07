using UnityEngine;

public class NpcAI : MonoBehaviour {
    public float sight = 5; // npc的感知距离
    public float attack = 1;    // npc的攻击距离
    public float target{get;protected set;}   // 目标玩家和npc的距离
    private StateManager fsm;
    
    // 初始化状态
    private void Awake(){
        // 初始化并添加状态
        IState m_walk = new WalkState(transform);
        IState m_chase = new ChaseState(transform, GameObject.Find("PlayerHandle").transform);
        IState m_attack = new AttackState();
        
        fsm = new StateManager(m_walk);
        
        // 根据npc和目标距离添加转换
        fsm.AddTransition(m_walk, m_chase, () => { return target <= sight && target >= attack;});
        fsm.AddTransition(m_walk,m_attack,(() => { return target <= attack;}));
        
        fsm.AddTransition(m_chase, m_walk, () => { return target > sight;});
        fsm.AddTransition(m_chase,m_attack,(() => { return target <= attack;}));
        
        fsm.AddTransition(m_attack, m_walk, () => { return target > sight;});
        fsm.AddTransition(m_attack, m_chase, () => { return target <= sight && target >= attack;});
    }

    private void Update(){
        target = GameObject.Find("PlayerHandle").transform.position.x - transform.position.x;
        fsm.Tick();
    }
}
