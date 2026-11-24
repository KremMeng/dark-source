using UnityEngine;
//把具体攻击都抽到player里，方便事件发消息
public  class PlayerComboA : AttackPlayerState {
        protected override void OnEnter(Player player){
            player.playerEvents.OnComboA?.Invoke();//转到comboA，无论如何都是触发一段攻击
            // 进入a段攻击，连击次数+1
            player.comboCount++;
            //如果角色不在idle状态就触发了攻击，那就让速度归零
            if (player.states.lastIndex == 1 || player.states.lastIndex == 2) {
                player.velocity = Vector3.zero;
            }
        }

        protected override void OnStep(Player player){
            player.ComboA();
        }

        protected override void OnExit(Player player){
            //离开a段攻击，重置passingTime
            player.attackPassingTime = 0;
        }

        public override void OnContact(Player entity, Collider other){
            
        }
    }
