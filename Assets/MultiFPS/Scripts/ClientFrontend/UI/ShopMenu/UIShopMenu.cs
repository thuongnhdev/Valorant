using MultiFPS.Gameplay;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiFPS.UI
{
    public class UIShopMenu : MonoBehaviour
    {
        public static UIShopMenu Instance;

        [SerializeField] Button buttonCancel;
        [SerializeField] GameObject Overlay;
        [SerializeField] Transform gridInfoPlayer;
        [SerializeField] GameObject playerPresenter;

        private List<GameObject> infoUsers = new List<GameObject>();

        [HideInInspector] public bool IsBuyPhase;
        [HideInInspector] public List<ItemElement> listGunPrimary = new List<ItemElement>();
        [HideInInspector] public List<ItemElement> listGunSecondary = new List<ItemElement>();
        [HideInInspector] public bool shopMenuShowing = false;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            buttonCancel.onClick.AddListener(OnClickCloseMenu);
        }

        private void Update()
        {
            HandleShopMenuVisibility();
        }

        private void HandleShopMenuVisibility()
        {
            if (shopMenuShowing)
            {
                if (!IsBuyPhase || Input.GetKeyDown(KeyCode.Escape))
                {
                    CloseShopMenu();
                    return;
                }

                CheckStateGun();
                SetInfoPlayer();
            }

            if (Input.GetKeyDown(KeyCode.B) && !ClientFrontend.Hub && IsBuyPhase)
            {
                if (ChatBehaviour._instance?.ChatWriting == true)
                    return;

                ToggleShopMenu();
            }
        }

        private void ToggleShopMenu()
        {
            ShowShopMenu(!shopMenuShowing);
            Overlay.SetActive(shopMenuShowing);
        }

        private void CloseShopMenu()
        {
            Overlay.SetActive(false);
            ShowShopMenu(false);
        }

        private void ShowShopMenu(bool isShow)
        {
            if (shopMenuShowing == isShow) return;

            shopMenuShowing = isShow;
            ClientFrontend.SetPause(isShow);
            ClientFrontend.ShowCursor(isShow);
        }

        private void OnClickCloseMenu()
        {
            CloseShopMenu();
        }

        public void CheckStateGun()
        {
            for (int i = 0; i < listGunSecondary.Count; i++)
            {
                foreach (var item in ClientFrontend.ClientPlayerInstance.MyCharacter.CharacterItemManager.Slots)
                {
                    if (item.Item == null)
                        continue;

                    if (listGunSecondary[i].nameGun == item.Item.ItemName)
                    {
                        listGunSecondary[i].SetSelected(true);
                        break;
                    }
                    else
                        listGunSecondary[i].SetSelected(false);
                }
            }

            for (int i = 0; i < listGunPrimary.Count; i++)
            {
                foreach (var item in ClientFrontend.ClientPlayerInstance.MyCharacter.CharacterItemManager.Slots)
                {
                    if (item.Item == null)
                        continue;

                    if (listGunPrimary[i].nameGun == item.Item.ItemName)
                    {
                        listGunPrimary[i].SetSelected(true);
                        break;
                    }
                    else
                        listGunPrimary[i].SetSelected(false);
                }
            }
        }

        private void SetInfoPlayer()
        {
            if (infoUsers.Count > 0 && infoUsers != null)
            {
                foreach (GameObject gm in infoUsers)
                {
                    Destroy(gm);
                }

                infoUsers.Clear();
            }
            
            List<PlayerInstance> players = GameManager.Players;
            var playerTeam = players.FindAll(x => x.Team == ClientFrontend.ThisClientTeam);
            for (int i = 0; i < playerTeam.Count; i++)
            {
                PlayerInstance player = playerTeam[i];
                GameObject presenter = Instantiate(playerPresenter, gridInfoPlayer);
                presenter.SetActive(true);
                presenter.transform.SetParent(gridInfoPlayer);
                presenter.GetComponent<InfoUserItemShop>().WriteData(player);

                infoUsers.Add(presenter);
            }
        }
    }
}
