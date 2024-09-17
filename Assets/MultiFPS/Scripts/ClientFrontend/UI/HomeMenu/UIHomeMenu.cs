using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MultiFPS;
using MultiFPS.Sound;

namespace MultiFPS.UI
{
    public class UIHomeMenu : UIMenu
    {
        public static UIHomeMenu Instance;

        [SerializeField] private Button btnPlay;
        [SerializeField] private Button btnSwitchMode;
        [SerializeField] private TextMeshProUGUI tmpGameMode;
        [SerializeField] private Button btnChooseTeam;

        public GameObject popupChooseTeam;
        public Button btnAttack;
        public Button btnDefending;

        private int currentModeIndex = 0;
        public int TotalModes;


        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }

            btnPlay.onClick.AddListener(OnClickBtnPlay);
            btnSwitchMode.onClick.AddListener(OnClickBtnSwitchMode);
            btnChooseTeam.onClick.AddListener(OnclickButtonChooseTeam);
            btnAttack.onClick.AddListener(OnClickBtnAttack);
            btnDefending.onClick.AddListener(OnClickBtnDefending);
            tmpGameMode.text = ClientInterfaceManager.Instance.SetNameMode(Gameplay.Gamemodes.Gamemodes.NormalMode);
        }
        private void OnClickBtnPlay()
        {
            SoundManager.Instance.PlayOneShot_UI();
            SoundManager.Instance.OnPlaySoundLobbyMenu();
            this.Hide();
            ClientInterfaceManager.Instance.UILobbyMenu.gameObject.SetActive(true);
        }    
        
        private void OnClickBtnSwitchMode()
        {
            SoundManager.Instance.PlayOneShot_UI();
            currentModeIndex = (currentModeIndex + 1) % TotalModes;
            RoomCreator.Instance.ChangeGameMode(currentModeIndex);
            tmpGameMode.text = ClientInterfaceManager.Instance.SetNameMode(RoomCreator.Instance.SelectedGamemode);
        }

        private void OnEnable()
        {
            ClientInterfaceManager.Instance.ActiveMainMenu(true, true);
        }

        private void OnclickButtonChooseTeam()
        {
            SoundManager.Instance.PlayOneShot_UI();
            popupChooseTeam.SetActive(true);
        }

        private void OnClickBtnAttack()
        {
            SoundManager.Instance.PlayOneShot_UI();
            ClientInterfaceManager.Instance.TeamIdx = 0;
            popupChooseTeam.SetActive(false);
        }
        private void OnClickBtnDefending()
        {
            SoundManager.Instance.PlayOneShot_UI();
            ClientInterfaceManager.Instance.TeamIdx = 1;
            popupChooseTeam.SetActive(false);
        }
    }
}