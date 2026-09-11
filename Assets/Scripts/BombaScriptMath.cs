using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BombaScriptMath : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI disarmText;
    [SerializeField] private Button disarmBtn;

    [Header("Mudar no futuro para um UIManager")]
    [SerializeField] private PropInteract propInteract;
    [SerializeField] private Canvas bombInterface;

    private void Update()
    {
        bombInterface.gameObject.SetActive(propInteract.isBombInteracting);
    }
    public void DisarmBomb()
    {
        disarmText.gameObject.SetActive(true);
        disarmBtn.gameObject.SetActive(false);
    }
}
