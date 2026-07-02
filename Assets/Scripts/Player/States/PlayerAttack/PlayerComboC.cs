using UnityEngine;
    public  abstract class PlayerComboC : AttackPlayerState {
        protected override void OnEnter(Player player){
            // 进入攻击c段，连击次数+1
            player.comboCount++;
            // 播放完C段动画后一定会切换回idle状态,不可打断
            player.OnAttackCFinish(() => { player.states.Change<IdlePlayerState>(); });

        }
       
        protected override void OnStep(Player player){
            
        }
         protected override void OnExit(Player player){
             // 退出攻击c段，重置连击次数
             player.comboCount = 0;
         }
        public override void OnContact(Player entity, Collider other){
            
        }
    }
