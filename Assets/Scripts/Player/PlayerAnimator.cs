using System;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Player))]
[AddComponentMenu("Player/Player Animator")]
public class PlayerAnimator : MonoBehaviour {
    
    //强制转换类，每个对象包含转换三要素：from哪个state、要去哪个state、在那一层
    [System.Serializable]
    public class ForcedTransition {
        
        [Tooltip("目标动画的animator层index，Baselayer是0，依次递增")]
        public int animLayer;
        [Tooltip("角色状态机中，‘fromStateId’的状态动画播放完时，强制切换到另外的动画")]
        public int fromStateId;
        [Tooltip("要强制转换的动画名")]
        public string toAnimState;
    }
    
    protected Player player;
    public Animator anim; //=> GetComponent<Animator>();
    
    //Animatir的参数hash大全
    protected int m_curStateHash;
    protected int m_lastStateHash;
    protected int m_horizonSpeedHash;
    protected int m_verticalSpeedHash;
    protected int m_horizonAnimSpeedHash;
    protected int m_healthHash;
    protected int m_jumpCounterHash;
    protected int m_isGroundedHash;
    protected int m_onStateChangeHash;
    
    protected Dictionary<int, ForcedTransition> m_forcedTransitions; //字典，存放状态列表的index和transition
                                                                     //from状态和to状态都在列表里，初始化时放入统称from，用的时候再转到to？
    [Header("Param Names")] //方便在编辑器界面修改
    public string curStateName = "Current State";   //当前状态
    public string lastStateName = "Last State";     //上一个状态
    public string horizonSpeedName = "Horizontal Speed";    //水平面速度
    public string verticalSpeedName = "Vertical Speed";     //竖直方向速度
    public string horizonAnimName = "Horizontal Animation Speed";   //水平的播放动画速度
    public string healthName = "Health";    //血量
    public string jumpCounterName = "Jump Counter";     //跳跃计数
    public string isGroundedName = "Is Grounded";       //在地面状态
    public string onStateChangedName = "On State Changed";      //状态切换触发器
   
    [Header("Settings")] 
    public float minHorizonAnimSpeed = 0.5f;
    public List<ForcedTransition> forcedTransitions; //列表，存放强制转换的规则，例如walk2run

    protected void Start(){
        
        InitializePlayer();
        InitializeForcedTransitions();
        InitializeParamsHash();
        InitializeAnimatorTriggers();
    }
    
    /// <summary>
    /// 确保根据物理和输入计算完成后，再去同步Animator参数
    /// </summary>
    protected void LateUpdate(){
        UpdateAnimatorParams();
    }

    protected virtual void InitializePlayer(){
        player = GetComponent<Player>();
        player.states.events.onChange.AddListener(HandleForcedTransitions); //隐式传委托，onChange触发就执行强制转换方法
    }
    
    /// <summary>
    /// 初始化强制转换字典：如果from状态id不重复，那就把转换列表的元素和对应地索引添加加到字典里
    /// </summary>
    protected virtual void InitializeForcedTransitions(){
        m_forcedTransitions = new Dictionary<int, ForcedTransition>();//前面声明过全局变量，这里new实例化
        //遍历forcedTransitions列表，把列表里的元素+index统统加入字典
        foreach (var transition in forcedTransitions) {
            if (!m_forcedTransitions.ContainsKey(transition.fromStateId)) {
                m_forcedTransitions.Add(transition.fromStateId,transition);
            }
        }
    }
    
    /// <summary>
    /// 强制过渡：从上一状态（用lastindex搜到的）转换到目标to状态,用anim.Play()方法播放同一层级的目标动画
    /// </summary>
    protected virtual void HandleForcedTransitions(){
        var lastStateIndex = player.states.lastIndex;
        //如果字典里有上一个状态的索引，获取索引对应的层级、
        if (m_forcedTransitions.ContainsKey(lastStateIndex)) {
            int layer = m_forcedTransitions[lastStateIndex].animLayer;
            anim.Play(m_forcedTransitions[lastStateIndex].toAnimState,layer);
        }
    }
    /// <summary>
    /// 初始化：触发Animator，当状态onChange了就用SetTrigger触发动画事件
    /// </summary>
    protected virtual void InitializeAnimatorTriggers(){
        player.states.events.onChange.AddListener(()=>anim.SetTrigger(m_onStateChangeHash));//Lambda表达式创造匿名的函数对象
    }
    /// <summary>
    /// 把动画状态机参数名从string转化为hash值，单纯为了降低一点消耗
    /// </summary>
    protected virtual void InitializeParamsHash(){
        
        m_curStateHash = Animator.StringToHash(curStateName);
        m_lastStateHash = Animator.StringToHash(lastStateName);
        m_horizonSpeedHash = Animator.StringToHash(horizonSpeedName);
        m_verticalSpeedHash = Animator.StringToHash(verticalSpeedName);
        m_horizonAnimSpeedHash = Animator.StringToHash(horizonSpeedName);
        m_healthHash = Animator.StringToHash(healthName);
        m_jumpCounterHash = Animator.StringToHash(jumpCounterName);
        m_isGroundedHash = Animator.StringToHash(isGroundedName);
        m_onStateChangeHash = Animator.StringToHash(onStateChangedName);
    }
    /// <summary>
    /// 用速度驱动，每帧更新动画机参数
    /// </summary>
    protected virtual void UpdateAnimatorParams(){
        //计算水平竖直速度（标量）、播放动画的速率
        var horizonSpeed = player.horizontalVelocity.magnitude;
        var verticalSpeed = player.verticalVelocity.y;
        //播放速度要有一个最小值兜底,防止滑步
        var horizonAnimSpeed = Mathf.Max(minHorizonAnimSpeed,horizonSpeed/player.stat.current.maxSpeed);

        //设置参数值
        anim.SetInteger(m_jumpCounterHash,player.jumpCounter);
        anim.SetInteger(m_curStateHash,player.states.curIndex);
        anim.SetInteger(m_lastStateHash,player.states.lastIndex);
        anim.SetFloat(m_horizonSpeedHash,horizonSpeed);
        anim.SetFloat(m_verticalSpeedHash,verticalSpeed);
        anim.SetFloat(m_horizonAnimSpeedHash,horizonAnimSpeed);
        anim.SetBool(m_isGroundedHash,player.isGrounded);
        
    }
}
