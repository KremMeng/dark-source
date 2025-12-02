using System;
using UnityEngine;
using Cinemachine;
using UnityEngine.UIElements;

//脚本必须挂在CinemachineVirtualCamera上
[RequireComponent(typeof(CinemachineVirtualCamera))]
[AddComponentMenu("Player/Player Camera")]
public class PlayerCamera : MonoBehaviour {
    
    [Header("Player Camera Settings")] 
    public Player player;   //玩家角色
    public float maxDistance = 3.0f;   //相机与target的最大距离
    public float initialPitchAngle = 20.0f;  //初始的俯仰角
    public float heightOffset = 1.0f;   //相机距离玩家的竖直偏移量

    [Header("Orbit View Settings")] 
    public bool canOrbit = true;
    public float orbitVelocityMulti = 80.0f;

    [Range(0, 90)] public float maxPitchAngle = 30;
    [Range(-90, 0)] public float minPitchAngle = -40;
    
    protected Transform m_target; //相机跟随的目标，target需要和玩家解耦开
    protected string targetName = "Follow Target GameObject";    //临时变量
    
    protected CinemachineVirtualCamera m_camera; //虚拟相机
    protected Cinemachine3rdPersonFollow m_cameraBody; //虚拟相机的第三人称跟随组件
    protected CinemachineBrain m_brain; //主相机同层级的cinemachine大脑
    
    //PlayerCamera类的内部变量
    protected Vector3 m_cameraTargetPos;  //相机跟随的target的理想位置，target需要和玩家解耦开
    protected float m_cameraDistance;   //与玩家的水平距离
    protected float m_cameraTargetYaw;  //相机跟随的目标:绕y轴旋转的角度,左右视角
    protected float m_cameraTargetPitch; //相机跟随的目标:俯仰角，上下视角
    
    /// <summary>
    /// 初始化相机
    /// </summary>
    protected void Start(){
        InitializeComponents();
        InitializeFollower();
        InitializeCamera();
    }

    protected void LateUpdate(){
        HandleOrbit(); //玩家输入
        ApplyTargetPos();
    }

    /// <summary>       
    /// 初始化相机组件信息
    /// </summary>
    protected virtual void InitializeComponents(){
        //如果没有指定玩家角色，就在场景里自动寻找
        if (!player) { player = FindObjectOfType<Player>(); }
        m_camera = GetComponent<CinemachineVirtualCamera>();
        m_cameraBody = m_camera.AddCinemachineComponent<Cinemachine3rdPersonFollow>();
        m_brain = Camera.main.GetComponent<CinemachineBrain>();
    }
    /// <summary>
    /// 初始化指定：相机跟随的目标的go和位置  
    /// </summary>
    protected virtual void InitializeFollower(){
        m_target = new GameObject(targetName).transform; //动态生成空go
        m_target.position = player.transform.position; //先初始化到玩家脚底的位置，防止穿帮
    }
    /// <summary>
    /// 初始化相机的follow和lookat
    /// </summary>
    protected virtual void InitializeCamera(){
        m_camera.Follow = m_target.transform;
        m_camera.LookAt = player.transform; //第三人称看向玩家身后
        InitialCameraPose();    //第一帧会设定PlayerCamera的初始参数默认值，避免刚进游戏像是闪了一下
    }
    /// <summary>
    /// 把镜头拉到初始站位
    /// </summary>
    protected virtual void InitialCameraPose(){ 
       //初始化内部变量到理想目标值
        m_cameraDistance = maxDistance;
        m_cameraTargetPos = player.unsizedPos + new Vector3(0, heightOffset, 0);//初始位置，在玩家正上方一定距离
        m_cameraTargetPitch = initialPitchAngle;
        m_cameraTargetYaw = player.transform.eulerAngles.y;//玩家朝向
        //初始化target位姿到理想目标值
        ApplyTargetPos();
        m_brain.ManualUpdate();//强制刷新相机
    }
    /// <summary>
    /// 应用相机跟随目标的理想位置:位置在玩家正上方，上下旋转角设定值，左右旋转靠玩家朝向
    /// </summary>
    protected virtual void ApplyTargetPos(){
        m_target.position = m_cameraTargetPos;
        m_target.rotation = Quaternion.Euler(m_cameraTargetPitch,m_cameraTargetYaw,0.0f);//unity实际上是绕zxy的顺序，但是接口仍然用xyz
        m_cameraBody.CameraDistance = m_cameraDistance;
    }
    /// <summary>
    /// 根据玩家输入更改相机上下左右朝向（右摇杆或鼠标）
    /// </summary>
    protected virtual void HandleOrbit(){
        if (canOrbit) {
            var lookDir = player.inputs.GetLookDirection();
            if (lookDir.sqrMagnitude > 0) {
                //根据输入设备选择时间因子:鼠标要考虑游戏世界的的快慢，其它设备与帧率无关所以*dt
                var usingMouse = player.inputs.IsLookingInMouse();
                float timeMulti = usingMouse ? Time.timeScale : Time.deltaTime * 200;//手柄比鼠标慢很多
                //获取水平和竖直旋转角
                m_cameraTargetYaw += lookDir.x * timeMulti ;
                m_cameraTargetPitch -= lookDir.z * timeMulti ;   //Vec3的z位置存的是输入的lookUp轴，右手定则绕向和直觉相反
                //限制一下竖直角度值
                m_cameraTargetPitch = AngleClamp(m_cameraTargetPitch,minPitchAngle,maxPitchAngle);
            }
        }
    }
    /// <summary>
    /// 转多圈（abs＞360度）的时候，先对360取模再限制区间，保证 720° ≡ 0°、-90° ≡ 270° 等语义正确
    /// </summary>
    protected virtual float AngleClamp(float angle,float minAngle,float maxAngle){
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, minAngle, maxAngle);

    }
    /// <summary>
    /// 速度驱动环绕
    /// </summary>
    protected virtual void HandleVelocityOrbit(){
        
    }
    /// <summary>
    /// 高度跟随
    /// </summary>
    protected virtual void HandleOffset(){
        
    }
}
