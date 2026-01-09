using UnityEngine;
public class JumpPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        player.verticalVelocity = new Vector3(0, 1.0f, 0);
        //播放完切roll
        player.ChangeOnAnimFinish(() =>  player.states.Change<RollPlayerState>());
    }

    protected override void OnExit(Player player){
        
    } 

    protected override void OnStep(Player player){
        // if (player.verticalVelocity.y <= 0) {
        //     player.states.Change<IdlePlayerState>();
        // }
    }

    public override void OnContact(Player player, Collider other){
        
    } 
}
