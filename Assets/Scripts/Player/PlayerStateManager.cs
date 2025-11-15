using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Player))] //所在物体必须有Player组件
public class PlayerStateManager : EntityStateManager<Player> {
    //使用CLassTypeName工具获取:玩家Player的所有状态名字的字符串
    [ClassTypeName(typeof(PlayerState))] public string[] states;
    //接口，转到PlayerState类动态实例化
    protected override List<EntityState<Player>> GetStateList(){
        return PlayerState.CreateListFromStatesArray(states);
    }
}
