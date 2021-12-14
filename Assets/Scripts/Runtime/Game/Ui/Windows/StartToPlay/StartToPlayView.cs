using CustomSelectables;
using Runtime.Services.CommonPlayerData;
using Runtime.Services.CommonPlayerData.Data;
using SimpleUi.Abstracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runtime.Game.Ui.Windows.StartToPlay
{
    public class StartToPlayView : UiView
    {
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;

        public CustomButton StartToPlay;
        
        public CustomButton FirstProgression;
        public TMP_Text FirstProgressionPrice;
        public TMP_Text FirstModif;
        public Image FirstPriceImg;
        public TMP_Text FirstMax;
        public Image FirstBackImg;

        public CustomButton SecondProgression;
        public TMP_Text SecondProgressionPrice;
        public TMP_Text SecondModif;
        public Image SecondPriceImg;
        public TMP_Text SecondMax;
        public Image SecondBackImg;

        public TMP_Text TotalMeat;

        private Color _grey = new Color(0.196f, 0.259f, 0.443f, 1f);
        private Color _normal = new Color(0.274f, 0.396f, 0.755f, 1f);

        public void UpdateUi()
        {
            var playerData = _commonPlayerData.GetData();
            TotalMeat.text = playerData.Coins.ToString();

            var fireRatePrice = playerData.GetNextFireRatePrice();
            var tilesPrice = playerData.GetNextTilesPrice();

            if (fireRatePrice > playerData.Coins || playerData.FireRateProgression >= playerData.FireRateMaxProgression)
                FirstBackImg.color = _grey;
            else
                FirstBackImg.color = _normal;
            if (tilesPrice > playerData.Coins || playerData.TilesProgression >= playerData.TilesMaxProgression)
                SecondBackImg.color = _grey;
            else
                SecondBackImg.color = _normal;

            if (playerData.FireRateProgression < playerData.FireRateMaxProgression)
                FirstProgressionPrice.text = fireRatePrice.ToString();
            else
            {
                FirstPriceImg.enabled = false;
                FirstProgressionPrice.enabled = false;
                FirstMax.enabled = true;
            }

            if (playerData.TilesProgression < playerData.TilesMaxProgression)
                SecondProgressionPrice.text = tilesPrice.ToString();
            else
            {
                SecondPriceImg.enabled = false;
                SecondProgressionPrice.enabled = false;
                SecondMax.enabled = true;
            }

            FirstModif.text = "+ " + playerData.FireRateForEachProgression;
            SecondModif.text = "+ " + playerData.TilesForEachProgression;
        }
    }
}