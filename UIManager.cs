using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Fader blackoutFader;
        [SerializeField] private Text levelText;

        [SerializeField] private Scrollbar sbar;
        [SerializeField] private Color highHPColor;
        [SerializeField] private Color poorHPColor;

        private bool isChangeHP = true;
        public Fader BlackoutFader => blackoutFader;

        private void Start()
        {
            ChangeHP(100f);
        }

        public void OnReset()
        {
            sbar.size = 1f;
            SetScrollbarColor();
            isChangeHP = true;
        }

        public void ShowLevelText(bool isShow)
        {
            levelText.gameObject.SetActive(isShow);
        }

        public void SetLevelText(string value)
        {
            levelText.text = value;
        }

        public void ChangeHP(float value)
        {
            if (!isChangeHP) return;
            
            var percent = value / 100f;
            sbar.size += percent;
            SetScrollbarColor();

            if (sbar.size <= 0f)
            {
                isChangeHP = false;
                gameManager.GameOver();
            }
        }

        private void SetScrollbarColor()
        {
            var colors = sbar.colors;
            colors.normalColor = Color.Lerp(poorHPColor, highHPColor, sbar.size);
            sbar.colors = colors;
        }
    }
}
