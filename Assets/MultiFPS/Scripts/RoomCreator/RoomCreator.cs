using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MultiFPS.Gameplay.Gamemodes;
using System.Collections;
using MultiFPS.UI;

namespace MultiFPS
{
    public enum RoomSession
    {
        HostMode,
        ClientMode,
    }

    /// <summary>
    /// user interface class for user to be able to specify game parameters like map, gamemode
    /// max players and game duration
    /// </summary>
    public class RoomCreator : MonoBehaviour
    {
        public static RoomCreator Instance;
        DNNetworkManager _networkManager;
        public Dropdown MapselectionDropdown;
        public Dropdown GamemodeSelectionDropdown;
        public Dropdown GameDurationDropdown;
        public Dropdown PlayerNumberDropdown;

        public InputField PortIF;
        public InputField RoomNameIF;
        public Button HostGameButton;
        public Button ServeGameButton;
        public Button ButtonHostGame;
        public Button ButtonJoinGame;
        public Toggle BotsToggle;
        public Toggle RoomToggle;

        //user input
        int _selectedMapID;
        int _selectedTimeDurationID;
        int _selectedPlayerNumberOptionID;
        Gamemodes _selectedGamemode;
        private RoomSession _roomSession = RoomSession.HostMode;

        [Header("Options for player to choose from")]
        public MapRepresenter[] Maps;
        public int[] TimeOptionsInMinutes = { 2, 5, 10 };

        [Header("Optional")]
        [SerializeField] GameObject _loadingScreen;
        [SerializeField] Text _loadingScreenMessage;

        Coroutine _loadingScreenCoroutine;

        public bool UseDomainToConnect = false;
        public string Domain;
        public int TotalMode; 

        public Gamemodes SelectedGamemode { get => _selectedGamemode; set => _selectedGamemode = value; }
        public RoomSession RoomSession { get => _roomSession; set => _roomSession = value; }

        private void Awake()
        {
            Instance = this;

            if (_loadingScreen)
                _loadingScreen.gameObject.SetActive(false);
        }

        void Start()
        {
            _networkManager = DNNetworkManager.Instance;
            RoomSetup.Properties.P_Gamemode = Gamemodes.None;

            List<string> mapOptions = new List<string>();

            for (int i = 0; i < Maps.Length; i++)
            {
                mapOptions.Add(Maps[i].Name);
            }

            MapselectionDropdown.ClearOptions();
            MapselectionDropdown.AddOptions(mapOptions);

            MapselectionDropdown.onValueChanged.AddListener(OnMapselected);
           // GamemodeSelectionDropdown.onValueChanged.AddListener(OnGamemodeSelected);
            GameDurationDropdown.onValueChanged.AddListener(OnGameDurationSelected);
            PlayerNumberDropdown.onValueChanged.AddListener(OnPlayerNumberOption);

            //game duration options
            List<string> durationOptions = new List<string>();

            for (int i = 0; i < TimeOptionsInMinutes.Length; i++)
            {
                durationOptions.Add(TimeOptionsInMinutes[i].ToString() + " minutes");
            }

            GameDurationDropdown.AddOptions(durationOptions);

            PlayerNumberDropdown.ClearOptions();

            //if (HostGameButton) HostGameButton.onClick.AddListener(ShowLobbyMenu);
            if (ServeGameButton) ServeGameButton.onClick.AddListener(ServeGame);
           // if (ServeGameButton) ServeGameButton.onClick.AddListener(ServeGame);

            OnMapselected(0);
        }


