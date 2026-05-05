using UnityEngine;
public class JumpPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        player.verticalVelocity = new Vector3(0, 4.0f, 0);
        player.IsFrozeVelocity(true);
        player.InputEnabled = false;
    }

    protected override void OnExit(Player player){
        player.IsFrozeVelocity(false);
    } 

    protected override void OnStep(Player player){
            //播放完切roll
            player.ChangeOnAnimFinish(() =>  player.states.Change<RollPlayerState>());
    }

    public override void OnContact(Player player, Collider other){
        
    } 
}
