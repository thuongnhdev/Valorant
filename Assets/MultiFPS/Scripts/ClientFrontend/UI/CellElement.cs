using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MultiFPS.UI
{
    public class CellElement
    {
        public int idx;
        public HeroData HeroData;
        public SelectScroll parent;
        public bool isSmall;
        public UnityAction<CellElement, bool> OnClick;
        public UnityAction<CellElement> OnLongPress;
        public bool isSelected;
        public bool IsLock;

        public CellElement(int iIdx, HeroData heroBase, SelectScroll scroll, bool small, UnityAction<CellElement, bool> click, UnityAction<CellElement> longPress, bool isSelect, bool isLock)
        {
            idx = iIdx;
            HeroData = heroBase;
            parent = scroll;
            isSmall = small;
            OnClick = click;
            OnLongPress = longPress;
            isSelected = isSelect;
            IsLock = isLock;

        }
    }

    public class SelectScroll : MonoBehaviour
    {
        public GridLayoutGroup layoutGroup;
        public Canvas canvas;
    }
}