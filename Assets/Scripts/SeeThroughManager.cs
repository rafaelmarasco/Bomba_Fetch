using UnityEngine;

public class SeeThroughManager : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Vector3 offSet = new(0f, 1f, 0f);

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

        for (int i = 0; i < MAX_PLAYERS; i++)
        {
            if (i < players.Count)
            {
                Vector3 pos = players[i].transform.position + offSet;
                Shader.SetGlobalVector(viewportIds[i], cam.WorldToViewportPoint(pos));
            }
            else
            {
                Shader.SetGlobalVector(viewportIds[i], new Vector3(-100f, -100f, 0f));
            }
        }
    }
}
