using UnityEngine;

public class FollowHand : MonoBehaviour
{
    [SerializeField] private GameObject playerhand;
    void FixedUpdate()
    {
        transform.position = playerhand.transform.position;    
    }
}
