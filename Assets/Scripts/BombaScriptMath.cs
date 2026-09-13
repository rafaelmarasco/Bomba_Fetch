using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BombaScriptMath : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI disarmText;
    [SerializeField] private Button disarmBtn;


    //[SerializeField] private Button[] numbersButton;

    [Header("Mudar no futuro para um UIManager")]
    [SerializeField] private PropInteract propInteract;
    [SerializeField] private Canvas bombInterface;
    [SerializeField] private PlayerInput uiInput;

    private void Update()
    {
        CheckBomb();
    }

    private void CheckBomb()
    {
        bombInterface.gameObject.SetActive(propInteract.isBombInteracting);
        
        if (propInteract.isBombInteracting)
        {
            BombUIConfig();    
        }
    }

    private void BombUIConfig()
    {
        uiInput = GetComponent<PlayerInput>();
    }

    public void DisarmBomb()
    {
        disarmText.gameObject.SetActive(true);
        disarmBtn.gameObject.SetActive(false);
    }
}
