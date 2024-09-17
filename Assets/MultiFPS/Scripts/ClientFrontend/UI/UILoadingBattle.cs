using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MultiFPS.Gameplay.Gamemodes;

namespace MultiFPS.UI
{
    public class UILoadingBattle : UIMenu
    {
        [SerializeField] private List<InfoPlayerLobby> infoPlayer = new();
        [SerializeField] private List<InfoPlayerLobby> infoEnemy = new();
        [SerializeField] private TextMeshProUGUI tmpLoading;
        [SerializeField] private TextMeshProUGUI tmpMapName;
        [SerializeField] private TextMeshProUGUI tmpMode;
        [SerializeField] private Slider sliderLoadingBar;

        private static int TIME_LOADING_MENU = 5;

        private void OnEnable()
        {
            StartCoroutine(Loading());
            tmpMode.text = ClientInterfaceManager.Instance.SetNameMode(RoomCreator.Instance.SelectedGamemode);
            tmpMapName.text = RoomCreator.Instance.GetGameMap();
        }


        private IEnumerator Loading()
        {
            float timeWait = TIME_LOADING_MENU;
            while (timeWait > 0)
            {
                float percentLoad = (TIME_LOADING_MENU - timeWait) / TIME_LOADING_MENU * 100;
                tmpLoading.text = string.Format("{0}%", percentLoad);
                sliderLoadingBar.value = percentLoad / 100;

                yield return new WaitForSeconds(1);
                timeWait--;
                if (timeWait == 0)
                {
                    if(RoomCreator.Instance.RoomSession == RoomSession.HostMode)
                        RoomCreator.Instance.HostGame();
                    else
                        RoomCreator.Instance.ClientGame();
                    this.Hide();
                }
            }

        } 
    }
}