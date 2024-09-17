using MultiFPS.Gameplay;
using MultiFPS.UI.HUD;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

namespace MultiFPS.UI
{
    public class ItemElement : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] TextMeshProUGUI itemName;
        [SerializeField] TextMeshProUGUI itemPrice;
        [SerializeField] Image itemIcon;
        [SerializeField] Image imgItemChoosed;
        [SerializeField] TextMeshProUGUI itemNamePressed;
        [SerializeField] Color normalColor;
        [SerializeField] Color selectedColor;
        [SerializeField] Button btnRefurn;
        [SerializeField] Image imgSelected;

        private int _playerSlotID;
        private int _slotID;
        private List<int> itemIDs = new List<int>();
        private int clickCount;

        public bool IsSelected { get; private set; }
        [HideInInspector]
        public GunType gunType;
        [HideInInspector]
        public string nameGun;

        public void Draw(int slotID, int playerSlotID)
        {
            _slotID = slotID;
            _playerSlotID = playerSlotID;

            Item item = ItemManager.Instance.SlotsLodout[slotID].availableItemsForSlot[playerSlotID];
            itemName.text = item.ItemName;
            itemNamePressed.text = item.ItemName;
            itemIcon.sprite = item.ItemIcon;
            itemPrice.text = "Free";
            imgItemChoosed.gameObject.SetActive(false);
            gunType = item.gunType;
            nameGun = item.ItemName;
            btnRefurn.onClick.AddListener(ToggleSelection);
            for (int i = 0; i < ItemManager.Instance.SlotsLodout[slotID].availableItemsForSlot.Length; i++)
            {
                itemIDs.Add(i);
            }
        }

        public void OnItemSelected()
        {
            if (ItemManager.Instance.SlotsLodout[_slotID].availableItemsForSlot.Length == 0) return;

            ClientFrontend.ClientPlayerInstance.MyCharacter.CharacterItemManager.BuyItemBattle(_slotID, _playerSlotID);
            UIShopMenu.Instance.CheckStateGun();
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            btnRefurn.gameObject.SetActive(selected);
            itemName.color = selected ? selectedColor : normalColor;
            imgSelected.gameObject.SetActive(selected);
        }

        public void HoldItem()
        {
            imgSelected.gameObject.SetActive(false);
            imgItemChoosed.gameObject.SetActive(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsSelected)
                clickCount++;
            else
            {
                ToggleSelection();
            }

            if (clickCount == 1)
            {
                HoldItem();
            }
            else if (clickCount == 2)
            {
                //buy item
                clickCount = 0;
                imgItemChoosed.gameObject.SetActive(false);
                ToggleSelection();
            }
        }

        private async void ToggleSelection()
        {
            if (IsSelected)
            {
                //refund item
                SetSelected(false);
                imgItemChoosed.gameObject.SetActive(false);
                PlayerGameplayInput.Instance.DropItem();
                await Task.Delay(100);
                PlayerGameplayInput.Instance.TakeItem2();
            }
            else
            {
                //buy item
                SetSelected(true);
                OnItemSelected();
            }
        }
    }
}
