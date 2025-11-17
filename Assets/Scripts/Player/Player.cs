
public class Player : Entity<Player> {
    //玩家的inputmanager实例
    public PlayerInputManager inputs { get; protected set; }

    protected override void Awake(){
        base.Awake();//先让父类初始化
        InitializeInputs();
    }

    protected virtual void InitializeInputs() => inputs = GetComponent<PlayerInputManager>();
}
