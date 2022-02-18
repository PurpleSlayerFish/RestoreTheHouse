using System;
using System.Diagnostics.CodeAnalysis;
using CustomSelectables;
using DG.Tweening;
using ECS.Utils.Extensions;
using Leopotam.Ecs;
using Runtime.Signals;
using SimpleUi.Abstracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Game.Ui.Windows.InGameButtons 
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
    public class InGameButtonsView : UiView 
    {
        [SerializeField] public Image JoystickButton;
        [SerializeField] public Image JoystickOrigin;
        [SerializeField] private TMP_Text _levelN;
        [SerializeField] public RectTransform Vignette;
        // [SerializeField] public int VignetteMultiplier;
        // [SerializeField] public int VignetteDisableValue;
        [SerializeField] public float _vignetteDuration = 0.3f;
        [SerializeField] public int _maxHp = 100;
        [SerializeField] private ProgressBar _hpBar;
        [SerializeField] public CustomButton InGameMenuButton;

        private RectTransform _joystickButtonRT;
        private RectTransform _joystickOriginRT;
        private Color color = new Color (0.965f, 0.185f,0.34f, 1f);
        
        public void Show(ref EScene currentLevel, ref EcsWorld _world)
        {
            _levelN.text = Enum.GetName(typeof(EScene), currentLevel)?.Replace("_", " ");
            _joystickButtonRT = JoystickButton.GetComponent<RectTransform>();
            _joystickOriginRT = JoystickOrigin.GetComponent<RectTransform>();
        }

        public void UpdateJoystick(ref SignalJoystickUpdate signal)
        {
            JoystickButton.gameObject.SetActive(signal.IsPressed);
            JoystickOrigin.gameObject.SetActive(signal.IsPressed);
            if (signal.IsPressed)
            {
                _joystickButtonRT.anchoredPosition = signal.ButtonPosition;
                _joystickOriginRT.anchoredPosition = signal.OriginPosition;
            }
        }
        
        public void UpdateHp(ref SignalHpUpdate signal)
        {
            // if (signal.Hp > VignetteDisableValue)
            // {
            //     Vignette.gameObject.SetActive(false);
            // }
            // else
            // {
            //     Vignette.gameObject.SetActive(true);
            //     Vignette.localScale = Vector3.one * Mathf.Clamp(signal.Hp/VignetteMultiplier, 1, 10);
            // }
            Vignette.DOKill();
            Vignette.gameObject.SetActive(true);
            Vignette.DOScale(Vector3.one * 3, _vignetteDuration).SetEase(Ease.Linear);
            Vignette.DOScale(Vector3.one * 15, _vignetteDuration).SetEase(Ease.Linear).SetDelay(_vignetteDuration)
                .OnComplete(() => Vignette.gameObject.SetActive(false));

            _hpBar.Repaint(signal.Hp.Remap01(_maxHp), color);
        }
    }
}