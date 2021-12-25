using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QueueItem
{
    [SerializeField] private string name;
    public float PauseBeforeStart;
    public float PauseBeforeEnding;
    public FramePreview Item;
}

public class Prewiew : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Fader prewiewFader;
    [SerializeField] private float pauseBeforeStart;
    [SerializeField] private List<QueueItem> queueItems;

    private Coroutine playCor;
    public Action StartGameAction;

    private void Start()
    {
        uiManager.ShowLevelText(false);
        playCor = StartCoroutine(PlayPreview());
    }

    private void Skip()
    {
        if (playCor != null)
        {
            StopCoroutine(playCor);
            StartGameAction?.Invoke();
            Destroy(gameObject);
        }
    }

    private IEnumerator PlayPreview()
    {
        var time = 0f;
        while (time <= pauseBeforeStart)
        { ;
            time += Time.deltaTime;
            yield return null;
        }

        bool isNext = false;
        int index = -1;
        QueueItem currentItem = null;

        while (++index < queueItems.Count)
        {
            if (currentItem != null)
            {
                if (currentItem.Item != null)
                {
                    currentItem.Item.gameObject.SetActive(false);
                }
            }

            isNext = false;
            currentItem = queueItems[index];
            yield return new WaitForSeconds(currentItem.PauseBeforeStart);

            if (currentItem.Item != null)
            {
                currentItem.Item.StopAction = () => isNext = true;
                currentItem.Item.gameObject.SetActive(true);
                currentItem.Item.Init();

                while (!isNext)
                {
                    yield return null;
                }
            }
 
            yield return new WaitForSeconds(currentItem.PauseBeforeEnding);
        }

        StartGameAction?.Invoke();

        prewiewFader.StartFader(0f, () =>
        {
            Destroy(gameObject);
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Skip();
        }
    }
}
