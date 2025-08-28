using UnityEngine;

public class ActorController : MonoBehaviour
{
    private static readonly int Forward = Animator.StringToHash("forward");
    public GameObject model;

    public PlayerInput pi;

    [SerializeField] private Animator anim;
    
    // Start is called before the first frame update
    void Awake()
    {
        pi = GetComponent<PlayerInput>();
        anim = model.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat(Forward, pi.dirUp);
    }
}
