/// <summary>
/// 状态生命周期
/// </summary>
public abstract class IState {
    public abstract void OnEnter();

    public abstract void OnStep();
    
    public abstract void OnExit();
}
