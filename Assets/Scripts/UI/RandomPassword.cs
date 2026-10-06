using UnityEngine;
using TMPro;

public class RandomPassword : MonoBehaviour
{
    [SerializeField] private TMP_Text passwordText; 

    private int[] password;
    private int passwordLength = 4;
    private int inicialRangeNumber = 0;
    private int finalRangeNumber = 9;


    private void OnEnable()
    {
        password = new int[passwordLength];
        GeneratePassword();
    }

    private void GeneratePassword()
    {
        for (int i = 0; i < passwordLength; i++)
        {
            int number = Random.Range(inicialRangeNumber, finalRangeNumber);
            password[i] = number;
        }

        passwordText.text = string.Join("", password);
    }
    
}
