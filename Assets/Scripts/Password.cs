using UnityEngine;
using UnityEngine.InputSystem;

public class Password : MonoBehaviour, ICanvasTool, IHideable
{
    private PlayerEventManager playerEventManager;
    private PlayerAnimator playerAnimator;
    private Prop prop;

    private Canvas passwordCanvas;
    public bool IsBeingHeld { get; private set; }

    private GameObject hidingPlace;

    [SerializeField] private MeshRenderer passwordMash; 

    private void Start() => prop = GetComponent<Prop>();

    public void Drop()
    {
        passwordCanvas.gameObject.SetActive(false);

        playerEventManager = null;
        playerAnimator = null;
        passwordCanvas = null;
        IsBeingHeld = false;
    }

    public void Pickup()
    {
        playerEventManager = prop.PlayerInteracting.GetComponent<PlayerEventManager>();
        playerAnimator = prop.PlayerInteracting.GetComponentInChildren<PlayerAnimator>();
        passwordCanvas = prop.PlayerInteracting.transform.Find("PasswordCanvas").GetComponent<Canvas>();
        IsBeingHeld = true;
    }

    public void Use()
    {
        if (passwordCanvas.gameObject.activeInHierarchy)
        {
            passwordCanvas.gameObject.SetActive(false);
            prop.EnableReposition(this.prop.PlayerInteracting);
            playerAnimator.BringToolDown(this.gameObject);
        }
        else
        {
            passwordCanvas.gameObject.SetActive(true);
            playerEventManager.ToolInteracted(this.gameObject);
        }
    }

    public void DisableCanvas() => passwordCanvas.gameObject.SetActive(false);

    public void Hide(GameObject hidingSpot)
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        Rigidbody rb = GetComponent<Rigidbody>();
        hidingPlace = hidingSpot;

        rb.isKinematic = true;
        rb.useGravity = false;
        collider.isTrigger = true;

        passwordMash.enabled = false;

        transform.position = new(hidingSpot.transform.position.x, 0f, hidingSpot.transform.position.z);
    }

    public void Reveal()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;
        collider.isTrigger = false;

        passwordMash.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == hidingPlace)
            Reveal();
    }
}
