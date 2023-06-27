using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPBase : MonoBehaviour
{

    public Rigidbody Rigidbody { get { return rb; } }
    public Transform Root { get { return root; } }
    public GameObject Visual { get { return visual; } }
    public Vector3 Center { get { return Root.position; } }

    public virtual bool HasInit { get { return hasInit; } }
    public bool IsLocal {get{return isLocal;}}
    public System.Action OnPlayerToggle;
    public System.Action OnInit;
    protected bool hasInit;

    [Header("Settings")]
    [SerializeField] protected Transform root;
    [SerializeField] protected GameObject visual;
    [SerializeField] protected bool hasBeenUpdated = false;
    [SerializeField] protected bool isLocal;
    protected Rigidbody rb;

    Vector3 oldPosition;

    protected virtual void Awake()
    {

        rb = GetComponent<Rigidbody>();
        if (rb == null) { rb = gameObject.AddComponent<Rigidbody>(); }
        if (Root == null) { root = transform; }
        if (Visual == null) { visual = gameObject; }

    }

    protected virtual void Start()
    {
        Init();
    }

    protected virtual void OnEnable() {}
    protected virtual void OnDisable() {
        if(HasInit) {
            Destroy();
        }
    }
    protected virtual void Destroy() {}

    public virtual void Init()
    {
        hasInit = true;
        enabled = true;
    }

    protected virtual void NetworkInit() {

    }

    protected virtual void Update()
    {

    }

    public virtual void ToggleVisual(bool toggle) {
        
    }

    public virtual void Kinematic(bool toggle) {
        rb.isKinematic = toggle;
    }
    public void Teleport(Vector3 newPosition) {
        Teleport(newPosition,transform.rotation);
    }

    public virtual void Teleport(Vector3 newPosition, Quaternion newRotation) {

        oldPosition = newPosition;

        Rigidbody.position = newPosition;
        Rigidbody.rotation = newRotation;

        Rigidbody.velocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;

        transform.position = newPosition;
        transform.rotation = newRotation;
  
    }


}
