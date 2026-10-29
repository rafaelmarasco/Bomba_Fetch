using UnityEngine;

public class Password : MonoBehaviour, ICanvasTool
{
    private Prop prop;
    private PlayerEventManager playerEventManager;
    private PlayerAnimator playerAnimator;
    private Canvas passwordCanvas;
    public bool IsBeingHeld { get; private set; }

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

}
