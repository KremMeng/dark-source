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
    
    public IButton buttonA = new IButton();
    public IButton buttonB = new IButton();
    public IButton buttonC = new IButton();
    public IButton buttonD = new IButton();
    public IButton buttonLB = new IButton();

    // Update is called once per frame
    void Update()
    {
        buttonA.Tick(Input.GetButton(btnA));
        buttonB.Tick(Input.GetButton(btnB));
        buttonC.Tick(Input.GetButton(btnC));
        buttonD.Tick(Input.GetButton(btnD));
        buttonLB.Tick(Input.GetButton(btnLB));

        
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

        run = buttonA.IsPressing;
        defense = buttonLB.IsPressing;
        jump = buttonB.OnPressed;
        attack = buttonC.OnPressed;
    }
}
