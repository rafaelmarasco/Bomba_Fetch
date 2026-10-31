using System.Collections;
using TMPro;
using UnityEngine;

public class ValidatePassword : MonoBehaviour
{
    [SerializeField] private RandomPassword passwordCheck;
    [SerializeField] private string submitPassword;
    [SerializeField] private GameObject victoryPrefab;
    [SerializeField] private GameObject feedbackTextGameObject;
    private TextMeshProUGUI feedbackText;

    private GameObject victoryScreen;

    private void Start()
    {
        feedbackTextGameObject.SetActive(false);
        if (victoryScreen != null)
        {
            Destroy(victoryScreen);
        }
    }

    public void CheckPassword()
    {
        if (passwordCheck.GetPassword() == null || passwordCheck.GetPassword() != submitPassword)
        {

            MostrarFeedback();           

        }
        else
        {
            if (victoryScreen == null)
            {
                victoryScreen = Instantiate(victoryPrefab, transform.position, Quaternion.identity);
            }
        }
    }

    public void SetSubmitPassword(string password)
    {
        submitPassword = password;
    }

    private void MostrarFeedback()
    {
        feedbackTextGameObject.SetActive(true);
        feedbackText = feedbackTextGameObject.GetComponentInChildren<TextMeshProUGUI>();
        feedbackText.text = "Senha incorreta";
        CancelInvoke(nameof(EsconderFeedback));
        Invoke(nameof(EsconderFeedback), 2f);
    }

    private void EsconderFeedback() => feedbackTextGameObject.SetActive(false);
}
