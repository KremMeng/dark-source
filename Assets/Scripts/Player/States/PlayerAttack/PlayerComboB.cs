using UnityEngine;
public class PlayerComboB : AttackPlayerState {
    protected override void OnEnter(Player player){
        player.playerEvents.OnComboB?.Invoke();
        // 进入b段攻击，连击次数+1
        player.comboCount++;
    }
    protected override void OnStep(Player player){
        player.ComboB();
    }
    protected override void OnExit(Player player){
        player.attackPassingTime = 0f;
        
    }
    public override void OnContact(Player entity, Collider other){
            
    }
}