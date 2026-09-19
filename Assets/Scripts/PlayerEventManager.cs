using System;
using UnityEngine;

public class PlayerEventManager : MonoBehaviour
{
    public event Action<Transform, GameObject> OnBombInteracted;
    public void BombInteracted(Transform position, GameObject prop) => OnBombInteracted?.Invoke(position, prop);

    public event Action<Prop> OnItemPickedUp;
    public void ItemPickedUp(Prop prop) => OnItemPickedUp?.Invoke(prop);

    public event Action<Prop> OnItemDroped;
    public void ItemDroped(Prop prop) => OnItemDroped?.Invoke(prop);

    public event Action OnBombDroped;
    public void BombDroped() => OnBombDroped?.Invoke();

    public event Action OnPropPush;
    public void PropPush() => OnPropPush?.Invoke();
}
