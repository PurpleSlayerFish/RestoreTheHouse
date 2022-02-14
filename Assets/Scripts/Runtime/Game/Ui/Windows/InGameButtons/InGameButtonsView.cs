using System;
using System.Diagnostics.CodeAnalysis;
using CustomSelectables;
using ECS.Game.Components.GameCycle;
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
        [SerializeField] private TMP_Text _levelN;
        [SerializeField] private GameObject _woodIndicator;
        [SerializeField] private GameObject _moneyIndicator;
        [SerializeField] private GameObject _concreteIndicator;
        [SerializeField] private TMP_Text _woodCountTxt;
        [SerializeField] private TMP_Text _moneyCountTxt;
        [SerializeField] private TMP_Text _concreteCountTxt;
        [SerializeField] public CustomButton InGameMenuButton;
        [SerializeField] public Image JoystickButton;
        [SerializeField] public Image JoystickOrigin;

        private int _woodCount = 0;
        private int _moneyCount = 0;
        private int _concreteCount = 0;
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