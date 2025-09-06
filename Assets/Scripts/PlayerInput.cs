using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("===== Key settings =====")]
    //定义键位
    public string keyUp = "w";
    public string keyDown = "s";
    public string keyLeft = "a";
    public string keyRight = "d";

    public string keyA;
    public string keyB;
    public string keyC;
    public string keyD;
    
    public bool run;
    [Header("===== Output signals =====")]
    //定义方向轴
    public float dirUpOrigin;
    public float dirRightOrigin;
    public float targetDirUp;
    public float targetDirRight;
    private float _velocityUp;
    private float _velocityRight;
    
    [Header("===== Others =====")]
    //软开关flag
    public bool inputEnabled = true;
    
    [SerializeField] public float dirMagnity;
    [SerializeField] public Vector3 dirVector;

    
    void Update()
    {
        targetDirUp = (Input.GetKey(keyUp) ? 1.0f : 0)- (Input.GetKey(keyDown) ? 1.0f : 0);
        targetDirRight = (Input.GetKey(keyRight) ? 1.0f : 0)- (Input.GetKey(keyLeft) ? 1.0f : 0);
        if (inputEnabled == false)
        {
            targetDirUp = 0;
            targetDirRight = 0;
        }
        dirUpOrigin = Mathf.SmoothDamp(dirUpOrigin, targetDirUp, ref _velocityUp, 1.0f);
        dirRightOrigin = Mathf.SmoothDamp(dirRightOrigin, targetDirRight, ref _velocityRight, 1.0f);
        
        //椭球映射
        Vector2 circleInput = Square2Circle(new Vector2(dirRightOrigin, dirUpOrigin));
        float dirRight = circleInput.x;
        float dirUp = circleInput.y;
        
        dirMagnity = Mathf.Sqrt(dirUp * dirUp + dirRight * dirRight);
        dirVector = dirUp * transform.forward + dirRight * transform.right;

        run = Input.GetKey(keyA);
    }

    private Vector2 Square2Circle(Vector2 input)
    {
        Vector2 output = Vector2.zero;

        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2);
        
        return output;
    }
}
