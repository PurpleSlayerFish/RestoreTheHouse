using DG.Tweening;
using TMPro;
using UnityEngine;
using Utils.UiExtensions;

namespace Runtime.Game.Ui.Windows.InGameMenu
{
    public class ProgressBar : MonoBehaviour
    {
        [System.Serializable]
        private struct Stage
        {
            public string name;
            public Color color;
            [Range(-1, 1)]
            public float ratio;

            public Stage(string name, Color color, float ratio)
            {
                this.color = color;
                this.ratio = ratio;
                this.name = name;
            }
        }

        [SerializeField] private SlicedFilledImage progress;
        [SerializeField] private TMP_Text nameField;
        [SerializeField] private Stage[] stages;

        public void SetFillAmount(float ratio)
        {
            progress.fillAmount = ratio;
            var stage = stages.Find(x => ratio <= x.ratio);
            progress.color = stage.color;
            nameField.text = stage.name;
        }
        
        public void Repaint(float ratio, int stageIndex)
        {
            progress.DOKill();
            progress.DOFillAmount(Mathf.Abs(ratio), 0.1f);
            var stage = stages[stageIndex];
            progress.DOColor(stage.color, 0.1f);
            nameField.text = stage.name;
        }
    }
}