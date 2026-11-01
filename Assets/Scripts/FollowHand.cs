using UnityEngine;

public class FollowHand : MonoBehaviour
{
    [SerializeField] private GameObject playerhand;
    void Update()
    {
        transform.position = playerhand.transform.position;    
    }
}
