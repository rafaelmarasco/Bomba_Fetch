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
    [SerializeField] private PropInteract propInteract;
    private CinemachineSplineDolly bombSplineDolly;
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        bombSplineDolly = bombCamera.GetComponent<CinemachineSplineDolly>();
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
        inputActions.Player.Grab.performed += Grab_performed;
    }

    private void Grab_performed(InputAction.CallbackContext context)
    {
        UpdateCamera();
    }

    private IEnumerator MoveAlongSpline()
    {
        bombSplineDolly.CameraPosition = 0f;
        float timePassed = 0f;
        float duration = .2f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            bombSplineDolly.CameraPosition = Mathf.Lerp(0f, 1f, timePassed / duration);
            yield return null;
        }
    }

    private void UpdateCamera()
    {
        activeCamera = propInteract.hasBomb == true ? Cameras.bomb : Cameras.main;

        switch (activeCamera)
        {
            case Cameras.main:
                bombCamera.gameObject.SetActive(false);
                mainCamera.gameObject.SetActive(true);
                break;
            case Cameras.bomb:
                mainCamera.gameObject.SetActive(false);
                bombCamera.gameObject.SetActive(true);
                StartCoroutine(MoveAlongSpline());
                break;
        }
    }
}
