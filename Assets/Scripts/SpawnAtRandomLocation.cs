using UnityEngine;

public class SpawnAtRandomLocation : MonoBehaviour
{
    [SerializeField] private GameObject[] locals;
    public bool Randomized { get; private set; } = false;

    public GameObject HidingSpot { get; private set; } = null;

    IHideable hideable;

    void Awake()
    {
        hideable = GetComponent<IHideable>();
        SetRandomHidingLocation();
        hideable.Hide(HidingSpot);
    } 

    private void SetRandomHidingLocation()
    {
        if (locals.Length == 0) return;

        int randomIndex = Random.Range(0, locals.Length);

        HidingSpot = locals[randomIndex];

        transform.position = HidingSpot.transform.position;

        Debug.Log(HidingSpot.name);

        Randomized = true;
    }
}
