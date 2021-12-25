using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Fader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float faderSpeed;
        private Coroutine faderCor;

        public void StartFader(float value, Action action = null)
        {
            if (faderCor != null) StopCoroutine(faderCor);
            faderCor = StartCoroutine(AlphaFader(value, action));
        }

        private IEnumerator AlphaFader(float value, Action action)
        {
            var t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * faderSpeed;
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, value, t);
                yield return null;
            }

            action?.Invoke();
            faderCor = null;
        }
    }
}
