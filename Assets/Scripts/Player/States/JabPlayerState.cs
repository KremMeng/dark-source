using UnityEngine;
public class JabPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        player.IsRollFreeze();
        
    }

    protected override void OnExit(Player player){
        player.velocity = Vector3.zero;
    } 

    protected override void OnStep(Player player){
        player.horizontalVelocity = player.transform.forward * -0.5f;
        //播放完切idle
        player.ChangeOnAnimFinish(() => player.states.Change<IdlePlayerState>());
        
    }

    public override void OnContact(Player player, Collider other){
        
    } 
}