        void OnMapselected(int mapID)
        {
            _selectedPlayerNumberOptionID = 0;
            _selectedMapID = mapID;
            OnGamemodeSelected(_selectedMapID);

            //fill gamemodes dropdown with options avaible for given map
            Gamemodes[] avaibleGamemodesForThisMap = Maps[mapID].AvailableGamemodes;
            UIHomeMenu.Instance.TotalModes = avaibleGamemodesForThisMap.Length;

            List<string> gamemodeOptions = new List<string>();

            for (int i = 0; i < avaibleGamemodesForThisMap.Length; i++)
            {
                gamemodeOptions.Add(avaibleGamemodesForThisMap[i].ToString());
            }

            GamemodeSelectionDropdown.ClearOptions();
            GamemodeSelectionDropdown.AddOptions(gamemodeOptions);

            //draw maxPlayerNumber dropdown
            List<string> playerNumberOptions = new List<string>();
            MapRepresenter map = Maps[mapID];
            //player number options
            for (int i = 0; i < map.MaxPlayersPresets.Length; i++)
            {
                playerNumberOptions.Add(map.MaxPlayersPresets[i].ToString() + " players");
            }
            PlayerNumberDropdown.ClearOptions();
            PlayerNumberDropdown.AddOptions(playerNumberOptions);
        }

        /// <summary>
        /// trigged by selecting gamemode in UI room creator, tells game which gamemode to setup
        /// </summary>
        /// <param name="gamemodeID"></param>
        void OnGamemodeSelected(int gamemodeID) //gamemode ID is relevant to gamemodes order in their enum
        {
            _selectedGamemode = Maps[_selectedMapID].AvailableGamemodes != null && Maps[_selectedMapID].AvailableGamemodes.Length > 0 ? Maps[_selectedMapID].AvailableGamemodes[gamemodeID] : Gamemodes.None;
        }
        void OnGameDurationSelected(int timeOptionID)
        {
            _selectedTimeDurationID = timeOptionID;
        }
        void OnPlayerNumberOption(int playerOptionID)
        {
            _selectedPlayerNumberOptionID = playerOptionID;
        }

        //write parameters and start game as host

        public void HostGame()
        {          
            SetGameProperties();
            _networkManager.StartHost();
        }

        void ServeGame()
        {
            SetGameProperties();
            _networkManager.StartServer();
        }

        public void ClientGame()
        {
            _networkManager.StartClient();
        }

        public void ChangeGameMode(int gamemodeID)
        {
            _selectedGamemode = Maps[_selectedMapID].AvailableGamemodes != null && Maps[_selectedMapID].AvailableGamemodes.Length > 0 ? Maps[_selectedMapID].AvailableGamemodes[gamemodeID] : Gamemodes.None;
        }

        public string GetGameMap()
        {
            return Maps[_selectedMapID].Name;
        }


        void SetGameProperties() 
        {
            _networkManager.onlineScene = Maps[_selectedMapID].Scene;

            RoomSetup.Properties.P_Gamemode = _selectedGamemode;
            RoomSetup.Properties.P_FillEmptySlotsWithBots = BotsToggle.isOn;
            RoomSetup.Properties.P_GameDuration = TimeOptionsInMinutes[_selectedTimeDurationID] * 60;
            RoomSetup.Properties.P_RespawnCooldown = 6f;

            //player count
            int maxPlayers = 10;
            RoomSetup.Properties.P_MaxPlayers = maxPlayers; //for gamemode

            _networkManager.maxConnections = maxPlayers; //for handling connections
        }

        #region loading screen
        void ShowLoadingScreen(string message, float liveTime = 10f)
        {
            if (_loadingScreenCoroutine != null)
                StopCoroutine(_loadingScreenCoroutine);

            StartCoroutine(LoadingScreenCoroutine(message, liveTime));
        }
        IEnumerator LoadingScreenCoroutine(string message, float liveTime)
        {
            if (!_loadingScreen || !_loadingScreenMessage) yield break;

            _loadingScreen.SetActive(true);
            _loadingScreenMessage.text = message;

            yield return new WaitForSeconds(liveTime);

            _loadingScreen.SetActive(false);
            _loadingScreenMessage.text = string.Empty;
        }
        #endregion
    
        void SetServer()
        {
            ButtonJoinGame.gameObject.SetActive(false);
            ButtonHostGame.gameObject.SetActive(false);

            if (RoomToggle.isOn)
                ButtonJoinGame.gameObject.SetActive(true);
            else
                ButtonHostGame.gameObject.SetActive(true);
        }    

        [System.Serializable]
        public class PlayerConnectToRoomRequest
        {
            public string Address;
            public string Port;
        }
    }
}