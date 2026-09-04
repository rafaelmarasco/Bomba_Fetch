using System;
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
    private CinemachineSplineDolly bombSplineDolly;

    private void Awake()
    {
        bombSplineDolly = bombCamera.GetComponent<CinemachineSplineDolly>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            activeCamera = Cameras.bomb;
            UpdateCamera();
        }
    }

    private IEnumerator MoveAlongSpline()
    {
        bombSplineDolly.CameraPosition = 0f;
        float timePassed = 0f;
        float duration = .2f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            bombSplineDolly.CameraPosition = Mathf.Lerp(0f, 1f, timePassed/duration);
            yield return null;
        }
    }

    private void UpdateCamera()
    {
        switch (activeCamera)
        {
            case Cameras.main:
                bombCamera.Priority = 0;
                mainCamera.Priority = 10;
                break;
            case Cameras.bomb:
                mainCamera.Priority = 0;
                bombCamera.Priority = 10;
                StartCoroutine(MoveAlongSpline());
                break;
        }
    }
}
