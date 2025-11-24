using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour {
    public InputActionAsset actions; //按键映射的配置文件，inspector里拖入
    protected InputAction m_movement;
    protected Camera m_camera;
    protected float m_movementDirctionUnlockTime;//小于这个时长，锁定玩家的移动输入

    protected virtual void Awake(){
        CacheActions();
        InitializeCamera();
    }

    protected void Start(){
        actions.Enable();//新输入系统的安全设置:需要先手动激活
    }

    protected virtual void OnEnable() => actions?.Enable();
    protected virtual void OnDisable() => actions?.Disable();

    //把按键映射的配置按字符串放到缓存变量里，避免每帧都字典查找--Dictionary<string, InputAction>
    protected virtual void CacheActions(){
        m_movement = actions["move"];
    }

    protected virtual void InitializeCamera() => m_camera = Camera.main;

    /// <summary>
    /// xz轴的平面移动输入
    /// </summary>
    public virtual Vector3 GetMovementDirction(){
        if(Time.time < m_movementDirctionUnlockTime) return Vector3.zero;
        var inputAxises = m_movement.ReadValue<Vector2>();
        return GetAxisWithCrossDeadZone(inputAxises);
    }
    /// <summary>
    /// 考虑死区的水平面轴
    /// </summary>
    /// <param name="axises">输入轴</param>
    public virtual Vector3 GetAxisWithCrossDeadZone(Vector2 axises){
        
        var deadzone = InputSystem.settings.defaultDeadzoneMin;
        axises.x = Mathf.Abs(axises.x) > deadzone ? RemapToDeadZone(axises.x, deadzone) : 0;
        axises.y = Mathf.Abs(axises.y) > deadzone ? RemapToDeadZone(axises.y, deadzone) : 0;
        Vector3 axisesWithDeadZone = new Vector3(axises.x, 0, axises.y);
        return axisesWithDeadZone; 
    }
    /// <summary>
    /// 把输入区间映射到0-1
    /// </summary>
    protected float RemapToDeadZone(float value, float deadzone) =>
        (value - (value > 0 ? -deadzone : deadzone)) / (1 - deadzone);//输入值可能为负，同样要map

    public virtual Vector3 GetMovementCameraDirction(){
        //获取原始输入方向
        var direction = GetMovementDirction();
        //把输入转到摄像机水平朝向，并不是完整相机空间
        float yaw = m_camera.transform.eulerAngles.y;//角色默认朝向+z，yaw是在此基础上的旋转角
        var rotation = Quaternion.AngleAxis(yaw,Vector3.up);
        direction = rotation * direction;//Unity内部运算符重载好了四元数运算
        return direction;
    }
}
