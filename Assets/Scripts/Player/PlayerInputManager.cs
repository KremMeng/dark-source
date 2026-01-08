using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class PlayerInputManager : MonoBehaviour {
    public InputActionAsset actions; //按键映射的配置文件，inspector里拖入
    
    //硬件输入缓存
    protected InputAction m_movement;
    protected InputAction m_look;
    protected InputAction m_jump;
    protected InputAction m_run;

    protected float? m_timeOfLastJump; //可空值类型，记录自程序应用以来，上次跳跃的时间
    protected float jumpBufferTimer = 0.15f; //跳跃缓冲计时器时间内按下第二次跳跃，落地后自动触发
    
    protected Camera m_camera;
    protected float m_movementDirctionUnlockTime;//小于这个时长，锁定玩家的移动输入

    protected virtual void Awake(){
        CacheActions();
        InitializeCamera();
    }

    protected void Start(){
        actions.Enable();//新输入系统的安全设置:需要先手动激活
    }

    protected void Update(){
        //记录按下跳跃的全局时间
        if (m_jump.WasPressedThisFrame()) {
            m_timeOfLastJump = Time.time;
        }
    }

    protected virtual void OnEnable() => actions?.Enable();
    protected virtual void OnDisable() => actions?.Disable();

    //把按键映射的配置按字符串放到缓存变量里，避免每帧都字典查找--Dictionary<string, InputAction>
    protected virtual void CacheActions(){
        m_movement = actions["move"];
        m_look = actions["look"];
        m_jump = actions["jump"];
        m_run = actions["run"];
    }

    protected virtual void InitializeCamera() => m_camera = Camera.main;

    /// <summary>
    /// xz轴的平面移动输入(方向)
    /// </summary>
    /// /// <returns>把 2D 摇杆映射到 3D 世界里的“地面方向“,所以返回Vec3</returns>
    public virtual Vector3 GetMovementDirction(){
        if(Time.time < m_movementDirctionUnlockTime) return Vector3.zero;
        var inputAxises = m_movement.ReadValue<Vector2>();
        return GetAxisWithCrossDeadZone(inputAxises);
    }

    /// <summary>
    /// 相机的输入方向
    /// </summary>
    /// <returns>鼠标正常返回，手柄需要考虑死区</returns>
    public virtual Vector3 GetLookDirection(){
        var cameraLook = m_look.ReadValue<Vector2>();
        if (IsLookingInMouse()) {
            return new Vector3(cameraLook.x, 0, cameraLook.y);//摇杆的right轴和up轴--对应视角世界x和世界z
        }
        else {
            return GetAxisWithCrossDeadZone(cameraLook);
        }
    }
    /// <summary>
    /// 获取当前输入，看是不是鼠标
    /// </summary>
    public virtual bool IsLookingInMouse(){
        //获取当前的控制设备activeControl
        if (m_look.activeControl == null) return false;
        return m_look.activeControl.device.name.Equals("Mouse"); //返回一个bool值
    }
    /// <summary>
    /// 考虑死区的水平面轴
    /// </summary>
    /// <param name="axises">输入轴</param>
    public virtual Vector3 GetAxisWithCrossDeadZone(Vector2 axises){
        
        var deadzone = InputSystem.settings.defaultDeadzoneMin;
        axises.x = Mathf.Abs(axises.x) > deadzone ? RemapToDeadZone(axises.x, deadzone) : 0;
        axises.y = Mathf.Abs(axises.y) > deadzone ? RemapToDeadZone(axises.y, deadzone) : 0;
        return new Vector3(axises.x, 0, axises.y);; 
    }
    /// <summary>
    /// 把输入区间映射到0-1
    /// </summary>
    protected float RemapToDeadZone(float value, float deadzone) =>
        (value - (value > 0 ? -deadzone : deadzone)) / (1 - deadzone);//输入值可能为负，同样要map

    public virtual Vector3 GetMovementCameraDirction(){
        //获取原始输入方向
        var direction = GetMovementDirction();
        if (direction.sqrMagnitude > 0) {
            //把输入转到摄像机水平朝向，并不是完整相机空间
            float yaw = m_camera.transform.eulerAngles.y;//角色默认朝向+z，yaw是在此基础上的旋转角
            var rotation = Quaternion.AngleAxis(yaw,Vector3.up);
            direction = rotation * direction;//Unity内部运算符重载好了四元数运算
            direction = direction.normalized;
        }
        return direction;
    }
    /// <summary>
    /// 是否允许再次跳跃，因为写在了各个基础状态的updtae里所以不能乱跳?
    /// </summary>
    /// <returns></returns>
    public virtual bool CanJump(){
        //上次跳跃时的时间存在 && 在jumpBuffer计时器时间内,允许再次跳跃
        if (m_timeOfLastJump != null && Time.time - m_timeOfLastJump < jumpBufferTimer) {
            m_timeOfLastJump = null;
            return true;
        }
        return false;
    }
    //跳跃相关按键判定
    public virtual bool JumpOnPressed() => m_jump.WasPressedThisFrame();
    public virtual bool JumpIsPresssing() => m_jump.IsPressed();
    public virtual bool JumpOnReleased() => m_jump.WasReleasedThisFrame();

    //跑步相关键位判定
    public virtual bool RunOnPressed() => m_run.WasPressedThisFrame();
    public virtual bool RunIsPressing() => m_run.IsPressed();
    public virtual bool RunOnReleased() => m_run.WasReleasedThisFrame();
}
