using UnityEngine;

public class WalkPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        
    }

    protected override void OnExit(Player player){
        
    }

    protected override void OnStep(Player player){
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
            }

        }
    }

    public override void OnContact(Player player, Collider other){
        
    }
}
