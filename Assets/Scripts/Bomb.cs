using UnityEngine;

public class Bomb : MonoBehaviour, ICanvasTool
{
    private Prop prop;
    private PlayerEventManager playerEventManager;
    private PlayerAnimator playerAnimator;
    private Canvas bombCanvas;
    public bool IsBeingHeld { get; private set; }

    private void Start() => prop = GetComponent<Prop>();

    public void Drop()
    {
        bombCanvas.gameObject.SetActive(false);

        playerEventManager = null;
        playerAnimator = null;
        bombCanvas = null;
        IsBeingHeld = false;
    }

    public void Pickup()
    {
        playerEventManager = prop.PlayerInteracting.GetComponent<PlayerEventManager>();
        playerAnimator = prop.PlayerInteracting.GetComponentInChildren<PlayerAnimator>();
        bombCanvas = prop.PlayerInteracting.transform.Find("BombCanvas").GetComponent<Canvas>();
        IsBeingHeld = true;
    }

    public void Use()
    {
        if (bombCanvas.gameObject.activeInHierarchy)
        {
            bombCanvas.gameObject.SetActive(false);
            prop.EnableReposition(this.prop.PlayerInteracting);
            playerAnimator.BringToolDown(this.gameObject);
        }
        else
        {
            bombCanvas.gameObject.SetActive(true);
            playerEventManager.ToolInteracted(this.gameObject);
        }
    }

    public void DisableCanvas() => bombCanvas.gameObject.SetActive(false);

}
