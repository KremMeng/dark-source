
using UnityEngine.Events;
[System.Serializable] //让自定义类/结构体能显示在编辑器面板上
public class PlayerStateManagerEvents : EntityStateManagerEvents {

    public UnityEvent OnJump;
    public UnityEvent OnJab;
    public UnityEvent OnRoll;
    public UnityEvent OnAttack;
}
