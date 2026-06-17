using System;
using UnityEngine;


// Camera Controller without Cinemachine
public class PlayerCameraController : MonoBehaviour {
    // --- 组件引用 ---
    public Player player;
    protected Camera mainCamera;
    protected Transform cameraPos;
    
    // --- 相机参数 ---
    [Header("Camera Parameters")]
    public float cameraDistance; //臂长，用于缩放、拉远视角
    public float cameraHeight; //相机高度
    Vector3 orbitPivot; //旋转轴心
    protected float cameraYaw; //绕y轴左右旋转角
    protected float cameraPitch; //绕x轴上下旋转角
    public float mouseMulti = 5.0f;
    
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
        orbitPivot = player.transform.position + new Vector3(0,cameraHeight, 0);
        cameraPos = mainCamera.transform.parent.transform;
        cameraYaw = player.transform.eulerAngles.y;
        cameraPitch = cameraPos.transform.eulerAngles.x;
    }

    /// <summary>
    /// 控制相机视角
    /// </summary>
    protected virtual void CameraViewRotation(){
        Vector3 lookDir = player.inputs.GetLookDirection();
        bool isMouseLook = player.inputs.IsLookingInMouse();
        float mouseMulti = isMouseLook ? Time.timeScale : Time.deltaTime * 200;
        if (lookDir.sqrMagnitude > 0) {
            cameraYaw += lookDir.x * horizonSpeedMulti * mouseMulti;
            cameraPitch = cameraPitch -
                          lookDir.z * verticalSpeedMulti * mouseMulti;
            cameraPitch = VerticalClamp(cameraPitch, minPitchAngle, maxPitchAngle);
        }
        // 更新旋转角
        cameraPos.rotation = Quaternion.Euler(0, cameraYaw, 0);
        transform.localEulerAngles = new Vector3(cameraPitch, 0, 0);
        // 更新相机位置
        orbitPivot = player.transform.position + new Vector3(0,cameraHeight, 0);
        cameraPos.position = orbitPivot + cameraDistance * -cameraPos.transform.forward;
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
    /// 延迟相机跟随（非子供向不要太夸张
    /// </summary>
    protected virtual void CameraLag(){
        
    }
}
