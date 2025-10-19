using UnityEngine;
using UnityEngine.Serialization;

public class JoystickInput : IUserInput
{
    [Header("===== Joystick settings =====")]
    //左摇杆
    public string axisX = "axisX";
    public string axisY = "axisY";
    //右摇杆
    public string axisLookUp = "axis4";
    public string axisLookRight = "axis5";
    
    [FormerlySerializedAs("btn0")] public string btnA = "btn0";
    [FormerlySerializedAs("btn1")] public string btnB = "btn1";
    [FormerlySerializedAs("btn2")] public string btnC = "btn2";
    [FormerlySerializedAs("btn3")] public string btnD = "btn3";
    
    public string btnLB = "btn4";
    
   
    // Update is called once per frame
    void Update()
    {
        lookUp = Input.GetAxis(axisLookUp);
        lookRight = Input.GetAxis(axisLookRight);
        
        targetDirUp = Input.GetAxis(axisY);
        targetDirRight = Input.GetAxis(axisX);
        
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

        run = Input.GetButton("btn0");
        defense = Input.GetButton(btnLB);
        bool newJump = Input.GetButtonDown("btn1");
        if (newJump != lastJump && newJump)
        {
            jump = true;
        }
        else {
            jump = false;
        }
        lastJump = newJump;
        
        bool newAttack = Input.GetButtonDown("btn2");
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
