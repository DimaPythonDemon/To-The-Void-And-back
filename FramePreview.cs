using Assets.Scripts;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Fader))]
public class FramePreview : MonoBehaviour
{
    [SerializeField] private RectTransform texTransform;
    [SerializeField] private Vector3 textMoveStart;
    [SerializeField] private Vector3 textMoveFinish;
    [SerializeField] private float scrollSpeed;
    
    private Coroutine scrollTextCor;
    private CanvasGroup canvasGroup;
    private Fader fader;

    public Action StopAction;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        fader = GetComponent<Fader>();
        Hide();
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void Init()
    {
        if (texTransform)
        {
            texTransform.localPosition = textMoveStart;
        }

        fader.StartFader(1f, ()=> ScrollText(textMoveFinish, StopAction));
    }

    private IEnumerator ScrollTextCor(Vector3 target, Action action)
    {
        while (texTransform.localPosition != target)
        {
            var delta = Time.deltaTime * scrollSpeed;
            texTransform.localPosition = Vector3.MoveTowards(texTransform.localPosition, target, delta);
            yield return null;
        }

        action?.Invoke();
        scrollTextCor = null;
    }

    private void ScrollText(Vector3 target, Action action = null)
    {
        if (!texTransform)
        {
            StopAction?.Invoke();
            return;
        }

        if(scrollTextCor != null) StopCoroutine(scrollTextCor);
        scrollTextCor = StartCoroutine(ScrollTextCor(target, action));
    }
}
