#if UNITY_EDITOR
// ============================================================
//  TrailerFlyCam.cs
//  Câmera livre estilo "fly cam" para o Modo Trailer.
//
//  Controles (só enquanto o Modo Trailer estiver ativo):
//    Mouse         — olhar ao redor
//    WASD          — mover para frente/trás/esquerda/direita
//    Q / E         — descer / subir
//    Shift         — multiplicar velocidade
//    Scroll wheel  — ajustar velocidade base
// ============================================================

using UnityEngine;

[AddComponentMenu("_Trailer/Trailer Fly Cam")]
[RequireComponent(typeof(Camera))]
public class TrailerFlyCam : MonoBehaviour
{
    [Header("Velocidade")]
    [SerializeField] private float baseSpeed = 10f;
    [SerializeField] private float shiftMultiplier = 3f;
    [SerializeField] private float scrollSpeedStep = 1f;
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 50f;

    [Header("Suavização")]
    [SerializeField] private float moveSmoothTime = 0.08f;
    [SerializeField] private float lookSensitivity = 2f;

    // ---- Estado ----
    private Vector3 velocity = Vector3.zero;
    private Vector3 smoothVelocity = Vector3.zero;
    private float yaw;
    private float pitch;

    private void OnEnable()
    {
        // Inicializa rotação a partir da rotação atual da câmera
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
        HandleSpeedScroll();
    }

    // ------------------------------------------------------------------ //
    //  Olhar
    // ------------------------------------------------------------------ //

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    // ------------------------------------------------------------------ //
    //  Movimento
    // ------------------------------------------------------------------ //

    private void HandleMovement()
    {
        // Usa unscaledDeltaTime para funcionar mesmo com timeScale == 0 (F4 pausa)
        float dt = Time.unscaledDeltaTime;

        float speed = baseSpeed * (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)
            ? shiftMultiplier
            : 1f);

        Vector3 input = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) input += transform.forward;
        if (Input.GetKey(KeyCode.S)) input -= transform.forward;
        if (Input.GetKey(KeyCode.A)) input -= transform.right;
        if (Input.GetKey(KeyCode.D)) input += transform.right;
        if (Input.GetKey(KeyCode.E)) input += Vector3.up;
        if (Input.GetKey(KeyCode.Q)) input -= Vector3.up;

        Vector3 targetVelocity = input.normalized * speed;
        smoothVelocity = Vector3.SmoothDamp(smoothVelocity, targetVelocity, ref velocity, moveSmoothTime, Mathf.Infinity, dt);

        transform.position += smoothVelocity * dt;
    }

    // ------------------------------------------------------------------ //
    //  Ajuste de velocidade com scroll
    // ------------------------------------------------------------------ //

    private void HandleSpeedScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            baseSpeed = Mathf.Clamp(baseSpeed + scroll * scrollSpeedStep * 10f, minSpeed, maxSpeed);
        }
    }
}
#endif
