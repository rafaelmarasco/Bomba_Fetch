using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private enum Cameras
    {
        main,
        bomb
    }

    [SerializeField] private Cameras activeCamera = Cameras.main;
    [SerializeField] private CinemachineCamera mainCamera;
    [SerializeField] private CinemachineCamera bombCamera;
    [SerializeField] private Transform bomb;
    [SerializeField] private GameObject head;

    [SerializeField] private PropInteract propInteract;

    private float fwdOffset = .7f;
    private float downOffset = .2f;

    private void OnEnable()
    {
        EventManager.Instance.OnBombInteracted += UpdateCamera;
        EventManager.Instance.OnItemDroped += UpdateCamera;
    }

    private IEnumerator MoveCameraToBomb()
    {
        Debug.Log("Começou a mover a camera");

        Vector3 bombFowardOffSet = bomb.forward * fwdOffset;
        Vector3 wordDownOffSet = Vector3.down * downOffset;
        //Vector3 fwdOffSet = bomb.right * -.3f;

        Vector3 startPos = mainCamera.transform.position;
        Vector3 finalPos = bomb.position + bombFowardOffSet + wordDownOffSet; //+ fwdOffSet;


        float timePassed = 0f;
        float duration = .2f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;

            bombCamera.transform.position = Vector3.Lerp(startPos, finalPos, timePassed / duration);

            Vector3 bombDir = bomb.position - bombCamera.transform.position;
            bombCamera.transform.rotation = Quaternion.LookRotation(bombDir, bomb.up);
            yield return null;
        }
        bombCamera.transform.position = finalPos;
        bombCamera.transform.rotation = Quaternion.LookRotation(bomb.position - finalPos, bomb.up);
        head.SetActive(false);
    }

    private void UpdateCamera()
    {
        activeCamera = propInteract.hasBomb ? Cameras.bomb : Cameras.main;

        switch (activeCamera)
        {
            case Cameras.main:
                bombCamera.gameObject.SetActive(false);
                mainCamera.gameObject.SetActive(true);
                break;
            case Cameras.bomb:
                mainCamera.gameObject.SetActive(false);
                bombCamera.gameObject.SetActive(true);
                StartCoroutine(MoveCameraToBomb());
                break;
        }
    }
}
