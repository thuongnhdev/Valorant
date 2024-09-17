using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiFPS.UI
{
    public class HeroSelectScroll : SelectScroll
    {
        public UISelectMenu parentMenu;
        public RectTransform scrollContents;
        public ReuseableScroller scroll;

        private float defaultContnetsHeight;

        void Awake()
        {
            defaultContnetsHeight = scrollContents.sizeDelta.y;
        }

        public void HeroScrollSetting()
        {
            scrollContents.sizeDelta = new Vector2(scrollContents.sizeDelta.x, defaultContnetsHeight);
        }

        public void SetScroll()
        {
            scroll.Clear();

            var heroList = MasterCatcher.Instance.HeroDatas;
            for (int i = 0; i < heroList.Count; i++)
            {
                var data = heroList[i];
                CellElement ele = new CellElement(i, data, this, true,  OnClick_Cell, OnLongPress_Cell, false, data.isLock);
                scroll.Add(ele);
            }
        }

        public void OnClick_Cell(CellElement cell, bool bCheck)
        {            
            parentMenu.SetHeroSlot(cell);
        }
 

        public void OnLongPress_Cell(CellElement cell)
        { }    

        public void RefreshScroll()
        {
            scroll.RefreshAll();
        }

    }
}