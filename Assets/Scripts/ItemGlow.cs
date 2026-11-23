using UnityEngine;

public class ItemGlow : MonoBehaviour
{
    [SerializeField]
    private GameObject neonGlow;

    public void TurnOn() => neonGlow.SetActive(true);

    public void TurnOff() => neonGlow.SetActive(false);
}
