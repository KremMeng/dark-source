using UnityEngine;

public class OnGroundSensor : MonoBehaviour
{
    public CapsuleCollider capCol;

    public Vector3 point1;
    public Vector3 point2;
    
    public float radius;
    public float offset = 0.1f;
    // Start is called before the first frame update
    void Awake()
    {
        radius = capCol.radius - 0.05f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        point1 = capCol.transform.position + transform.up * (radius - offset);
        point2 = capCol.transform.position + transform.up * (capCol.height - radius - offset);
        
        //储存碰撞过的物体
        Collider[] outputCols = Physics.OverlapCapsule(point1,point2,radius,LayerMask.GetMask("Ground"));
        if (outputCols.Length != 0)
        {
            SendMessageUpwards("IsOnGround");
        }
        else
        {
            SendMessageUpwards("IsNotGround");
        }
    }
}
