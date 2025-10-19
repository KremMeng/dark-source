using UnityEngine;

public class keyboardInput : IUserInput
{
    [Header("===== Key settings =====")]
    //移动键位
    public string keyUp = "w";
    public string keyDown = "s";
    public string keyLeft = "a";
    public string keyRight = "d";
    
    public string keyA = "left shift";
    public string keyB = "space";
    public string keyC = "j";
    public string keyD = "k";
    
    //相机键位
    public string cameraUp;
    public string cameraDown;
    public string cameraRight;
    public string cameraLeft;

    [Header("===== Mouse settings")]
    public bool mouseEnabled = false;

    public float mouseSensityX = 1.0f;
    public float mouseSensityY = 1.0f;
    
   
    void Update()
    {
        if (mouseEnabled)
        {
            lookUp = Input.GetAxis("Mouse Y") * 2.5f * mouseSensityY;
            lookRight = Input.GetAxis("Mouse X") * 3.0f * mouseSensityX;
        }
        else
        {
            lookUp = (Input.GetKey(cameraUp) ? 1.0f : 0) - (Input.GetKey(cameraDown) ? 1.0f : 0);
            lookRight = (Input.GetKey(cameraRight) ? 1.0f : 0) - (Input.GetKey(cameraLeft) ? 1.0f : 0);
        }
       
        
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
        bool newJump = Input.GetKey(keyB);
        bool newAttack = Input.GetKey(keyC);
        defense = Input.GetKey(keyD);
        
        if (newJump != lastJump && newJump)
        {
            jump = true;
        }
        else {
            jump = false;
        }
        lastJump = newJump;

        if (newAttack != lastAttack && newAttack)
        {
            attack = true;
        }
        else
        {
            attack = false;
        }
        lastAttack = newAttack;
    }

  
}
