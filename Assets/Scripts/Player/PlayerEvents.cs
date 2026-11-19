
using UnityEngine.Events;
[System.Serializable] //让自定义类/结构体能显示在编辑器面板上
public class PlayerEvents : EntityEvents {

    public UnityEvent OnJump;
    public UnityEvent OnJab;
    public UnityEvent OnRoll;
    
    public UnityEvent OnComboA;
    public UnityEvent OnComboB;
    public UnityEvent OnComboC;
}
