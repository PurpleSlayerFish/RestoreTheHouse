using System;
using System.Diagnostics.CodeAnalysis;
using CustomSelectables;
using SimpleUi.Abstracts;
using TMPro;
using UnityEngine;

namespace Runtime.Game.Ui.Windows.LevelComplete 
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class LevelCompleteView : UiView 
    {
        [SerializeField] private CanvasGroup _rootCanvasGroup;
        [SerializeField] private TMP_Text _levelN;
        
        [SerializeField] public CustomButton NextLevel;
        
        public void Show(EScene currentLevel)
        {
            gameObject.SetActive(true);
            _levelN.text = Enum.GetName(typeof(EScene), currentLevel)?.Replace("_", " ");
        }
    }
}