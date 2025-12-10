using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.VersionControl;
using UnityEngine;
[AddComponentMenu("Asset/Player/States/Break Player State")]
public class BreakPlayerState : PlayerState{
    protected override void OnEnter(Player player){
    }

    protected override void OnExit(Player player){
    }

    protected override void OnStep(Player player){
        player.Decelerate();
        if (player.horizontalVelocity.sqrMagnitude == 0) {//如果完全停住
            player.states.Change<IdlePlayerState>();
        }
    }

    public override void OnContact(Player player, Collider other){
        
    }
}
