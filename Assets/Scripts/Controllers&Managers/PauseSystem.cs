using UnityEngine;

public class PauseSystem : MonoBehaviour
{
    private bool isGameFrozzen = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            AlternatePauseState();
    }

    private void AlternatePauseState()
    {
        isGameFrozzen = !isGameFrozzen;
        Time.timeScale = isGameFrozzen ? 0f : 1f;

    }
}
