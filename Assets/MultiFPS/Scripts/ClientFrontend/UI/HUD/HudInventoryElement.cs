using MultiFPS.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MultiFPS.UI
{
    public class HudInventoryElement : MonoBehaviour
    {
        [SerializeField] Image _itemIcon;
        [SerializeField] Image _itemBackground;
        [SerializeField] Image _imgUsing;
        [SerializeField] GameObject _gunAmmoObj;
        [SerializeField] TextMeshProUGUI _itemNameTxt;
        [SerializeField] TextMeshProUGUI _currentAmmoTxt;
        [SerializeField] TextMeshProUGUI _currentAmmoSupplyTxt;
        
        //[SerializeField] Color _notInUsebackGroundColor;
        //[SerializeField] Color _inUsebackGroundColor;
        //[SerializeField] Color _inUseColor;
        //[SerializeField] Color _notInUseColor;

        public void Draw(Item item, SlotType slotType, bool inUse, int slotID, SlotInput input)
        {
            gameObject.SetActive(slotType == SlotType.Normal || slotType == SlotType.PocketItem && item && item.CurrentAmmoSupply > 0);

            //_itemIDtext.color = inUse ? _inUseColor : _notInUseColor;
            //_itemIcon.color = inUse ? _inUseColor : _notInUseColor;
            //_itemBackground.color = inUse ? _inUsebackGroundColor : _notInUsebackGroundColor;

            if (!item)
            {
                transform.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 80);
                _itemIcon.sprite = null;
                this.gameObject.SetActive(false);
                return;
            }

            if(slotType == SlotType.Melee)
            {
                _currentAmmoTxt.gameObject.SetActive(false);
                _currentAmmoSupplyTxt.gameObject.SetActive(false);
            }

            if (slotID == 2)
            {
                var rescale = new Vector2(1.35f, 1.0f);
                transform.localScale = new Vector2(0.65f, 1.0f);
                _itemNameTxt.transform.localScale = rescale;
                _itemIcon.transform.localScale = rescale;
                _gunAmmoObj.transform.localScale = rescale;
            }

            _itemIcon.sprite = item.ItemIcon;
            _imgUsing.gameObject.SetActive(inUse);
            _itemNameTxt.text = item.ItemName;
        }

        public void UpdateAmmo(int currentAmmo, int currentAmmoSupply)
        {
            _currentAmmoTxt.text = string.Format("{0}", currentAmmo);
            _currentAmmoSupplyTxt.text = string.Format("/{0}", currentAmmoSupply);
        }

        public void OnClickItem()
        {

        }    
    }
}