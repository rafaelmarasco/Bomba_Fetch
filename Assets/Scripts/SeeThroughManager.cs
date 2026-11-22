using UnityEngine;

public class SeeThroughManager : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Vector3 offSet = new(0f, 1f, 0f);
    [SerializeField]
    private LayerMask occluderMask;

    static readonly int Forward = Shader.PropertyToID("_SeeThroughForward");
    private static readonly Vector3 EmptyValue = new(-100f, -100f, -10000f);

    const int MAX_PLAYERS = 4;
    int[] viewportIds;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;

        viewportIds = new int[MAX_PLAYERS];

        for (int i = 0; i < MAX_PLAYERS; i++)
        {
            viewportIds[i] = Shader.PropertyToID($"_Player{i}ViewportPos");
        }
    }

    private void LateUpdate()
    {
        var players = SeeThroughTarget.All;

        Vector3 fwd = cam.transform.forward;
        fwd.y = 0f;
        fwd.Normalize();
        Shader.SetGlobalVector(Forward, fwd);


        for (int i = 0; i < MAX_PLAYERS; i++)
        {
            if (i < players.Count)
            {
                Vector3 pos = players[i].transform.position + offSet;

                if (IsOccluded(pos))
                {
                    Vector3 viewPort = cam.WorldToViewportPoint(pos);
                    viewPort.z = Vector3.Dot(pos, fwd);
                    Shader.SetGlobalVector(viewportIds[i], viewPort);
                }
                else
                    Shader.SetGlobalVector(viewportIds[i], EmptyValue);
            }
            else
                Shader.SetGlobalVector(viewportIds[i], EmptyValue);
        }
    }

    private bool IsOccluded(Vector3 target)
    {
        Vector3 origin = cam.orthographic ? target - cam.transform.forward * 100f : cam.transform.position;

        return Physics.Linecast(origin, target, occluderMask, QueryTriggerInteraction.Ignore);
    }
}
