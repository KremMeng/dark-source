public class IButton
{
    public bool IsPressing = false;
    public bool OnPressed = false;
    public bool OnReleased = false;

    private bool currentState = false;
    private bool lastState = false;

    public void Tick(bool btnInput)
    {
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
            }
        }
        lastState = currentState;
    }
}
