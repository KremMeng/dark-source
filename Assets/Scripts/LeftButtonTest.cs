using UnityEngine;
using UnityEngine.InputSystem;

public class LeftButtonTest : MonoBehaviour
{
    public void LeftButtonTester(InputAction.CallbackContext context){
        print("Input system: left mouse btn down");
    }
}
