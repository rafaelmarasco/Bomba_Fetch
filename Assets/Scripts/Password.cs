using UnityEngine;
using UnityEngine.InputSystem;

public class Password : MonoBehaviour, ICanvasTool, IHideable
{
    private PlayerEventManager playerEventManager;
    private PlayerAnimator playerAnimator;
    private Prop prop;

    [Header ("Password Visual")]
    [SerializeField] private MeshRenderer passwordMash; 

    private GameObject hidingPlace;

    private Canvas passwordCanvas;
    public bool IsBeingHeld { get; private set; }

    public bool IsHidden => hidingPlace != null;

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

        transform.position = new(hidingSpot.transform.position.x, 0f, hidingSpot.transform.position.z);

        hidingPlace.GetComponent<Prop>().SetAsHiddingSpot(true, this);

        passwordMash.enabled = false;
    }

    public void Reveal()
    {
        if (!IsHidden) return;

        BoxCollider collider = GetComponent<BoxCollider>();
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;
        collider.isTrigger = false;

        hidingPlace.GetComponent<Prop>().SetAsHiddingSpot(false, this);

        hidingPlace = null;

        passwordMash.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == hidingPlace)
            Reveal();
    }
}
