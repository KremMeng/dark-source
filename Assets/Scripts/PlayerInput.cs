using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //定义键位
    public string keyUp = "w";
    public string keyDown = "s";
    public string keyLeft = "a";
    public string keyRight = "d";
    [Space]
    //定义方向轴
    public float dirUp;
    public float dirRight;
    public float targetDirUp;
    public float targetDirRight;
    private float velocityUp;
    private float velocityRight;
    [Space]
    //软开关flag
    public bool inputEnabled = true;
    
    void Update()
    {
        targetDirUp = (Input.GetKey(keyUp) ? 1.0f : 0)- (Input.GetKey(keyDown) ? 1.0f : 0);
        targetDirRight = (Input.GetKey(keyRight) ? 1.0f : 0)- (Input.GetKey(keyLeft) ? 1.0f : 0);
        if (inputEnabled == false)
        {
            targetDirUp = 0;
            targetDirRight = 0;
        }
        dirUp = Mathf.SmoothDamp(dirUp, targetDirUp, ref velocityUp, 1.0f);
        dirRight = Mathf.SmoothDamp(dirRight, targetDirRight, ref velocityRight, 1.0f);
    }
}
