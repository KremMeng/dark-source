using UnityEngine;
    public class PlayerComboC : AttackPlayerState {
        protected override void OnEnter(Player player){
            player.IsFrozeVelocity(true);
            player.InputEnabled = false;
            // 进入攻击c段，连击次数+1
            player.comboCount++;
            // 播放刀光粒子特效
            if (player.particles != null && player.particles.swordSlash != null) {
                player.particles.Play(player.particles.swordSlash);
            }
            player.OnAttackCFinish(() => {
                player.states.Change<IdlePlayerState>();
            });
        }
       
        protected override void OnStep(Player player){
            
        }

        protected override void OnExit(Player player){
            // 退出攻击c段，重置连击次数
            player.comboCount = 0;
            player.IsFrozeVelocity(false);
            player.InputEnabled = true;
        }

        public override void OnContact(Player entity, Collider other){
            
        }
    }
