using UnityEngine;

public class Hazzard : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "GFX")
        {
            Debug.Log("O player Colidiu");
            EventManager.Instance.Eletrocute();
         
        }
    }
}
