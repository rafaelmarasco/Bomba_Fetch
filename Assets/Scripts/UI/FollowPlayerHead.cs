using UnityEngine;

public class FollowPlayerHead : MonoBehaviour
{
    [SerializeField] private Vector3 offset; // acima da cabeça, em coordenadas do mundo
    private Camera cam;
    private Transform player;

    void Start()
    {
        cam = Camera.main;
        player = transform.parent;                    // o player (pai do Canvas)
        GetComponent<Canvas>().worldCamera = cam;     // Event Camera por código
    }

    void LateUpdate()
    {
        // posição: sempre acima do player, ignorando a rotação dele
        transform.position = player.position + offset;

        // rotação: sempre virado para a câmera
        transform.rotation = cam.transform.rotation;
    }
}
