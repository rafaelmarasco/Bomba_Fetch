using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
public class ButtonTutorial : MonoBehaviour
{
    [SerializeField]
    private Porta doorToOpen;

    [SerializeField]
    private Prop keyProp;

    private Animator animator;

    private bool isPressed = false;

    private const string IS_ON = "isOn";

    private void Start()
    {
        doorToOpen.Aberta = false;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isPressed)
            doorToOpen.Aberta = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == keyProp.gameObject)
        {
            isPressed = true;
            doorToOpen.Aberta = true;
            animator.SetBool(IS_ON, isPressed);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == keyProp.gameObject)
        {
            isPressed = false;
            doorToOpen.Aberta = false;
            animator.SetBool(IS_ON, isPressed);
        }
    }
}
