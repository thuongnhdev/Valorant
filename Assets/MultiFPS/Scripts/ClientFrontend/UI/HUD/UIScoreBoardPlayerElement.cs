using MultiFPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MultiFPS.UI.HUD
{
    public class UIScoreBoardPlayerElement : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _playerName;
        [SerializeField] TextMeshProUGUI _kills;
        [SerializeField] TextMeshProUGUI _deaths;
        [SerializeField] TextMeshProUGUI _assists;
        [SerializeField] TextMeshProUGUI _latency;
        [SerializeField] Image _background;
        [SerializeField] Image _imglineBg;
        [SerializeField] Color teamColor; 
        [SerializeField] Color enemyColor;
        [SerializeField] Image linePlayerImg;
        [SerializeField] Image imgIconLoadout;
        [SerializeField] Image imgIconAmmo;
        [SerializeField] Image imgIconHero;

        public void WriteData(PlayerInstance player)
        {
            this.gameObject.SetActive(true);

            if (player.Team == -1 && player != ClientFrontend.ClientPlayerInstance) return;

            _playerName.text = player.PlayerInfo.Username;
            _kills.text = player.Kills.ToString();
            _deaths.text = player.Deaths.ToString();
            _assists.text = player.Assists.ToString();
            imgIconHero.sprite = SpriteManager.Instance.GetIconHero(player.PlayerInfo.CharacterModelId);
            //assign appropriate color for player in scoreboard depending on team, if player is not in any team, give him white color

            bool isThisClientTeam = ClientFrontend.ThisClientTeam == player.Team;

            _background.color = isThisClientTeam ? teamColor : enemyColor;
            _imglineBg.color = isThisClientTeam ? teamColor : enemyColor;
            _latency.text = player.BOT ? "BOT" : player.Latency.ToString();
            linePlayerImg.gameObject.SetActive(player.isOwned);

            if (player.MyCharacter == null)
                return;

            if (player.MyCharacter.CharacterItemManager.CurrentlyUsedItem == null)
            {
                imgIconLoadout.gameObject.SetActive(false);
            }   
            else
            {
                imgIconLoadout.gameObject.SetActive(true);
                imgIconLoadout.sprite = player.MyCharacter.CharacterItemManager.CurrentlyUsedItem.ItemIcon;
            }    

        }
    }
}