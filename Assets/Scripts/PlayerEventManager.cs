using System;
using UnityEngine;

public class PlayerEventManager : MonoBehaviour
{
    public event Action<Transform, GameObject> OnBombInteracted;
    public void BombInteracted(Transform cameraPos, GameObject prop) => OnBombInteracted?.Invoke(cameraPos, prop);

    public event Action<Prop> OnItemPickedUp;
    public void ItemPickedUp(Prop prop) => OnItemPickedUp?.Invoke(prop);

    public event Action<Prop> OnItemDroped;
    public void ItemDroped(Prop prop) => OnItemDroped?.Invoke(prop);

    public event Action OnBombDroped;
    public void BombDroped() => OnBombDroped?.Invoke();

    public event Action OnPropPush;
    public void PropPush() => OnPropPush?.Invoke();

    public event Action<Vector3, Vector3, float, float> OnEletrocuted;
    public void Eletrocute(Vector3 flyDirection, Vector3 propFlyDirection, float flyForce, float stunTime) 
        => OnEletrocuted?.Invoke(flyDirection, propFlyDirection, flyForce, stunTime);

    public event Action<float> OnKnockDown;
    public void KnockedDown(float stunTime) => OnKnockDown?.Invoke(stunTime);

    public event Action<bool> OnStopedMoving;
    public void StopMoving(bool stopMoving) => OnStopedMoving?.Invoke(stopMoving);
}
