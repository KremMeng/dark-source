using UnityEngine;
    public  class PlayerComboA : AttackPlayerState {
        protected override void OnEnter(Player player){
            // 进入a段攻击，连击次数+1
            player.comboCount++;
        }

        protected override void OnStep(Player player){
            passingTime += Time.deltaTime;
            attackOnPressed = player.inputs.AttackOnPressed();
            canNextCombo = passingTime <= bufferDurationAfter && passingTime > bufferDurationBefore;
            Debug.Log("是否按下按键？"+attackOnPressed+"，是否可进行下一次攻击？"+canNextCombo);
            // 有了一次攻击以后，检查前后摇窗口，满足窗口范围就进行下一次攻击
            if (attackOnPressed && canNextCombo) {
                    player.states.Change<PlayerComboB>();
            }
            // 超过窗口时间没有攻击，就回到idle状态、清零攻击次数
            else if(passingTime > bufferDurationAfter){
                player.states.Change<IdlePlayerState>();
                player.comboCount = 0;
            }
        }

        protected override void OnExit(Player player){
            //离开a段攻击，重置passingTime
            passingTime = 0;
        }

        public override void OnContact(Player entity, Collider other){
            
        }
    }
