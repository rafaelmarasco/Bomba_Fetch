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

    public event Action<Prop> OnItemPickedUp;
    public void ItemPickedUp(Prop prop) => OnItemPickedUp?.Invoke(prop);

    public event Action<Prop> OnItemDroped;
    public void ItemDroped(Prop prop) => OnItemDroped?.Invoke(prop);

    public event Action OnBombDroped;
    public void BombDroped() => OnBombDroped?.Invoke();

    public event Action OnPropPush;
    public void PropPush() => OnPropPush?.Invoke();
}
