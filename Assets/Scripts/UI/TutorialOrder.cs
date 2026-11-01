using UnityEngine;
using UnityEngine.UI;

public class TutorialOrder : MonoBehaviour
{
    [Header("Tutorial Button")]
    [SerializeField] private Button firstTutorial;
    [SerializeField] private Button secondTutorial;
    [SerializeField] private Button thirdTutorial;


    [Header("Tutorial GameObject")]
    [SerializeField] private GameObject tutorialOrder;
    [SerializeField] private GameObject firstOrder;
    [SerializeField] private GameObject secondOrder;
    [SerializeField] private GameObject thirdOrder;

    [Header("Tutorial Prefab")]
    [SerializeField] private GameObject firstTutorialPrefab;
    [SerializeField] private GameObject secondTutorialPrefab;
    [SerializeField] private GameObject thirdTutorialPrefab;

    private void Start()
    {
        TutorialGameObject();
        TutorialButtonSettings();        
        Time.timeScale = 0f;
    }

    private void TutorialGameObject()
    {
        tutorialOrder.SetActive(true);

        firstOrder = Instantiate(firstTutorialPrefab, transform.position, Quaternion.identity, tutorialOrder.transform);
        secondOrder = Instantiate(secondTutorialPrefab, transform.position, Quaternion.identity, tutorialOrder.transform);
        thirdOrder = Instantiate(thirdTutorialPrefab, transform.position, Quaternion.identity, tutorialOrder.transform);

        firstOrder.SetActive(true);
        secondOrder.SetActive(false);
        thirdOrder.SetActive(false);
    }

    private void TutorialButtonSettings()
    {
        firstTutorial = firstOrder.GetComponentInChildren<Button>();
        secondTutorial = secondOrder.GetComponentInChildren<Button>();
        thirdTutorial = thirdOrder.GetComponentInChildren<Button>();

        firstTutorial.onClick.RemoveAllListeners();
        secondTutorial.onClick.RemoveAllListeners();
        thirdTutorial.onClick.RemoveAllListeners();

        firstTutorial.onClick.AddListener(() => SecondTutorial());
        secondTutorial.onClick.AddListener(() => ThirdTutorial());
        thirdTutorial.onClick.AddListener(() => CloseTutorial());
    }

    private void SecondTutorial()
    {
        firstOrder.SetActive(false);
        secondOrder.SetActive(true);
    }

    private void ThirdTutorial()
    {
        secondOrder.SetActive(false);
        thirdOrder.SetActive(true);
    }

    private void CloseTutorial()
    {
        Time.timeScale = 1f;
        tutorialOrder.SetActive(false);
    }
}
