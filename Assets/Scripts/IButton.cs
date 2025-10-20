using UnityEngine;

public class IButton
{
    public bool IsPressing = false;
    public bool OnPressed = false;
    public bool OnReleased = false;
    public bool IsExtending = false;

    private bool currentState = false;
    private bool lastState = false;

    private ITimer extTimer = new ITimer();
    public float duartionTime = 0.15f;
    public void Tick(bool btnInput)
    {
        //超过时间间隔，转为finished状态
        extTimer.Tick();
        
        //IsPressing Signal
        currentState = btnInput;
        IsPressing = currentState;
        
        //OnPressed & OnReleased Signal
        OnPressed = false;
        OnReleased = false;
        
        if (currentState != lastState)
        {
            if (currentState)
            {
                OnPressed = true;
            }
            else
            {
                OnReleased = true;
                StartTimer(extTimer,duartionTime);
            }
        }
        lastState = currentState;

        if (extTimer.state == ITimer.STATE.RUN) {
            IsExtending = true;
        }
        else {
            IsExtending = false;
        }
    }

    public void StartTimer(ITimer timer,float duration){
        timer.duration = duration;
        timer.state = ITimer.STATE.RUN;
    }
}
