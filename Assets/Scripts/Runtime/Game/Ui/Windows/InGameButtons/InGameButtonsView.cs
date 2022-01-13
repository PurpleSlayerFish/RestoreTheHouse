using System;
using System.Diagnostics.CodeAnalysis;
using CustomSelectables;
using Leopotam.Ecs;
using Runtime.Signals;
using SimpleUi.Abstracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Game.Ui.Windows.InGameButtons 
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class InGameButtonsView : UiView 
    {
        [SerializeField] private TMP_Text _levelN;
        [SerializeField] private TMP_Text _currencyCount;
        [SerializeField] public CustomButton InGameMenuButton;
        [SerializeField] public Image JoystickButton;
        [SerializeField] public Image JoystickOrigin;

        private RectTransform _joystickButtonRT;
        private RectTransform _joystickOriginRT;
        
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
    }
}