using UnityEngine;

public class JoystickTest : MonoBehaviour
{
    private string[] joystickNames;
    private const float DEAD = 0.19f;
    
    void Update()
    {
        joystickNames = Input.GetJoystickNames();
        //检测button
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                Debug.Log("Key pressed: " + keyCode.ToString());
                break;          // 找到就停，防止刷屏
            }
        }
        print("axis9: "+ Input.GetAxis("axis9"));
        print("axis10: "+ Input.GetAxis("axis10"));
        print("axis3: "+ Input.GetAxis("axis3"));
    }
    
}
