using System;
using UnityEngine;


// Camera Controller without Cinemachine
public class PlayerCameraController : MonoBehaviour {
    // --- 组件引用 ---
    public Player player;
    protected Camera mainCamera;
    [SerializeField] private Transform cameraTarget; //缓存：界面拖节点，避免频繁查找
    
    // --- 相机参数 ---
    [Header("Camera Parameters")]
    public float cameraDistance; //臂长，用于缩放、拉远视角
    public float cameraHeight; //相机高度
    Vector3 orbitPivot; //旋转轴心
    protected float cameraYawAngle; //绕y轴左右旋转角
    protected float cameraPitchAngle; //绕x轴上下旋转角
    
    public float mouseMulti = 5.0f;
    public float cameraOffsetTime = 0.1f; //当前值reach目标值所需时间
    private Vector3 cameraDampVelocity; //摄像机lag的插值速度
    
    public float horizonCamSpeed;
    public float verticalCamSpeed;
    public float horizonSpeedMulti = 1.0f;
    public float verticalSpeedMulti = 1.0f;
    
    [Header("Up-Down View Limits")]
    public float minPitchAngle = -20f;
    public float maxPitchAngle = 80f;
    

    protected void Awake(){
        InitializeCameraParams();
    }
    
    protected virtual void LateUpdate(){
        CameraViewRotation();
        CameraLag();
    }

    protected virtual void InitializeCameraParams(){
        player = FindObjectOfType<Player>();
        mainCamera = Camera.main;
        //cameraTarget = player.transform.Find("CameraTarget"); //已经序列化拖拽了，但是为了如果后续有类似切换角色的功能依旧需要手动初始化
        cameraYawAngle = player.transform.eulerAngles.y;
        cameraPitchAngle = cameraTarget.transform.eulerAngles.x;
    }

    /// <summary>
    /// 控制相机视角
    /// </summary>
    protected virtual void CameraViewRotation(){
        Vector3 lookDir = player.inputs.GetLookDirection();
        bool isMouseLook = player.inputs.IsLookingInMouse();
        mouseMulti = isMouseLook ? Time.timeScale : Time.deltaTime * 50;
        if (lookDir.sqrMagnitude > 0) {
            cameraYawAngle += lookDir.x * horizonSpeedMulti * mouseMulti;
            cameraPitchAngle -= lookDir.z * verticalSpeedMulti * mouseMulti;
            cameraPitchAngle = VerticalClamp(cameraPitchAngle, minPitchAngle, maxPitchAngle); 
        }
        // 更新旋转角
         cameraTarget.transform.rotation = Quaternion.Euler(cameraPitchAngle, cameraYawAngle, 0);
        
        // 更新相机位置
         orbitPivot = player.transform.position + new Vector3(0,cameraHeight, 0);
         cameraTarget.position = orbitPivot + cameraDistance * -cameraTarget.transform.forward;
    }

    /// <summary>
    /// 把竖直角度钳在±360内
    /// </summary>
    protected virtual float VerticalClamp(float pitch, float minPitch, float maxPitch){
        if(pitch > 360f) pitch -= 360f;
        if(pitch < -360f) pitch += 360f;
        return Mathf.Clamp(pitch, minPitch, maxPitch);
    }
    
    /// <summary>
    /// 缩放视角
    /// </summary>
    protected virtual void CameraZoom(){ 
        Vector3 moveDir = player.inputs.GetMovementDirction();
        Vector3 lookDir = player.inputs.GetLookDirection();
        if (moveDir.sqrMagnitude > 0 && lookDir.sqrMagnitude > 0) {
            
        }
    }
    /// <summary>
    /// 延迟相机：对相机和target的距离做插值过渡
    /// </summary>
    protected virtual void CameraLag(){
        mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, cameraTarget.position,
            ref cameraDampVelocity, cameraOffsetTime);
        mainCamera.transform.LookAt(orbitPivot);
    }
}
