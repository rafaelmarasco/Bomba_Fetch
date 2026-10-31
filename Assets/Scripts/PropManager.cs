using UnityEngine;

public class PropManager : MonoBehaviour
{
    [SerializeField] private PhysicsMaterial physicsMaterial;
    public PhysicsMaterial PhysicsMaterial => physicsMaterial;
}
