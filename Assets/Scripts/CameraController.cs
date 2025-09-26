using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    public PlayerInput playerInput;
    
    private GameObject playerHandle;
    private GameObject cameraHandle;

    public float horizontalSpeed = 100.0f;
    public float verticalSpeed = 80.0f;
    [FormerlySerializedAs("eulerTemp")] public float eulerPitch = 20.0f;
    
    // Start is called before the first frame update
    void Awake()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    { 
        //水平视角
        playerHandle.transform.Rotate(Vector3.up, playerInput.lookRight * horizontalSpeed * Time.deltaTime);
         
        //竖直视角
        eulerPitch = eulerPitch - playerInput.lookUp * verticalSpeed * Time.deltaTime;
        eulerPitch = Mathf.Clamp(eulerPitch, -40, 30);
        cameraHandle.transform.localEulerAngles = new Vector3(eulerPitch, 0, 0);
    }
}
