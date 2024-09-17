using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using MultiFPS.Sound;

namespace MultiFPS.UI
{
    public class UILobbyMenu : UIMenu
    {
        public static UILobbyMenu Instance;

        [Header("Info LobbyMenu")]
        [SerializeField] private Button btnBack;
        [SerializeField] private Button btnHome;
        [SerializeField] private Button btnStart;
        [SerializeField] private Button btnJoint;
        [SerializeField] private GameObject JoiningObj;
        [SerializeField] private TextMeshProUGUI txtTimeMatch;
        [SerializeField] private Button btnMode;
        [SerializeField] private Button btnCancel;
        [SerializeField] private GameObject objFade;
        [SerializeField] private GameObject objCount;
        [SerializeField] private TextMeshProUGUI tmpGameMode;


        [Header("Info Player")]
        [SerializeField] private List<InfoPlayerLobby> infoPlayerLobbies = new List<InfoPlayerLobby>();

        private bool isFindingMatch;
        private float timeElapsed;

        private const int TIME_FIND_MATCHING = 5;
        private Coroutine countupCoroutine;

        private void Awake()
        {
            Instance = this;

            objCount.SetActive(false);
            objFade.SetActive(false);

            btnStart.onClick.AddListener(()=>StartCount(RoomSession.HostMode));
            btnJoint.onClick.AddListener(()=>StartCount(RoomSession.ClientMode));
            btnCancel.onClick.AddListener(OnClickCancelFind);
            SetupHomeAndBackButtons();
        }

        private void OnEnable()
        {
            ClientInterfaceManager.Instance.ActiveMainMenu(true);
            tmpGameMode.text = ClientInterfaceManager.Instance.SetNameMode(RoomCreator.Instance.SelectedGamemode);
            SetStateBtnObj(true);
        }

        private void OnDisable()
        {
            OnClickCancelFind();
            ClientInterfaceManager.Instance.ActiveMainMenu(false);
            SetStateBtnObj(true);
        }

        private void SetupHomeAndBackButtons()
        {
            if (btnHome != null && btnBack != null)
            {
                btnBack.onClick.AddListener(GoHome);
                btnHome.onClick.AddListener(GoHome);
            }
        }

        private void GoHome()
        {
            SoundManager.Instance.PlayOneShot_UI();
            gameObject.SetActive(false);
            ClientInterfaceManager.Instance.GoHomeMenu(true);
        }

        private void OnClickCancelFind()
        {
            isFindingMatch = false;
            timeElapsed = 0;
            btnStart.gameObject.SetActive(true);
            objCount.SetActive(false);
            if (countupCoroutine != null)
            {
                StopCoroutine(countupCoroutine);
                countupCoroutine = null;
            }

            SetStateBtnObj(true);
        }

        private void StartCount(RoomSession roomSession)
        {
            SoundManager.Instance.PlayOneShot_UI();
            OnClickCancelFind();
            isFindingMatch = true;
            countupCoroutine = StartCoroutine(CountupTimer());
            SetStateBtnObj(false);
            objCount.SetActive(true);
            RoomCreator.Instance.RoomSession = roomSession;
        }

        private void SetStateBtnObj(bool isEnable)
        {
            btnStart.gameObject.SetActive(isEnable);
            btnJoint.gameObject.SetActive(isEnable);
            JoiningObj.gameObject.SetActive(!isEnable);
        }

        #region Prototype
        private IEnumerator CountupTimer()
        {
            timeElapsed = 0;
            while (isFindingMatch)
            {
                yield return new WaitForSeconds(1f);

                timeElapsed += 1f;
                UpdateTimerText();

                if (timeElapsed >= TIME_FIND_MATCHING)
                {
                    ClientInterfaceManager.Instance.UIMatchMenu.gameObject.SetActive(true);
                    gameObject.SetActive(false);
                    OnClickCancelFind();
                }
            }
        }

        private void UpdateTimerText()
        {
            int totalSeconds = (int)timeElapsed;
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            if (hours > 0)
            {
                txtTimeMatch.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
            }
            else if (minutes > 0)
            {
                txtTimeMatch.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            else
            {
                txtTimeMatch.text = string.Format("00:{0:00}", seconds);
            }
        }
        #endregion
        public void AddPlayerToLobby(InfoPlayerLobby player)
        {
            infoPlayerLobbies.Add(player);
            UpdatePlayerList();
        }

        public void RemovePlayerFromLobby(InfoPlayerLobby player)
        {
            infoPlayerLobbies.Remove(player);
            UpdatePlayerList();
        }

        private void UpdatePlayerList()
        {
        
        }

    }
}
