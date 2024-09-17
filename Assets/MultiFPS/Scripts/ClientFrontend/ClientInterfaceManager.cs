using UnityEngine.SceneManagement;
using UnityEngine;
using MultiFPS.Gameplay;
using MultiFPS.UI.HUD;
using System.Collections.Generic;
using Mirror;

using MultiFPS.Gameplay.Gamemodes;
using MultiFPS.UI.Gamemodes;

namespace MultiFPS.UI
{
    public class ClientInterfaceManager : MonoBehaviour
    {
        public static ClientInterfaceManager Instance;

        //UI prefabs
        public GameObject PauseMenuUI;
        public GameObject ChatUI;
        public GameObject ScoreboardUI;
        public GameObject KillfeedUI;
        public GameObject PlayerHudUI;
        public GameObject PlayerNametag;
        public GameObject GameplayCamera;
        public GameObject UIHomeMenu;
        public GameObject UILobbyMenu;
        public GameObject UIMatchMenu;
        public GameObject UILoading;
        public GameObject UIMainMenu;
        public GameObject UISelectMenu;
        public GameObject UILoadingBattle;
        public GameObject UIShopMenu;
        [SerializeField] GameObject[] _additionalUI;


        //these colors are here because we may want to adjust them easily in the inspector
        public UIColorSet UIColorSet;

        public SkinContainer[] characterSkins;
        public ItemSkinContainer[] ItemSkinContainers;

        List<UICharacterNametag> _spawnedNametags = new List<UICharacterNametag>();

        [Header("Gamemodes UI Prefabs")]
        [SerializeField] GameObject[] gamemodesUI;

        public int IdxHeroSelected = 0;
        [HideInInspector]
        public int TeamIdx;

        public void Awake()
        {

            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                //if this happens it means that player returns to hub scene with Client Manager from previous hub scene load, so we dont
                //need another one, so destroy this one

                Destroy(gameObject);
                return;
            }


            SceneManager.sceneLoaded += OnSceneLoaded;

            GameManager.GameEvent_CharacterTeamAssigned += OnCharacterTeamAssigned;
            ClientFrontend.ClientFrontendEvent_OnObservedCharacterSet += OnObservedCharacterSet;

            UserSettings.SelectedItemSkins = new int[ItemSkinContainers.Length];

            for (int i = 0; i < ItemSkinContainers.Length; i++)
            {
                UserSettings.SelectedItemSkins[i] = -1;
            }

            ClientFrontend.ClientEvent_OnJoinedToGame += InstantiateUIforGivenGamemode;

            //by default cursor is hidden, show it for main menu
            ClientFrontend.ShowCursor(true);
            InitializeUI();

        }

        /// <summary>
        /// This method will instantiate UI proper for gamemode, for example if we play Defuse gamemode, spawn UI wchich have
        /// score numbers for both teams, and bomb icon which we will display and color in red when bomb is planted
        /// </summary>
        void InstantiateUIforGivenGamemode(Gamemode gamemode, NetworkIdentity player)
        {
            int gamemodeID = (int)gamemode.Indicator;

            if (gamemodeID >= gamemodesUI.Length || gamemodesUI[gamemodeID] == null) return; //no ui for this gamemode avaible

            Instantiate(gamemodesUI[gamemodeID]).GetComponent<UIGamemode>().SetupUI(gamemode, player);
            JoinTeam();
        }

