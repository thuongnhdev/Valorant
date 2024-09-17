using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MultiFPS.UI
{
    public class UIPauseMenuPanelsManager : MonoBehaviour
    {
        [System.Serializable]
        public class Panel
        {
            public GameObject PanelOverlay;
            public Button ShowButton;
            public Button HideButton;

            public void Initialize()
            {
                ShowButton.onClick.AddListener(ShowPanel);
                HideButton.onClick.AddListener(HidePanel);
                HidePanel();
            }
            public void ShowPanel()
            {
                PanelOverlay.SetActive(true);
            }
            public void HidePanel()
            {
                PanelOverlay.SetActive(false);
            }
        }

        public Panel[] panels;

        private void Awake()
        {
            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].ShowButton.onClick.AddListener(HideAllPanels);
                panels[i].Initialize();
            }

            if (panels[1] != null)
                panels[1].ShowPanel();
        }
        void HideAllPanels()
        {
            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].HidePanel();
            }
        }
    }
}