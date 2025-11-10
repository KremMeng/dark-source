using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IUserInput : MonoBehaviour
{ [Header("===== Output signals =====")]
    //方向轴信号
    public float dirUpOrigin;
    public float dirRightOrigin;
    public float dirMagnity;
    public Vector3 dirVector;
    
    public float lookUp;
    public float lookRight;
    
    //动作信号
    public bool run;
    public bool roll;
    public bool jab;
    public bool jump;
    public bool lastJump;
    //public bool attack;
    public bool lastAttack;
    public bool defense;
    public bool lockon;

    public bool lb;
    public bool rb;
    public bool lt;
    public bool rt;
    
    [Header("===== Others =====")]
    //软开关flag
    public bool inputEnabled = true;
    
    public float targetDirUp;
    public float targetDirRight;
    protected float _velocityUp;
    protected float _velocityRight;
    
    protected Vector2 Square2Circle(Vector2 input)
    {
        Vector2 output = Vector2.zero;

        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2);
        
        return output;
    }

    protected void CalculateDmagDvec(float dirUp,float dirRight){
        dirMagnity = Mathf.Sqrt(dirUp * dirUp + dirRight * dirRight);
        dirVector = dirUp * transform.forward + dirRight * transform.right;
    }
}
