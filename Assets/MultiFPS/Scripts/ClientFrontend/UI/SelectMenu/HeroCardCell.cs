using MultiFPS.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiFPS.UI
{
    public class HeroCardCell : MonoBehaviour
    {
        [SerializeField] private GameObject parentObj;
        [SerializeField] private Image imgIcon;
        [SerializeField] private Image imgClass;
        [SerializeField] private TextMeshProUGUI tmpPlayerName;
        [SerializeField] private TextMeshProUGUI tmpHeroName;
        [SerializeField] private Image imgHealthBar;

        public bool isMySlot;

        public void SetMyHero(CellElement cell)
        {
            if (cell != null)
            {
                imgIcon.sprite = SpriteManager.Instance.GetIconHero(cell.HeroData.assetIdx);
                imgIcon.gameObject.SetActive(true);
                if (imgClass != null)
                    imgClass.sprite = SpriteManager.Instance.GetIconHeroClass(cell.HeroData.assetIdx);
                tmpHeroName.text = cell.HeroData.name;
                tmpPlayerName.text = UserSettings.UserNickname;
            }
        }

        public void WriteData(PlayerInstance player)
        {

        }
    }
}