// using UnityEngine;
// using UnityEngine.Serialization;
//
// public class JoystickInput : IUserInput
// {
//     [Header("===== Joystick settings =====")]
//     //左摇杆
//     public string axisX = "axisX";
//     public string axisY = "axisY";
//     //右摇杆
//     public string axisLookUp = "axis4";
//     public string axisLookRight = "axis5";
//     
//     [FormerlySerializedAs("btn0")] public string btnA = "btn0";
//     [FormerlySerializedAs("btn1")] public string btnB = "btn1";
//     [FormerlySerializedAs("btn2")] public string btnC = "btn2";
//     [FormerlySerializedAs("btn3")] public string btnD = "btn3";
//     public string btnLB = "btn4";
//     public string btnRB = "btn5";
//     public string btnRStick = "btn9";
//     
//     public IButton buttonA = new IButton();
//     public IButton buttonB = new IButton();
//     public IButton buttonC = new IButton();
//     public IButton buttonD = new IButton();
//     public IButton buttonLB = new IButton();
//     public IButton buttonRB = new IButton();
//     public IButton buttonLT = new IButton();
//     public IButton buttonRT = new IButton();
//     public IButton buttonRStick = new IButton();
//     
//     // Update is called once per frame
//     void Update()
//     {
//         buttonA.Tick(Input.GetButton(btnA));
//         buttonB.Tick(Input.GetButton(btnB));
//         buttonC.Tick(Input.GetButton(btnC));
//         buttonD.Tick(Input.GetButton(btnD));
//         buttonLB.Tick(Input.GetButton(btnLB));
//         buttonRB.Tick(Input.GetButton(btnRB));
//         buttonLT.Tick(Input.GetAxis("axis9")>=0.9f);
//         buttonRT.Tick(Input.GetAxis("axis10")>=0.9f);
//         buttonRStick.Tick(Input.GetButton(btnRStick));
//         
//         lookUp = Input.GetAxis(axisLookUp);
//         lookRight = Input.GetAxis(axisLookRight);
//         
//         targetDirUp = Input.GetAxis(axisY);
//         targetDirRight = Input.GetAxis(axisX);
//         
//         if (inputEnabled == false)
//         {
//             targetDirUp = 0;
//             targetDirRight = 0;
//         }
//         dirUpOrigin = Mathf.SmoothDamp(dirUpOrigin, targetDirUp, ref _velocityUp, 1.0f);
//         dirRightOrigin = Mathf.SmoothDamp(dirRightOrigin, targetDirRight, ref _velocityRight, 1.0f);
//         
//         //椭球映射
//         Vector2 circleInput = Square2Circle(new Vector2(dirRightOrigin, dirUpOrigin));
//         float dirRight = circleInput.x;
//         float dirUp = circleInput.y;
//         
//         dirMagnity = Mathf.Sqrt(dirUp * dirUp + dirRight * dirRight);
//         dirVector = dirUp * transform.forward + dirRight * transform.right;
//
//         run = (buttonA.IsPressing && !buttonA.IsDelaying) || buttonA.IsExtending ;
//         jump = buttonA.OnPressed && buttonA.IsExtending; //松手后的延时内短按
//         roll = buttonA.IsDelaying && buttonA.OnReleased; //蓄力延时窗口内，就松开了按钮;
//         
//         defense = buttonLB.IsPressing;
//         //attack = buttonC.OnPressed;
//         lb = buttonLB.OnPressed;
//         rb = buttonRB.OnPressed;
//         lt = buttonLT.OnPressed;
//         rt = buttonRT.OnPressed;
//
//         lockon = buttonRStick.OnPressed;
//     }
// }
