using UnityEngine;

public class Bomb : MonoBehaviour, IHeldTool
{
    private Prop prop;
    private PlayerEventManager playerEventManager;

    public bool IsBeingHeld {  get; private set; }

    private void Start()
    {
        prop = GetComponent<Prop>();
    }

    public void Drop()
    {
        playerEventManager = null;
        IsBeingHeld = false;
    }

    public void Pickup()
    {
        playerEventManager = prop.PlayerInteracting.GetComponent<PlayerEventManager>();
        IsBeingHeld = true;
    }

    public void Use()
    {

        playerEventManager.BombInteracted(this.gameObject);
    }

}
