using UnityEngine;
public class FallPlayerState : PlayerState{
    
    protected override void OnEnter(Player player){
        
    }

    protected override void OnExit(Player player){
        
    } 

    protected override void OnStep(Player player){
        player.Gravity();
        player.FaceDirectionSmooth(player.horizontalVelocity);
        // player.Jump();
   
        if (player.inputs.JumpOnPressed()) {
            player.Jump();
        }
        // if (player.verticalVelocity == Vector3.zero) {
        //     player.states.Change<IdlePlayerState>();
        // }
        if (player.isGrounded) {
            player.states.Change<IdlePlayerState>();
        }
    }

    public override void OnContact(Player player, Collider other){
        
    } 
}
