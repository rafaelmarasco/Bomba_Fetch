using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    private enum Cameras // REMOVER ???????????????
    {
        main,
        bomb
    }

    [SerializeField] private CinemachineCamera mainCamera;

    public void MakeCameraFollow(PlayerInput input)
    {
        if (mainCamera.Target.TrackingTarget == null)
            mainCamera.Target.TrackingTarget = input.gameObject.GetComponentInChildren<Transform>();
    }

    /*
    private IEnumerator MoveCameraToBomb()
    {
        Debug.Log("Começou a mover a camera");

        Vector3 bombFowardOffSet = bomb.forward * .65f;
        Vector3 downOffSet = Vector3.down * .25f;
        //Vector3 fwdOffSet = bomb.right * -.3f;

        Vector3 startPos = mainCamera.transform.position;
        Vector3 finalPos = bomb.position + bombFowardOffSet + downOffSet; //+ fwdOffSet;


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
    */
}
