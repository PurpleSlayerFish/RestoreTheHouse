using System;
using System.Diagnostics.CodeAnalysis;
using CustomSelectables;
using DG.Tweening;
using SimpleUi.Abstracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Game.Ui.Windows.LevelComplete 
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class LevelCompleteView : UiView 
    {
        [SerializeField] private CanvasGroup _rootCanvasGroup;
        [SerializeField] private RectTransform _targetPos;
        [SerializeField] private float _coinRewardAddedDuration;
        [SerializeField] private Image _coinRewardAddedImage;
        [SerializeField] private TMP_Text _coinReward;
        [SerializeField] private TMP_Text _levelN;
        
        [SerializeField] public RectTransform TotalCoinIcon;        
        [SerializeField] public TMP_Text TotalCoinCount;
        [SerializeField] public CustomButton NextLevel;
        [SerializeField] public CustomButton MainMenu;
        
        private static Vector3 WorldPosition(RectTransform rectTransform) => rectTransform.TransformPoint(rectTransform.rect.center);
        
        public void Show(int coinReward, int currentCoins, EScene currentLevel , Action onComplete = null)
        {
            gameObject.SetActive(true);
            _rootCanvasGroup.DOFade(1, 0.2f).OnComplete(()=>onComplete?.Invoke());
            _coinReward.text = "+" + coinReward;
            TotalCoinCount.text = currentCoins.ToString();
            _levelN.text = Enum.GetName(typeof(EScene), currentLevel)?.Replace("_", " ");
        }
        
        public void AddCoins(Vector2 screenPosition, int newValue)
        {
            var pos = WorldPosition(_targetPos);

            var coinEffect = Instantiate(_coinRewardAddedImage, transform);
            var rectTransform = (RectTransform)coinEffect.transform;
            coinEffect.DOFade(0, 0);
            rectTransform.DOAnchorPos(screenPosition, 0);
            DOTween.Sequence()
                .Append(coinEffect.DOFade(1, 0.4f))
                .Append(rectTransform.DOMove(pos, _coinRewardAddedDuration))
                .OnComplete(() =>
                {
                    Destroy(coinEffect.gameObject);
                    TotalCoinCount.text = newValue.ToString();
                });
        }
    }
}