using Unity.VisualScripting;
using UnityEngine;

public class SeeThroughTarget : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Vector3 offSet = new(0f, 1f, 0f);


    private static readonly int WorldId = Shader.PropertyToID("_PlayerWorldPos");
    private static readonly int ViewPortId = Shader.PropertyToID("_PlayerViewPortPos");

    private void Start() { if (cam == null) cam = Camera.main; }

    private void LateUpdate()
    {
        Vector3 pos = transform.position + offSet;
        Shader.SetGlobalVector(WorldId, pos);
        Shader.SetGlobalVector(ViewPortId, cam.WorldToViewportPoint(pos));
    }
}
