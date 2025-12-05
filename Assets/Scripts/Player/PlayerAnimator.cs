using UnityEngine;
[RequireComponent(typeof(Player))]
[AddComponentMenu("Player/Player Animator")]
public class PlayerAnimator : MonoBehaviour {

    [System.Serializable]
    public class ForcedTransition {
        [Tooltip("角色状态机中，‘fromStateId’的状态动画播放完时，强制切换到另外的动画")]
        public int fromStateId;
        [Tooltip("目标动画的animator层index，Baselayer是0")]
        public int animLayer;
        [Tooltip("要强制转换的动画名")]
        public string toAnimState;
    }
}
