using UnityEngine;

public class SpawnAtRandomLocation : MonoBehaviour
{
    [SerializeField] private Transform[] locals;
    public bool Randomized { get; private set; } = false;

    void Start() => SetRandomLocation();

    private void SetRandomLocation()
    {
        int randomIndex = Random.Range(0, locals.Length);

        transform.position = locals[randomIndex].position;

        Debug.Log(locals[randomIndex].name);

        Randomized = true;
    }
}
