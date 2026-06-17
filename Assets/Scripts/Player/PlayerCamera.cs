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
    
    [Header("Following Settings")] 
    public float upDeadZone = 0.05f;
    public float downDeadZone = 0.05f; //上下跟随有“懒得跟”的死区，死区范围内不lag
    public float airUpDeadZone = 4f;
    public float airDownDeadZone = 0;
    public float maxFollowSpeed = 10f;
    public float maxAirFollowSpeed = 100f;

    [Header("Orbit View Settings")] 
    public bool canOrbit = true;
    public bool canOrbitByHeading  = true; //能否根据角色朝向更改相机look
    public float orbitVelocityMulti = 5.0f;

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
        
        HandleOrbit(); //玩家右摇杆输入
        HandleStrafeFacing();//跟着角色朝向
        HandleLagFollow(); //上下方向跟随/延迟
        ApplyTargetPos(); //放到最后，让其它计算先落地，最后一次性写入transform
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
    /// 延迟,区分地面和空中两种情况，空中的响应会略微缓慢
    /// </summary>
    protected virtual void HandleLagFollow(){
        
        //初始化相机目标点在当前帧内的高度，用上一帧的target位置作为基础计算
        var curTargetPos = player.unsizedPos + new Vector3(0, heightOffset, 0);
        var lastTargetPos = m_cameraTargetPos; //相机上一帧的目标位置
        var camHeightPos = lastTargetPos.y;
        
        //地面
        if (player.isGrounded || IsFollowState()) {
            //如果玩家跳跃\下落等超过死区上限,相机向对应方向缓慢位移offset距离
            if (curTargetPos.y > lastTargetPos.y + upDeadZone) {
                var offset = curTargetPos.y - upDeadZone - lastTargetPos.y;
                //防止一帧内跳太多，突兀了，有个每帧位移的最大值
                camHeightPos += Mathf.Min(offset, maxFollowSpeed * Time.deltaTime);
            }else if (curTargetPos.y < lastTargetPos.y - downDeadZone) {
                var offset = curTargetPos.y  - lastTargetPos.y + downDeadZone;//此时offset是负数
                camHeightPos -= Mathf.Max(offset, maxFollowSpeed * Time.deltaTime);
            }
        }
        
        //空中
        else if (curTargetPos.y > lastTargetPos.y + airUpDeadZone) {
            var offset = curTargetPos.y - airUpDeadZone - lastTargetPos.y;
            //防止一帧内跳太多，突兀了，有个每帧位移的最大值
            camHeightPos += Mathf.Min(offset, maxFollowSpeed * Time.deltaTime);
        }else if (curTargetPos.y < lastTargetPos.y - airDownDeadZone) {
            var offset = curTargetPos.y  - lastTargetPos.y + airDownDeadZone;
            camHeightPos -= Mathf.Max(offset, maxFollowSpeed * Time.deltaTime);
        }

        //计算最终位置：除了高度信息都和target相同
        m_cameraTargetPos = new Vector3(curTargetPos.x,camHeightPos, curTargetPos.z);
    }
    /// <summary>
    /// 在玩家右摇杆输入的基础上，根据玩家速度朝向改变相机朝向
    /// </summary>
    protected virtual void HandleStrafeFacing(){
        if (canOrbitByHeading && player.isGrounded) {
            //获取玩家水平速度下、相机空间的速率
            var localVelocity = m_target.InverseTransformDirection(player.horizontalVelocity);
            m_cameraTargetYaw += localVelocity.x * orbitVelocityMulti * Time.deltaTime;
        }
    }
    /// <summary>
    /// 判断某状态是不是需要在高度上跟随
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsFollowState(){

        return true;
    }
}
