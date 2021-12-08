using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Runtime.Signals;
using Runtime.UI.QuitConcentPopUp;
using Signals;
using SimpleUi.Signals;
using UnityEngine;
using Zenject;

namespace Utils.UiExtensions
{
    public static partial class UiExtensions
    {
        public static void OpenQuestionPopUp(this SignalBus signalBus, string title, Action<bool> action)
        {
            signalBus.Fire(new SignalQuestionChoice(title, action));
            signalBus.OpenWindow<ConsentWindow>();
        }
        
        public static TweenerCore<float, float, FloatOptions> DOFillAmount(this SlicedFilledImage target, float endValue, float duration)
        {
            endValue = Mathf.Clamp(endValue, 0, 1);
            var t = DOTween.To(() => target.fillAmount, x => target.fillAmount = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
    }
}