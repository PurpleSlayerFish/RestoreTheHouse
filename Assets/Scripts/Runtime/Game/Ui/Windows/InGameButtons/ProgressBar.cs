using DG.Tweening;
using UnityEngine;
using Utils.UiExtensions;

namespace Runtime.Game.Ui.Windows.InGameButtons
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private SlicedFilledImage progress;

        public void SetFillAmount(float ratio)
        {
            progress.fillAmount = ratio;
        }
        
        public void Repaint(float ratio, Color color)
        {
            progress.DOKill();
            progress.DOFillAmount(Mathf.Abs(ratio), 0.1f);
            progress.DOColor(color, 0.05f);
        }
    }
}