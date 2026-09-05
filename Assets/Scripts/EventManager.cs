using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<EventManager>();
            return _instance;
        }
    }

    private void Awake() => _instance = this;



    public event Action OnBombInteracted;
    public void BombInteracted() => OnBombInteracted?.Invoke();

    public event Action OnItemPickedUp;
    public void ItemPickedUp() => OnItemPickedUp?.Invoke();

    public event Action OnItemDroped;
    public void ItemDroped() => OnItemDroped?.Invoke();

    public event Action OnPropPush;
    public void PropPush() => OnPropPush?.Invoke();
}
