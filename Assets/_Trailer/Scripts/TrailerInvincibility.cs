#if UNITY_EDITOR
// ============================================================
//  TrailerInvincibility.cs
//  Bloqueia eventos de dano em todos os PlayerHarmHandlers
//  da cena enquanto o Modo Trailer está ativo.
//
//  NÃO altera nenhum script existente do jogo.
//  Intercepta os eventos OnEletrocuted e OnBurned do
//  PlayerEventManager de cada jogador encontrado na cena.
// ============================================================

using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("_Trailer/Trailer Invincibility")]
public class TrailerInvincibility : MonoBehaviour
{
    // Mantém referência para poder remover os listeners depois
    private readonly List<PlayerEventManager> registeredManagers = new();

    // ------------------------------------------------------------------ //

    public void SetInvincible(bool invincible)
    {
        if (invincible) Subscribe();
        else Unsubscribe();
    }

    // ------------------------------------------------------------------ //

    private void Subscribe()
    {
        registeredManagers.Clear();

        // Busca todos os jogadores vivos na cena
        foreach (PlayerEventManager pem in FindObjectsByType<PlayerEventManager>(FindObjectsSortMode.None))
        {
            pem.OnEletrocuted += AbsorbEletrocution;
            pem.OnBurned += AbsorbBurn;
            registeredManagers.Add(pem);
        }

        Debug.Log($"[Trailer] Invencibilidade ativada em {registeredManagers.Count} jogador(es).");
    }

    private void Unsubscribe()
    {
        foreach (PlayerEventManager pem in registeredManagers)
        {
            if (pem == null) continue;
            pem.OnEletrocuted -= AbsorbEletrocution;
            pem.OnBurned -= AbsorbBurn;
        }
        registeredManagers.Clear();
        Debug.Log("[Trailer] Invencibilidade removida.");
    }

    // ------------------------------------------------------------------ //
    //  Absorvedores — assinatura deve bater com os delegates do jogo
    // ------------------------------------------------------------------ //

    // Corresponde a: Action<Vector3, Vector3, float, float>
    private void AbsorbEletrocution(Vector3 flyDir, Vector3 propFlyDir, float force, float stunTime)
    {
        // Nada acontece — evento consumido silenciosamente
    }

    // Corresponde a: Action<Vector3, Vector3, float, float, Vector3, float>
    private void AbsorbBurn(Vector3 jumpDir, Vector3 runTarget, float jumpForce, float runTime, Vector3 firePos, float stunTime)
    {
        // Nada acontece — evento consumido silenciosamente
    }

    private void OnDisable() => Unsubscribe();
}
#endif
