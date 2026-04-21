using UnityEngine;
public class RollPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        // 进入Roll状态时，朝移动输入的方向翻滚
        Vector3 rollDir = player.GetRollDirection();                                
        float rollSpeed = Mathf.Max(player.horizontalVelocity.magnitude, player.stat.current.maxSpeed * 3.0f); //保底初速度
        player.horizontalVelocity = rollDir * rollSpeed;
        //player.FaceDirection(rollDir, 720f); // 720度/秒的快速转向翻滚方向
        
        player.IsFrozeVelocity(true);
        player.InputEnabled = false;
    }


    protected override void OnExit(Player player){
        player.IsFrozeVelocity(false);                
        player.InputEnabled = true; // 确保退出时恢复输入
    } 

    protected override void OnStep(Player player){
        //根据上个状态索引，播放完roll的动画转到上个状态
        if (player.states.lastIndex == 1) {
            player.ChangeOnAnimFinish(() => player.states.Change<WalkPlayerState>());
        }else if (player.states.lastIndex == 7) {
            player.ChangeOnAnimFinish(() => player.states.Change<RunPlayerState>());
        }
        
    }

    public override void OnContact(Player player, Collider other){
        
    } 
}
