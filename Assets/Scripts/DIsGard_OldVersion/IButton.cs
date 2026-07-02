// using UnityEngine;
//
// public class IButton
// {
//     public bool IsPressing = false;
//     public bool OnPressed = false;
//     public bool OnReleased = false;
//     
//     public bool IsExtending = false;
//     public bool IsDelaying = false;
//
//     private bool currentState = false;
//     private bool lastState = false;
//     
//     public float extendingDuartion = 0.4f;
//     public float delayingDuartion = 0.4f;
//     
//     private ITimer extTimer = new ITimer();
//     private ITimer delayTimer = new ITimer();
//     
//     //判断各个信号
//     public void Tick(bool btnInput)
//     {
//         //每一帧tick
//         extTimer.Tick();
//         delayTimer.Tick();
//         
//         //IsPressing Signal
//         currentState = btnInput;
//         IsPressing = currentState;
//         
//         //OnPressed & OnReleased Signal
//         OnPressed = false;
//         OnReleased = false;
//         
//         IsExtending = false;
//         IsDelaying = false;
//         
//         //判断按下/松手瞬间，开启需要的计时器
//         if (currentState != lastState)
//         {
//             if (currentState)
//             {
//                 OnPressed = true;
//                 StartTimer(delayTimer,delayingDuartion);
//             }
//             else
//             {
//                 OnReleased = true;
//                 StartTimer(extTimer,extendingDuartion);
//             }
//         }
//         
//         //上面有开启计时器，则判断为true
//         if (extTimer.state == ITimer.STATE.RUN) {
//             IsExtending = true;
//         }
//
//         if (delayTimer.state == ITimer.STATE.RUN) {
//             IsDelaying = true;
//         }
//         
//         lastState = currentState;
//     }
//     //开启计时器，仅调用一次
//     public void StartTimer(ITimer timer,float duration){
//         timer.eclipseTime = 0;
//         timer.duration = duration;
//         timer.state = ITimer.STATE.RUN;
//     }
// }
