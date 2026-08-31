using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Player))] //所在物体必须有Player组件
public class PlayerStateManager : EntityStateManager<Player> {
    //使用CLassTypeName工具：用于在 Inspector 中自动填充所有继承自 PlayerState 的子类的名称（字符串数组）
    [ClassTypeName(typeof(PlayerState))] public string[] states; 
    //接口，转到PlayerState类动态实例化
    //此时是不知道以后会有哪些state的，用反射的好处就是拓展性好
    protected override List<EntityState<Player>> GetStateList(){
        return PlayerState.CreateStatesListAfterReflection(states);
    }
}
