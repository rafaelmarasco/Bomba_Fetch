using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    private void Awake() => Instance = this;

    public event Action OnBombInteracted;
    public void BombInteracted() => OnBombInteracted?.Invoke();
}
