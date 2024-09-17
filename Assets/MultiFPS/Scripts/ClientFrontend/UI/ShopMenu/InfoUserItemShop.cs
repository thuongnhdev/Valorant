using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MultiFPS.Gameplay;

namespace MultiFPS.UI
{
    public class InfoUserItemShop : MonoBehaviour
    {
        [SerializeField]
        private Image avaCharacter;
        [SerializeField]
        private TextMeshProUGUI tmpNameCharacter;
        [SerializeField]
        private TextMeshProUGUI tmpScore; 
        [SerializeField]
        private Image imgIconScore;
        [SerializeField]
        private Image imgGun1;
        [SerializeField]
        private Image imgGun2;
        [SerializeField]
        private Image imgAmmorInf;
        [SerializeField]
        private Image imgBackground;
        [SerializeField]
        private Sprite[] spritesBackground;
        [SerializeField]
        private Color colorIsOwned;

        public void WriteData(PlayerInstance player)
        {
            if (avaCharacter)
                avaCharacter.sprite = SpriteManager.Instance.GetIconHero(player.PlayerInfo.CharacterModelId);
            if (player.isOwned)
            {                
                imgBackground.sprite = spritesBackground[0];
                tmpScore.color = colorIsOwned;
                imgIconScore.color = colorIsOwned;
                tmpNameCharacter.gameObject.SetActive(false);
            }    
            else
            {
                tmpNameCharacter.text = player.PlayerInfo.Username;
                imgBackground.sprite = spritesBackground[1];
            }

            if (player == null || player.MyCharacter == null)
                return;

            if (player.MyCharacter.CharacterItemManager.Slots[0].Item != null)
                 imgGun1.sprite = player.MyCharacter.CharacterItemManager.Slots[0].Item.ItemIcon;
            else
                imgGun1.gameObject.SetActive(false);

            if (player.MyCharacter.CharacterItemManager.Slots[1].Item != null) 
                imgGun2.sprite = player.MyCharacter.CharacterItemManager.Slots[1].Item.ItemIcon;
            else
                imgGun2.gameObject.SetActive(false);
        }
    }
}