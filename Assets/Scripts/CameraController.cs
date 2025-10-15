using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    //public JoystickInput playerInput;
    private IUserInput playerInput;
    
    private GameObject playerHandle;
    private GameObject cameraHandle;
    private GameObject _model;
    private GameObject _camera;

    public float horizontalSpeed = 100.0f;     
    public float verticalSpeed = 80.0f;
    private Vector3 cameraDampVelocity;
    public float cameraOffset = 0.05f;
    
    [FormerlySerializedAs("eulerTemp")] public float eulerPitch = 20.0f;
    
    // Start is called before the first frame update
    void Awake()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        //_model = playerHandle.GetComponent<ActorController>().model;
        //playerInput = playerHandle.GetComponent<ActorController>().playerInput; 违反封装
        ActorController ac = playerHandle.GetComponent<ActorController>(); //引用，局部变量
        _model = ac.model;
        playerInput = ac.playerInput;
        if (Camera.main != null) _camera = Camera.main.gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //视角旋转
        Vector3 eulerTemp = _model.transform.eulerAngles;
        //水平视角
        playerHandle.transform.Rotate(Vector3.up, playerInput.lookRight * horizontalSpeed * Time.fixedDeltaTime);
        //竖直视角
        eulerPitch = eulerPitch - playerInput.lookUp * verticalSpeed * Time.fixedDeltaTime;
        eulerPitch = Mathf.Clamp(eulerPitch, -40, 30);
        cameraHandle.transform.localEulerAngles = new Vector3(eulerPitch, 0, 0);
        _model.transform.eulerAngles = eulerTemp;
        //延迟相机
        _camera.transform.eulerAngles = transform.eulerAngles;
        _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, transform.position, ref cameraDampVelocity,cameraOffset);
    }
}
