using System;
using System.Diagnostics.CodeAnalysis;
using CustomSelectables;
using Leopotam.Ecs;
using SimpleUi.Abstracts;
using TMPro;
using UnityEngine;

namespace Runtime.Game.Ui.Windows.InGameButtons 
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class InGameButtonsView : UiView 
    {
        [SerializeField] private TMP_Text _levelN;
        [SerializeField] private TMP_Text _meatCount;
        [SerializeField] public CustomButton InGameMenuButton;
        
        public void Show(ref EScene currentLevel, ref EcsWorld _world)
        {
            _levelN.text = Enum.GetName(typeof(EScene), currentLevel)?.Replace("_", " ");
        }

        public void UpdateMeat(ref int impact)
        {
            _meatCount.text = impact.ToString();
        }
    }
}