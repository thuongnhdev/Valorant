using MultiFPS.Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace MultiFPS.UI
{
    public class ListItemElement : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private Transform _gridParent;
        [SerializeField] private ItemElement item;
        [SerializeField] private GunType gunType;


        void Awake()
        {
            CharacterItemManager characterItemManager = _playerPrefab.GetComponent<CharacterItemManager>();

            if (UserSettings.PlayerLodout == null || UserSettings.PlayerLodout.Length == 0)
            {
                UserSettings.PlayerLodout = new int[characterItemManager.Slots.Count];

                for (int i = 0; i < UserSettings.PlayerLodout.Length; i++)
                {
                    UserSettings.PlayerLodout[i] = -1;  
                }
            }

            for (int i = 0; i < ItemManager.Instance.SlotsLodout.Length; i++)
            {
                for (int j = 0; j < ItemManager.Instance.SlotsLodout[i].availableItemsForSlot.Length; j++)
                {
                    if (ItemManager.Instance.SlotsLodout[i].availableItemsForSlot[j].gunType == gunType)
                    {
                        ItemElement itemEle = Instantiate(item, _gridParent);
                        itemEle.Draw(i, j);

                        if (itemEle.gunType == GunType.Sidearm)
                            UIShopMenu.Instance.listGunPrimary.Add(itemEle);
                        else 
                            UIShopMenu.Instance.listGunSecondary.Add(itemEle);
                    }
                }
            }
        }
    }
}
