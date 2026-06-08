using UnityEngine;
public class JabPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        player.IsFrozeVelocity(true);
        player.InputEnabled = false;
    }

    protected override void OnExit(Player player){
        player.IsFrozeVelocity(false);
    } 

    protected override void OnStep(Player player){
        //后撤距离
        player.horizontalVelocity += player.transform.forward * -0.015f;
        //播放完切idle
        player.ChangeOnAnimFinish(() => player.states.Change<IdlePlayerState>());
        
    }

    public override void OnContact(Player player, Collider other){
        
    } 
}
