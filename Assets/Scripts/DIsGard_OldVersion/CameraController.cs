using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    //public JoystickInput playerInput;
    private IUserInput playerInput;
    
    private GameObject playerHandle;
    private GameObject cameraHandle;
    private GameObject _model;
    private GameObject _camera;

    public float horizontalSpeed = 100.0f;     
    public float verticalSpeed = 80.0f;
    private Vector3 cameraDampVelocity;
    public float cameraOffset = 0.05f;

    private LockTarget lockTarget;
    public Image lockDot;
    public bool lockState; //索敌状态flag

    public bool isAI = false;   //敌人(AI)或玩家(Player)
    
    [FormerlySerializedAs("eulerTemp")] public float eulerPitch = 20.0f;

    public class LockTarget {
        public GameObject go;
        public float halfHeight;

        public LockTarget(GameObject go, float halfHeight){
            this.go = go;
            this.halfHeight = halfHeight;
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        //_model = playerHandle.GetComponent<ActorController>().model;
        //playerInput = playerHandle.GetComponent<ActorController>().playerInput; 违反封装
        ActorController ac = playerHandle.GetComponent<ActorController>(); //引用，局部变量
        _model = ac.model;
        playerInput = ac.playerInput;
        if (Camera.main != null) _camera = Camera.main.gameObject;

        if (!isAI) {//敌人不需要索敌小红点，会影响玩家视角
            lockState = false;
            lockDot.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //没有索敌的时候，正常水平竖直视角转换
        if (lockTarget == null) {
            Vector3 eulerTemp = _model.transform.eulerAngles; //保存当前朝向的角度
        
            //水平视角，由playerHandle方向定
            playerHandle.transform.Rotate(Vector3.up, playerInput.lookRight * horizontalSpeed * Time.fixedDeltaTime);
            //竖直视角，由cameraHandle方向定
            eulerPitch = eulerPitch - playerInput.lookUp * verticalSpeed * Time.fixedDeltaTime;
            eulerPitch = Mathf.Clamp(eulerPitch, -40, 30);
            cameraHandle.transform.localEulerAngles = new Vector3(eulerPitch, 0, 0);//相对于父级角色的旋转,避免覆盖掉水平旋转
        
            //让角色不跟着世界坐标转
            _model.transform.eulerAngles = eulerTemp;//每一帧结尾，回到该帧初始角度
        }
        //索敌的时候，角色始终lookat目标敌人
        else {
            Vector3 dirPlayer2Enemy = lockTarget.go.transform.position - _model.transform.position;
            dirPlayer2Enemy.y = 0;
            playerHandle.transform.forward = dirPlayer2Enemy;
            cameraHandle.transform.LookAt(lockTarget.go.transform); //索敌时，视角更偏向怪物的脚底
            //_model.transform.forward = new Vector3(dirPlayer2Enemy.x,0,dirPlayer2Enemy.z);
        }

        if (!isAI) {
            //延迟相机,主相机追cameraPos
            _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, transform.position, ref cameraDampVelocity, cameraOffset);
            //_camera.transform.eulerAngles = transform.eulerAngles;
            _camera.transform.LookAt(cameraHandle.transform); //固定看向后脖颈
        }
        
    }
    void Update(){
        if (lockTarget != null) {
            //把小圆点放到目标敌人的半高位置上
            if (!isAI) {
                lockDot.transform.position = Camera.main.WorldToScreenPoint(lockTarget.go.transform.position + new Vector3(0,lockTarget.halfHeight,0));
            }
            
            if (Vector3.Distance(playerHandle.transform.position, lockTarget.go.transform.position) > 10.0f) {
                lockProgress(null,false,false,isAI);
            }
        }
    }
    //区分敌人AI和玩家的索敌,,isAI在编辑器面板手动设置
    public void lockProgress(LockTarget _lockTarget,bool _lockState,bool _lockEnabled,bool _isAI){
        lockTarget = _lockTarget;
        lockState = _lockState;
        lockDot.enabled = _lockEnabled;     
    }
    //索敌，解开索敌
    public void LockOrLockOn(){
        //用OverlapBox先计算碰撞体数组
        Collider[] cols;
        
        Vector3 origin1 = _model.transform.position;
        Vector3 origin2 = origin1 + new Vector3(0, 1, 0);
        Vector3 boxCenter = origin2 + _model.transform.forward * 5.0f;
        cols = Physics.OverlapBox(boxCenter, new Vector3(0.8f, 0.8f, 5), _model.transform.rotation,
            LayerMask.GetMask(isAI ?"Player":"Enemy"));
       //遍历碰撞体数组，设置索敌开关
       //先检查空
       if (cols.Length == 0) {
           lockProgress(null,false,false,isAI);
       }
       else {
           foreach (var col in cols) {
               print(col.name);
               //检测到的碰撞体和目前的锁定目标相同，那么就取消锁定
               if (lockTarget != null && col.gameObject == lockTarget.go) {
                   lockProgress(null,false,false,isAI);
                   break;
               }
                //否则，让新的数组元素作为lockTarget
                lockProgress(new LockTarget(col.gameObject,col.bounds.extents.y),true,true,isAI);
                break;
           }
       }
       
    }
}
