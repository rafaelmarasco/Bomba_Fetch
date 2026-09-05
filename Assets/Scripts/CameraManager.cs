using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


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

    [SerializeField] private PropInteract propInteract;

    private void OnEnable()
    {
        EventManager.Instance.OnBombInteracted += UpdateCamera;
        EventManager.Instance.OnItemDroped += UpdateCamera;
    }

    private IEnumerator MoveCameraToBomb()
    {
        float offSet = 1f;

        Vector3 startPos = mainCamera.transform.position;
        Vector3 finalPos = bomb.position + Vector3.up * offSet;

        float timePassed = 0f;
        float duration = .2f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            bombCamera.transform.position = Vector3.Lerp(startPos, finalPos, timePassed / duration);
            bombCamera.transform.LookAt(bomb.position);
            yield return null;
        }
        bombCamera.transform.position = finalPos;
        bombCamera.transform.LookAt(bomb.position);
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
