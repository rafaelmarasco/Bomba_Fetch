using UnityEngine;

public class SpawnAtRandomLocation : MonoBehaviour
{
    [SerializeField] private GameObject[] locals;
    public bool Randomized { get; private set; } = false;

    public GameObject HidingSpot { get; private set; }

    void Start() => SetRandomHidingLocation();

    private void SetRandomHidingLocation()
    {
        int randomIndex = Random.Range(0, locals.Length);

        HidingSpot = locals[randomIndex];

        transform.position = HidingSpot.transform.position;

        Debug.Log(HidingSpot.name);

        Randomized = true;
    }
}
