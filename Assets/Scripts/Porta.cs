using UnityEngine;

// Vai no objeto PAI (vazio, posicionado na dobradica).
// O PAI gira; o FILHO (mesh) so acompanha.
public class Porta : MonoBehaviour
{
    [SerializeField] Transform filho;            // a mesh da porta
    [SerializeField] float maxAngle = 90f;       // quanto abre
    [SerializeField] float openSpeed = 360f;     // graus/s ao empurrar
    [SerializeField] float closeSpeed = 180f;    // graus/s ao voltar
    [SerializeField] float alcanceFrente = 1.2f; // distancia em que comeca a empurrar
    [SerializeField] float alcanceLado = 1.0f;   // meia largura da passagem
    [SerializeField] Vector3 ajusteCentro;       // empurra a area de deteccao, se precisar
    [SerializeField] bool bloquearQuandoFechada = true;
    [SerializeField] Collider colisor;           // collider do filho (nao trigger)

    Transform player;
    Vector3 centro;         // centro da passagem, capturado com a porta fechada
    Vector3 frenteFechada;  // eixo que atravessa a passagem, nao gira junto
    Vector3 ladoFechado;    // eixo da largura da passagem, idem
    Quaternion rotFechada;  // rotacao original do Pai, definida no editor
    float currentAngle;
    float sinal;            // +1 ou -1, travado enquanto o jogador esta perto
    bool jogadorPerto;

    void Awake()
    {
        rotFechada = transform.localRotation;
        frenteFechada = transform.forward;
        ladoFechado = transform.right;
        centro = CalcularCentro();

        if (colisor == null && filho != null) colisor = filho.GetComponentInChildren<Collider>();

        // acha pelo componente Player em vez de tag: o prefab do player
        // esta Untagged, so tem a LAYER Player configurada
        Player p = FindFirstObjectByType<Player>();
        if (p != null) player = p.transform;
        else Debug.LogWarning("Porta: nenhum Player encontrado na cena.", this);
    }

    // centro visual da folha: usa os bounds do renderer, porque a origem
    // do transform da mesh pode estar em qualquer canto do modelo
    Vector3 CalcularCentro()
    {
        Vector3 c = transform.position;

        if (filho != null)
        {
            Renderer r = filho.GetComponentInChildren<Renderer>();
            c = r != null ? r.bounds.center : filho.position;
        }

        return c + transform.rotation * ajusteCentro;
    }

    void Update()
    {
        float alvo = 0f;

        if (player != null)
        {
            Vector3 offset = player.position - centro;
            float dFrente = Vector3.Dot(offset, frenteFechada);
            float dLado = Vector3.Dot(offset, ladoFechado);

            bool dentro = Mathf.Abs(dFrente) < alcanceFrente && Mathf.Abs(dLado) < alcanceLado;

            if (dentro)
            {
                // trava o lado na primeira deteccao: abre para longe do jogador
                // e nao inverte quando ele cruza o plano da porta
                if (!jogadorPerto)
                {
                    sinal = dFrente > 0f ? -1f : 1f;
                    jogadorPerto = true;
                }

                float t = Mathf.SmoothStep(0f, 1f,
                    Mathf.InverseLerp(alcanceFrente, 0f, Mathf.Abs(dFrente)));
                alvo = maxAngle * sinal * t;
            }
            else
            {
                jogadorPerto = false;
            }
        }

        // o Player anda por raycast contra a layer Walls e trava a 0.8 de distancia.
        // enquanto ele esta na zona de empurrar a porta para de bloquear, senao
        // ele nunca chegaria perto o bastante para abri-la.
        if (bloquearQuandoFechada && colisor != null)
            colisor.enabled = !jogadorPerto;

        float vel = Mathf.Abs(alvo) > Mathf.Abs(currentAngle) ? openSpeed : closeSpeed;
        currentAngle = Mathf.MoveTowards(currentAngle, alvo, vel * Time.deltaTime);

        // gira A PARTIR da rotacao do editor, senao a porta salta para o eixo do mundo
        transform.localRotation = rotFechada * Quaternion.Euler(0f, currentAngle, 0f);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 c = Application.isPlaying ? centro : CalcularCentro();
        Vector3 f = Application.isPlaying ? frenteFechada : transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.matrix = Matrix4x4.TRS(c, Quaternion.LookRotation(f, Vector3.up), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(alcanceLado * 2f, 2f, alcanceFrente * 2f));
    }
}
