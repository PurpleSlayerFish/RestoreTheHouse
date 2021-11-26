using System;
using System.Diagnostics.CodeAnalysis;
using CustomSelectables;
using SimpleUi.Abstracts;
using TMPro;
using UnityEngine;

namespace Runtime.Game.Ui.Windows.GameOver 
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GameOverView : UiView 
    {
        [SerializeField] private TMP_Text _levelN;
        
        [SerializeField] public CustomButton Restart;
        [SerializeField] public CustomButton MainMenu;
        
        public void Show(EScene currentLevel)
        {
            _levelN.text = Enum.GetName(typeof(EScene), currentLevel)?.Replace("_", " ");
        }
    }
}