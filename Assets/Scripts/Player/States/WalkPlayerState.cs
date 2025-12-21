using UnityEngine;

public class WalkPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        
    }

    protected override void OnExit(Player player){
        
    }

    protected override void OnStep(Player player){
        player.Friction();
        //用cc的话需要接入手写的重力
        player.Jump();
        player.Gravity();
        //player.inputs.JumpOnPressed();
        
        //检测相机空间下的玩家输入
        var inputDirection = player.inputs.GetMovementCameraDirction();
        if (inputDirection.sqrMagnitude > 0) {
            //给走路赋予速度
            //判断当前水平速度和输入方向的夹角，大于90°是负数
            var dot = Vector3.Dot(inputDirection, player.horizontalVelocity);
            //超出刹车阈值时
            if (dot >= player.stat.current.brakeThreshold) {
                //改变速度、位置、朝向才能正确走起来
                player.Accelerate(inputDirection);  //速度
                player.FaceDirectionSmooth(player.horizontalVelocity);  //velocity参数是Vec3向量，自带方向信息
            }
            // else {//低于刹车阈值就进入刹车状态
            //     player.states.Change<BreakPlayerState>();
            // }

        }
        else {
            //没有输入，根据摩擦力减速
            player.Friction();
            //减速到零，切换到Idle
            if (player.horizontalVelocity.sqrMagnitude <= 0) {
                player.states.Change<IdlePlayerState>();
            }
        }
    }

    public override void OnContact(Player player, Collider other){
        
    }
}
