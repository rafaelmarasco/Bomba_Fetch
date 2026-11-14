using UnityEngine;

public class PlayerSearchHandler : MonoBehaviour
{
    private PlayerInputHendler propInteract;

    private void Start()
    {
        propInteract = GetComponent<PlayerInputHendler>();
    }

    public void Search()
    {
        if (!propInteract.CheckForProps(out GameObject prop)) return;
        prop.TryGetComponent(out Prop propBeingSearch);

        Debug.Log($"Procurando objeto em {prop.name} que é um esconderijo: {propBeingSearch.IsHiddingSpot}");

        if (!propBeingSearch.IsHiddingSpot) return;

        propBeingSearch.HiddenProp?.Reveal();

    }
}
