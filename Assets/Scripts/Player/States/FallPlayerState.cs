using UnityEngine;
public class FallPlayerState : PlayerState{
    
    protected override void OnEnter(Player player){
        player.InputEnabled = false;
        player.IsFrozeVelocity(true);
    }

    protected override void OnExit(Player player){
     
    } 

    protected override void OnStep(Player player){
        player.Gravity();
        player.FaceDirectionSmooth(player.horizontalVelocity);
        player.AccelerateWithInputDir();
        
        // 下落过程中检测到落地，且竖直速度稍大
        if (player.isGrounded && player.verticalVelocity.sqrMagnitude > 0.25) {
            player.verticalVelocity = Vector3.zero;
            player.states.Change<RollPlayerState>();
        }
        else if(player.isGrounded && player.verticalVelocity.sqrMagnitude < 0.25){
            player.states.Change<IdlePlayerState>();
        }
    }

    public override void OnContact(Player player, Collider other){
        
    } 
}
