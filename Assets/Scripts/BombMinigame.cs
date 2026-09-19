using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using JetBrains.Annotations;


public class BombMinigame : MonoBehaviour
{
    private const string BOMB = "Bomb";
    private const string NAVIGATE = "Navigate";
    private const string SUBMIT = "Submit";
    private const string CANCEL = "Cancel";
    private const int BUTTON_COUNT = 9;

    [Header("Input Settings")]
    [SerializeField]private PlayerInput playerInput;
    private InputActionMap inputMap;
    private InputAction navigateAction;
    private InputAction submitAction;
    private InputAction cancelAction;

    [Header("Minigame Settings")]
    [SerializeField] private Button[] buttons;
    [SerializeField] private int column = 3;
    private int id = 0;

    [Header("Display Settings")]
    [SerializeField] private TextMeshProUGUI[] displayTexts;
    private int position = 0;
    //private int valueButton = 1;

    private void Awake()
    {
        CleanDisplay();
        SetInputs();
        SetButtons();   
    }

    private void OnEnable()
    {
        inputMap.Enable();

        navigateAction.performed += OnNavigate;
        submitAction.performed += OnSubmit;
        cancelAction.performed += OnCancel;

        buttons[id].Select();
    }

    private void OnDisable()
    {
        inputMap.Disable();

        navigateAction.performed -= OnNavigate;
        submitAction.performed -= OnSubmit;
        cancelAction.performed -= OnCancel;
    }

    private void SetInputs()
    {
        inputMap = playerInput.actions.FindActionMap(BOMB);
        navigateAction = inputMap.FindAction(NAVIGATE);
        submitAction = inputMap.FindAction(SUBMIT);
        cancelAction = inputMap.FindAction(CANCEL);
    }

    private void SetButtons()
    {
        for (int i = 0; i < BUTTON_COUNT; i++)
        {
            int valueButton = i + 1;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => UpdateDisplay(valueButton));
            
        }

        buttons[9].onClick.RemoveAllListeners();
        buttons[10].onClick.RemoveAllListeners();
        buttons[11].onClick.RemoveAllListeners();

        
        buttons[9].onClick.AddListener(() => CleanDisplay());
        buttons[10].onClick.AddListener(() => UpdateDisplay(0));
        buttons[11].onClick.AddListener(() => SubmitResult());
    }

    private void OnNavigate(InputAction.CallbackContext context)
    {
        Debug.Log("OnNavigate: " + context.ReadValue<Vector2>());
        Vector2 direction = context.ReadValue<Vector2>();

        int line = buttons.Length / column;
        int col = id % column;
        int lin = id / column;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0.5f) col = (col + 1) % column;                // right
            else if (direction.x < -0.5f) col = (col - 1 + column) % column; // left
            else return;
        }
        else
        {
            if (direction.y > 0.5f) lin = (lin - 1 + line) % line; //up
            else if (direction.y < -0.5f) lin = (lin + 1) % line; //down
            else return;
        }

        id = lin * column + col;
        buttons[id].Select();
    }

    private void OnSubmit(InputAction.CallbackContext context) => buttons[id].OnSubmit(null);
    private void OnCancel(InputAction.CallbackContext context) => OnDisable();


    private void UpdateDisplay(int value)
    {
        if (position < displayTexts.Length)
        {
            displayTexts[position].text = value.ToString();
            position++;
        }
    }

    private void CleanDisplay()
    {
        for (int i = 0; i < displayTexts.Length; i++)
        {
            displayTexts[i].text = "";
        }
        position = 0;
    }

    private void SubmitResult()
    {
        string result = "";
        for (int i = 0; i < displayTexts.Length; i++)
        {
            result += displayTexts[i].text;
        }
        Debug.Log("Submit Result: " + result);
    }
}
