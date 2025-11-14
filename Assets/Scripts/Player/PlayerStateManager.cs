using UnityEngine;
[RequireComponent(typeof(Player))] //所在物体必须有Player组件
public class PlayerStateManager : EntityStateManager<Player> {
    //使用CLassTypeName工具
    [ClassTypeName(typeof(PlayerState))] public string[] states;
}
