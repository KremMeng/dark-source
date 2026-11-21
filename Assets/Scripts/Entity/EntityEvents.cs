using System;
using UnityEngine.Events;
/// <summary>
/// Entity基类的事件，包括地面状态等，invoke发出通知
/// </summary>
[Serializable]
public class EntityEvents {

    public UnityEvent OnGroundEnter;
    public UnityEvent OnGroundExit;

    public UnityEvent OnRailEnter;
    public UnityEvent OnRailExit;
}
