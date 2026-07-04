using UnityEngine;
    public class AttackPlayerState : PlayerState {
        protected float passingTime; //上次攻击后流逝的时间，每个子攻击状态独立计算
        
        public float bufferDurationBefore = 0.5f; //前摇间隔
        public float bufferDurationAfter = 1.0f; //后摇间隔

        protected bool attackOnPressed; //检查攻击键按下

        private bool hasHitted; //是否已进入过攻击状态
        protected bool canNextCombo; //是否可进行下一次攻击
        
        protected override void OnEnter(Player player){
            player.comboCount = 0;//初始化为0
            
        }
       
        protected override void OnStep(Player player){
            
        }
        protected override void OnExit(Player player){
            
        }
        public override void OnContact(Player entity, Collider other){
            
        }
    }
