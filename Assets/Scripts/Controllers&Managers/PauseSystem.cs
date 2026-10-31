using UnityEngine;

public class PauseSystem : MonoBehaviour
{
    private bool isGameFrozzen = false;
    [SerializeField] private GameObject prefabPauseScreen;
    private GameObject pauseScreen;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            AlternatePauseState();
    }

    private void AlternatePauseState()
    {
        isGameFrozzen = !isGameFrozzen;

        if (pauseScreen == null)
        {
            pauseScreen = Instantiate(prefabPauseScreen, transform.position, Quaternion.identity);
        }

        if (isGameFrozzen)
        {
            Time.timeScale = 0f;
            
            if (pauseScreen.activeInHierarchy == false)
            {
               pauseScreen.SetActive(true);
            }
        }
        else if (!isGameFrozzen)
        {
            Time.timeScale = 1f;
            if(pauseScreen.activeInHierarchy == true)
            {
                pauseScreen.SetActive(false);
            }
        }
        

    }
}
