using System;
using System.Diagnostics.CodeAnalysis;
using CustomSelectables;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Utils.Extensions;
using Leopotam.Ecs;
using Runtime.Game.Ui.Windows.InGameMenu;
using SimpleUi.Abstracts;
using TMPro;
using UnityEngine;

namespace Runtime.Game.Ui.Windows.InGameButtons 
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class InGameButtonsView : UiView 
    {
        [SerializeField] private TMP_Text _levelN;

        [SerializeField] public CustomButton InGameMenuButton;
        
        public void Show(EScene currentLevel, EcsWorld _world)
        {
            _levelN.text = Enum.GetName(typeof(EScene), currentLevel)?.Replace("_", " ");
        }
    }
}