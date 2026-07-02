using UnityEngine;
    public  class AttackPlayerState : PlayerState {
        protected float passingTime; //上次攻击后流逝的时间，每个子攻击状态独立计算
        
        public float bufferDurationBefore = 0.2f; //前摇间隔
        public float bufferDurationAfter = 0.5f; //后摇间隔

        protected bool attackOnPressed; //检查攻击键按下

        private bool hasHitted; //是否已进入过攻击状态
        protected bool canNextCombo; //是否可进行下一次攻击
        
        protected override void OnEnter(Player player){
        }
       
        protected override void OnStep(Player player){
            canNextCombo = passingTime < bufferDurationBefore;
            attackOnPressed = player.inputs.AttackOnPressed();
            // 没攻击的时候按下攻击键，必然转a段攻击
            if (attackOnPressed) {
                player.states.Change<PlayerComboA>();
            }
        }
        protected override void OnExit(Player player){
            
        }
        public override void OnContact(Player entity, Collider other){
            
        }
    }
