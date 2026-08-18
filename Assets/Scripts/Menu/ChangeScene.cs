using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    SceneController sceneController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneController = new SceneController();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScenes(int numberScene)
    {
        sceneController.LoadScene(numberScene);
    }
}
