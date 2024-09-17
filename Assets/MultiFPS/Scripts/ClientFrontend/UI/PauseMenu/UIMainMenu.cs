using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiFPS.UI
{
    public class UIMainMenu : UIMenu
    {
        [Header("Feature releted")]
        [SerializeField] private Button btnCollection;
        [SerializeField] private Button btnMail;
        [SerializeField] private Button btnSetting;
        [SerializeField] private Button btnMenu;
        [SerializeField] private GameObject profilePlayer;
        [SerializeField] private Button userInfoBtn;
        [SerializeField] private UserInfoController userInfo;

        [Header("Party System")]
        [SerializeField] private Button btnAdd;
        [SerializeField] private Image imgAva;

        [SerializeField] private Button btnBack;
        [SerializeField] private Image imgLogo;

        public static UIMainMenu Instance;

        private void Awake()
        {
            base.Init();
            Instance = this;

            if (userInfoBtn != null)
            {
                userInfoBtn.onClick.AddListener(() =>
                {
                    userInfo?.InitPop();
                });
            }
        }

        public void CheckState(bool isActiveLogo)
        {
            imgLogo.gameObject.SetActive(isActiveLogo);
            btnBack.gameObject.SetActive(!isActiveLogo);
        }
    }
}