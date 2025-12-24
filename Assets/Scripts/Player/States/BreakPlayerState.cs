using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.VersionControl;
using UnityEngine;
[AddComponentMenu("Asset/Player/States/Break Player State")]
public class BreakPlayerState : PlayerState{
    protected override void OnEnter(Player player){
        //播放完切idle
        player.ChangeOnAnimFinish(() => player.states.Change<IdlePlayerState>());
    }

    protected override void OnExit(Player player){
        
    }

    protected override void OnStep(Player player){
        player.Gravity();
        player.Decelerate();
      }   

    public override void OnContact(Player player, Collider other){
        
    }
}
