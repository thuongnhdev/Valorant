using UnityEngine;

namespace MultiFPS.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        public GameObject LobbyMenu;
        public GameObject HomeMenu;
        public GameObject MatchMenu;
        public GameObject LoadingMenu;
        public GameObject MainMenu;
        public GameObject SelectMenu;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeUI();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            LobbyMenu.SetActive(false);
            HomeMenu.SetActive(true);
            MatchMenu.SetActive(false);
            LoadingMenu.SetActive(false);
            MainMenu.SetActive(false);
            SelectMenu.SetActive(false);
        }

        void InitializeUI()
        {
            if (LobbyMenu)
                LobbyMenu = Instantiate(LobbyMenu, transform);
            if (HomeMenu)
                HomeMenu = Instantiate(HomeMenu, transform);
            if (MatchMenu)
                MatchMenu = Instantiate(MatchMenu, transform);
            if (LoadingMenu)
                LoadingMenu = Instantiate(LoadingMenu, transform);
            if (MainMenu)
                MainMenu = Instantiate(MainMenu, transform);
            if (SelectMenu)
                SelectMenu = Instantiate(SelectMenu, transform);
        }

        public void GoHomeMenu(bool isActive)
        {
            if (HomeMenu != null)
                HomeMenu.gameObject.SetActive(isActive);
        }

        public void ActiveMainMenu(bool isActive)
        {
            if (MainMenu != null)
                MainMenu.gameObject.SetActive(isActive);
        }
    }
}