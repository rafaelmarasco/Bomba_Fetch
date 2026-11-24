using UnityEngine;
using PrimeTween;

public class PainelPop : MonoBehaviour
{
   [SerializeField] private float duration;
   [SerializeField] private float inicialScale;

   private void OnEnable()
   {
        Tween.StopAll(onTarget: transform);
        Tween.Scale(transform,
            startValue: Vector3.one * inicialScale,
            endValue: Vector3.one,
            duration: duration,
            ease: Ease.OutBack,
            useUnscaledTime: true);
   }
}
