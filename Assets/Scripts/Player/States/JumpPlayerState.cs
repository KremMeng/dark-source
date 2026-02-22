using UnityEngine;
public class JumpPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        player.IsRollFreeze();
    }

    protected override void OnExit(Player player){
        player.IsNotFreeze();
    } 

    protected override void OnStep(Player player){
            player.verticalVelocity = new Vector3(0, 4.0f, 0);
            //播放完切roll
            player.ChangeOnAnimFinish(() =>  player.states.Change<RollPlayerState>());
    }

    public override void OnContact(Player player, Collider other){
        
    } 
}
