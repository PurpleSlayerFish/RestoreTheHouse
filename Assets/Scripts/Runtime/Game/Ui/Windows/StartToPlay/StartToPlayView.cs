using CustomSelectables;
using Runtime.Services.CommonPlayerData.Data;
using SimpleUi.Abstracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Game.Ui.Windows.StartToPlay 
{
    public class StartToPlayView : UiView 
    {
        public CustomButton StartToPlay;
        public CustomButton PiranhaProgression;
        public TMP_Text PiranhaProgressionPrice;
        public TMP_Text PiranhaNx;
        public Image PiranhaPriceImg;
        public TMP_Text PiranhaMax;
        public Image PiranhaBackImg;
        
        public CustomButton MeatProgression;
        public TMP_Text MeatProgressionPrice;
        public TMP_Text MeatNx;
        public Image MeatPriceImg;
        public TMP_Text MeatMax;
        public Image MeatBackImg;
        
        public TMP_Text TotalMeat;

        private Color grey = new Color(0.85f, 0.85f, 0.85f, 1f);

        public void UpdateUi(CommonPlayerData playerData, int price, int maxProgression, int meatForEachProgression)
        {
            TotalMeat.text = playerData.Coins.ToString();
            
            if (price > playerData.Coins)
            {
                PiranhaBackImg.color = grey;
                MeatBackImg.color = grey;
            }
            else
            {
                PiranhaBackImg.color = Color.white;
                MeatBackImg.color = Color.white;
            }
            
            if (playerData.PiranhasProgression < maxProgression)
            {
                PiranhaProgressionPrice.text = price.ToString();
            }
            else
            {
                PiranhaPriceImg.enabled = false;
                PiranhaProgressionPrice.enabled = false;
                PiranhaMax.enabled = true;
            }
            if (playerData.MeatProgression < maxProgression * meatForEachProgression)
            {
                MeatProgressionPrice.text = price.ToString();
            }
            else
            {
                MeatPriceImg.enabled = false;
                MeatProgressionPrice.enabled = false;
                MeatMax.enabled = true;
            }
            PiranhaNx.text = "\u00D7 " + playerData.PiranhasProgression;
            MeatNx.text = "+ " + playerData.MeatProgression + "%";
        }
    }
}