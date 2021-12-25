using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class LevelInfo
    {
        public int LevelIndex;
        public int ColumnsAmount;
        public int RowsAmount;
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Prewiew prewiew;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private LevelGeneration levelGeneration;
        [SerializeField] private EnemySpawner enemySpawner;
        [SerializeField] private PlayerControl player;
        [SerializeField] private List<LevelInfo> levelsInfo;

        private Coroutine timerCor;
        private int currentLevel;
        private LevelInfo levelInfo;

        private void Awake()
        {
            prewiew.StartGameAction = StartGame;
        }

        private LevelInfo GetLevelInfo()
        {
            return levelsInfo.Find(info => info.LevelIndex == currentLevel);
        }

        public void StartGame()
        {
            ++currentLevel;
            levelInfo = GetLevelInfo();
            if (levelInfo == null)
            {
                GameOver();
                return;
            }

            uiManager.ShowLevelText(true);
            uiManager.SetLevelText($"Уровень {currentLevel}");
            uiManager.BlackoutFader.StartFader(1f, () =>
            {
                NextLevel();
                StartTimer(0.25f, () => uiManager.BlackoutFader.StartFader(0f));
            });
        }

        private void NextLevel()
        {
            player.PlaceInCenter();
            levelGeneration.Level = currentLevel;

            levelGeneration.GenerateMap(3, levelInfo.ColumnsAmount, levelInfo.RowsAmount);
            enemySpawner.CreateEnemy(levelGeneration.GetEnemyTile());
            player.PlaceInCenter();
        }

        public void GameOver()
        {
            uiManager.ShowLevelText(true);
            uiManager.SetLevelText($"Конец игры");
            uiManager.BlackoutFader.StartFader(1f, () =>
            {
                currentLevel = 1;
                levelInfo = GetLevelInfo();
                NextLevel();

                uiManager.OnReset();
                StartTimer(0.25f, () => uiManager.BlackoutFader.StartFader(0f));
            });
        }

        private IEnumerator Timer(float value, Action action)
        {
            var t = 0f;
            while (t <= value)
            {
                t += Time.deltaTime;
                yield return null;
            }

            action?.Invoke();
            timerCor = null;
        }

        private void StartTimer(float value, Action action)
        {
            if (timerCor != null) StopCoroutine(timerCor);
            timerCor = StartCoroutine(Timer(value, action));
        }
    }
}
