using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SwitchInputUI : MonoBehaviour
{
    [SerializeField] private GameObject firstButton;

    private void OnEnable()
    {
        StartCoroutine(TrocarParaUI());
    }

    private IEnumerator TrocarParaUI()
    {
        yield return null; // espera 1 frame para os players existirem

        foreach (var p in PlayerInput.all)
            p.SwitchCurrentActionMap("UI");

        if (firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
    }

    private void OnDisable()
    {
        foreach (var p in PlayerInput.all)
            p.SwitchCurrentActionMap("Player");
    }
}
