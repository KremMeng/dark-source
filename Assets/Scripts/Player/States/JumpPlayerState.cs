using UnityEngine;
public class JumpPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        
    }

    protected override void OnExit(Player player){
        
    } 

    protected override void OnStep(Player player){
        player.verticalVelocity = new Vector3(0, player.height, 0);
        if (player.verticalVelocity.y <= 0) {
            player.states.Change<IdlePlayerState>();
        }
    }

    public override void OnContact(Player player, Collider other){
        
    } 
}
