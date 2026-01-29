using UnityEngine;
public class RollPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        player.IsRollFreeze();
    }

    protected override void OnExit(Player player){
        player.IsNotFreeze();
    } 

    protected override void OnStep(Player player){
        //进入状态时给一个向上的冲量即可
        player.horizontalVelocity = player.transform.forward * 1.2f;
        //播放完切idle
        player.ChangeOnAnimFinish(() => player.states.Change<WalkPlayerState>());
    }

    public override void OnContact(Player player, Collider other){
        
    } 
}
