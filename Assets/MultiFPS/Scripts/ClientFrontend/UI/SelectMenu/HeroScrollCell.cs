using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MultiFPS.UI
{
    public class HeroScrollCell : MonoBehaviour
    {
        #region Field

        [SerializeField] private Image imgClass;
        [SerializeField] private Image imgIcon;
        [SerializeField] private Image imgSelected;
        [SerializeField] private TextMeshProUGUI tmpName;
        [SerializeField] private GameObject lockObj;

        private UnityAction<CellElement, bool> OnClick;
        private CellElement ele;

        #endregion

        #region Method
        private void OnVisible(CellElement cell)
        {
            ele = cell;
            OnClick = cell.OnClick;
            imgSelected.gameObject.SetActive(false);
            lockObj.SetActive(ele.IsLock);
            if (ele.idx > -1)
            {
                var hero = cell.HeroData;
                imgIcon.sprite = SpriteManager.Instance.GetIconHero(hero.assetIdx);
                imgClass.sprite = SpriteManager.Instance.GetIconHeroClass(hero.assetIdx);
                tmpName.text = hero.name;
            }
        }

        public void OnClick_button()
        {
            OnClick?.Invoke(ele, true);
            SelectedHero();
        }

        public void SelectedHero()
        {
            imgSelected.gameObject.SetActive(ele.isSelected);
        }
        #endregion

    }
}