        private void JoinTeam()
        {
            List<PlayerInstance> players = GameManager.Players;
            var teamJoin = -1;
            if (players.Count >= RoomSetup.Properties.P_MaxPlayers)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (!players[i].BOT) continue;

                        teamJoin = players[i].Team;
                        players[i].DespawnCharacterIfExist();
                        NetworkServer.Destroy(players[i].gameObject);
                        
                        break;
                }
            }

            teamJoin = (players.Count == 0) ? Random.Range(0, 2) : (players.FindAll(x => x.Team == 0).Count < RoomSetup.Properties.P_MaxPlayers/2 ? 0 : 1);
            if (RoomCreator.Instance.RoomSession == RoomSession.HostMode) teamJoin = 0;
            else
            {
                teamJoin = TeamIdx;
            }
            ClientFrontend.ClientPlayerInstance.ClientRequestJoiningTeam(teamJoin);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            var index = SceneManager.GetActiveScene().buildIndex;

            ClientFrontend.Hub = (index == 0);

            ClientFrontend.ShowCursor(index == 0);
            ClientFrontend.SetClientTeam(-1);

            //if we loaded non-hub scene, then spawn all the UI prefabs for player, then on disconnecting they will
            //be destroyed by scene unloading
            if (index != 0)
            {
                if (PauseMenuUI)
                    Instantiate(PauseMenuUI);
                if (ChatUI)
                    Instantiate(ChatUI);
                if (ScoreboardUI)
                    Instantiate(ScoreboardUI);
                if (KillfeedUI)
                    Instantiate(KillfeedUI);  
                if (UIShopMenu)
                    Instantiate(UIShopMenu);
                if (PlayerHudUI)
                    Instantiate(PlayerHudUI).GetComponent<Crosshair>().Setup();
                if (_additionalUI != null)
                    for (int i = 0; i < _additionalUI.Length; i++)
                    {
                        Instantiate(_additionalUI[i]);
                    }

                UILobbyMenu.SetActive(false);
                UIHomeMenu.SetActive(false);
                UIMatchMenu.SetActive(false);
                UILoading.SetActive(false);
                UIMainMenu.SetActive(false);
                UISelectMenu.SetActive(false);
                UILoadingBattle.SetActive(false);
            }

            else
            {
                UIHomeMenu.SetActive(true);
                UIMainMenu.SetActive(true);
            }
        }

        private void InitializeUI()
        {
            if (ClientFrontend.Hub)
            {
                if (UIMainMenu)
                    UIMainMenu = Instantiate(UIMainMenu, transform);
                if (UIHomeMenu)
                    UIHomeMenu = Instantiate(UIHomeMenu, transform);
                if (UILobbyMenu)
                    UILobbyMenu = Instantiate(UILobbyMenu, transform);
                if (UIMatchMenu)
                    UIMatchMenu = Instantiate(UIMatchMenu, transform);
                if (UILoading)
                    UILoading = Instantiate(UILoading, transform);
                if (UISelectMenu)
                    UISelectMenu = Instantiate(UISelectMenu, transform);  
                if (UILoadingBattle)
                    UILoadingBattle = Instantiate(UILoadingBattle, transform);

                UILobbyMenu.SetActive(false);
                UIMatchMenu.SetActive(false);
                UILoading.SetActive(false);
                UISelectMenu.SetActive(false);
                UILoadingBattle.SetActive(false);
            }
        }

        //reassign nametags when spectated player is changed
        public void OnObservedCharacterSet(CharacterInstance characterInstance)
        {
            DespawnAllNametags();

            List<PlayerInstance> players = GameManager.Players;

            for (int i = 0; i < players.Count; i++)
            {
                OnCharacterTeamAssigned(players[i].MyCharacter);
            }
        }

        public string SetNameMode(Gameplay.Gamemodes.Gamemodes gamemodes)
        {
            string nameMode = "None";
            switch (gamemodes)
            {
                case Gameplay.Gamemodes.Gamemodes.None:
                    nameMode = "None";
                    break;
                case Gameplay.Gamemodes.Gamemodes.Deathmatch:
                    nameMode = "Single Deathmatch";
                    break;
                case Gameplay.Gamemodes.Gamemodes.TeamDeathmatch:
                    nameMode = "DEATHMATCH";
                    break;
                case Gameplay.Gamemodes.Gamemodes.TeamEliminations:
                    nameMode = "Team Eliminations";
                    break;
                case Gameplay.Gamemodes.Gamemodes.Defuse:
                    nameMode = "Team Defuse";
                    break;
                case Gameplay.Gamemodes.Gamemodes.TeamDeathmatchMode:
                    nameMode = "Team Deathmatch Mode";
                    break;
                case Gameplay.Gamemodes.Gamemodes.NormalMode:
                    nameMode = "UNRATED";
                    break;
                default:
                    break;
            }

            return nameMode;
        }

        public void OnCharacterTeamAssigned(CharacterInstance characterInstance)
        {
            if (!characterInstance) return;
            //dont spawn nametag for player if we dont know yet which team our player belongs to
            if (!ClientFrontend.ClientTeamAssigned) return;

            //dont spawn matkers for enemies
            if (ClientFrontend.ThisClientTeam != characterInstance.Health.Team || GameManager.Gamemode.FFA) return;

            if (characterInstance.Health.CurrentHealth <= 0) return;
            //dont spawn nametag for player who views world from first person perspective

            if (characterInstance.netId == ClientFrontend.ObservedCharacterNetID())
                return;

            UICharacterNametag playerNameTag = Instantiate(PlayerNametag).GetComponent<UICharacterNametag>();
            playerNameTag.Set(characterInstance);

            _spawnedNametags.Add(playerNameTag);
        }

        void DespawnAllNametags()
        {
            for (int i = 0; i < _spawnedNametags.Count; i++)
            {
                _spawnedNametags[i].DespawnMe();
            }
            _spawnedNametags.Clear();
        }

        private void OnDestroy()
        {
            ClientFrontend.ClientEvent_OnJoinedToGame -= InstantiateUIforGivenGamemode;
        }

        public void GoHomeMenu(bool isActive)
        {
            if (UIHomeMenu != null)
            {
                UIHomeMenu.gameObject.SetActive(isActive);
                UILobbyMenu.gameObject.SetActive(!isActive);
                UIMainMenu.gameObject.SetActive(true);
            }    
        }

        public void ActiveMainMenu(bool isActive, bool isHomeMenu = false)
        {
            if (UIMainMenu != null)
                UIMainMenu.gameObject.SetActive(isActive);

            if(isActive)
            {
                GetComponentInChildren<UIMainMenu>().CheckState(UIHomeMenu.activeInHierarchy);
            }    
        }
    }

    [System.Serializable]
    public class ItemSkinContainer
    {
        public string ItemName;
        public SingleItemSkinContainer[] Skins;
    }
    [System.Serializable]
    public class SingleItemSkinContainer
    {
        public string SkinName;
        public Material Skin;
    }
}
