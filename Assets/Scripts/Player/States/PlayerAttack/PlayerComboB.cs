using UnityEngine;
public  abstract class PlayerComboB : AttackPlayerState {
    protected override void OnEnter(Player player){
        // 进入b段攻击，连击次数+1
        player.comboCount++;
    }
    protected override void OnStep(Player player){
        // 有了上一次攻击以后，检查后摇窗口，满足窗口范围就进行下一次攻击
        passingTime += Time.deltaTime;
        attackOnPressed = player.inputs.AttackOnPressed();
        if (attackOnPressed && canNextCombo) {
            player.states.Change<PlayerComboC>();
        }
        // 超过窗口时间没有攻击，就回到idle状态、清零攻击次数
        else {
            player.states.Change<IdlePlayerState>();
            player.comboCount = 0;
        }
    }
    protected override void OnExit(Player player){
        passingTime = 0f;
    }
    public override void OnContact(Player entity, Collider other){
            
    }
}