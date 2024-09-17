using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace MultiFPS
{
    [RequireComponent(typeof(ScrollRect))]
    public class ReuseableScroller : MonoBehaviour, IList<object>
    {
        #region IList implementation
        public int IndexOf(object item)
        {
            return innerList.IndexOf(item);
        }
        public void Insert(int index, object item)
        {
            innerList.Insert(index, item);
            if (hasInit)
            {
                RecalculateSize();
                isDirty = true;
            }
        }
        public void RemoveAt(int index)
        {
            innerList.RemoveAt(index);
            if (hasInit)
            {
                RecalculateSize();
                isDirty = true;
            }
        }
        public object this[int index]
        {
            get
            {
                return innerList[index];
            }
            set
            {
                innerList[index] = value;
                if (indexToPair.ContainsKey(index))
                {
                    var pair = indexToPair[index];
                    pair.obj = value;
                    pair.go.SendMessage("OnVisible", pair.obj, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        #endregion
        #region ICollection implementation
        public void Add(object item)
        {
            innerList.Add(item);
            if (hasInit)
            {
                RecalculateSize();
                isDirty = true;
            }
        }
        public void Clear()
        {
            innerList.Clear();
            if (hasInit)
            {
                RecalculateSize();
                isDirty = true;
            }
        }
        public bool Contains(object item)
        {
            return innerList.Contains(item);
        }
        public void CopyTo(object[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }
        public bool Remove(object item)
        {
            var result = innerList.Remove(item);
            if (result && hasInit)
            {
                RecalculateSize();
                isDirty = true;
            }
            return result;
        }
        public int Count
        {
            get
            {
                return innerList.Count;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        float RectWidth { get { return scrollRectTrans.rect.width; } }
        float RectHeight { get { return scrollRectTrans.rect.height; } }
        float ContentWidth { get { return scrollRect.content.rect.width; } }
        float ContentHeight { get { return scrollRect.content.rect.height; } }

        public void RefreshObject(object obj)
        {
            if (!innerList.Contains(obj)) return;
            var index = innerList.IndexOf(obj);
            if (!indexToPair.ContainsKey(index)) return;
            indexToPair[index].go.SendMessage("OnVisible", indexToPair[index].obj, SendMessageOptions.DontRequireReceiver);
        }

        public void RefreshIndex(int index)
        {
            if (!indexToPair.ContainsKey(index)) return;
            indexToPair[index].go.SendMessage("OnVisible", indexToPair[index].obj, SendMessageOptions.DontRequireReceiver);
        }

        public void RefreshAll()
        {
            foreach (var kv in indexToPair)
                kv.Value.go.SendMessage("OnVisible", kv.Value.obj, SendMessageOptions.DontRequireReceiver);
        }

        public void FocusIndex(int index)
        {
            //if it has not init, we have to wait a frame
            if (!hasInit)
                initialFocus = index;
            else
            {
                scrollRect.StopMovement();
                scrollRect.content.anchoredPosition = _focusIndex(index);
                isDirty = true;
            }
            if (index == 0) ForceUpdate();
        }

        public Coroutine FocusIndexAnimation(int index)
        {
            StopAllCoroutines();
            return StartCoroutine(_focusIndexAnimation(index));
        }

        IEnumerator _focusIndexAnimation(int index)
        {
            if (!hasInit)
                yield return null;
            scrollRect.StopMovement();
            var dest = _focusIndex(index);
            while (Vector2.Distance(dest, scrollRect.content.anchoredPosition) > 0.1f)
            {
                scrollRect.content.anchoredPosition = Vector2.Lerp(scrollRect.content.anchoredPosition, dest, Time.deltaTime * 10f);
                yield return null;
            }
            scrollRect.content.anchoredPosition = dest;
        }

        Vector3 _focusIndex(int index)
        {
            if (innerList.Count <= index)
                index = innerList.Count - 1;

            var focusingLine = Mathf.FloorToInt((float)index / itemPerLine);
            if (layoutGroup.startCorner == GridLayoutGroup.Corner.UpperLeft)
            {
                //			if(moveType == Scrollbar.Direction.TopToBottom)
                //			if(moveType == Scrollbar.Direction.LeftToRight)
                if (moveType == Scrollbar.Direction.TopToBottom)
                {
                    var yVal = (index != 0 ? originalPadding.top : 0) + (layoutGroup.cellSize.y + layoutGroup.spacing.y) * focusingLine;
                    if (yVal > ContentHeight - RectHeight)
                        yVal = ContentHeight - RectHeight;
                    if (yVal < 0) yVal = 0;
                    return new Vector2(scrollRect.content.anchoredPosition.x, yVal);
                }
                else if (moveType == Scrollbar.Direction.LeftToRight)
                {
                    var xVal = (index != 0 ? originalPadding.left : 0) + (layoutGroup.cellSize.x + layoutGroup.spacing.x) * focusingLine;
                    if (xVal > ContentWidth - RectWidth)
                        xVal = ContentWidth - RectWidth;
                    if (xVal < 0) xVal = 0;
                    return new Vector2(-xVal, scrollRect.content.anchoredPosition.y);
                }
            }
            else if (layoutGroup.startCorner == GridLayoutGroup.Corner.UpperRight)
            {
                //			if(moveType == Scrollbar.Direction.TopToBottom)
                //			if(moveType == Scrollbar.Direction.RightToLeft)
                if (moveType == Scrollbar.Direction.TopToBottom)
                {
                    var yVal = (index != 0 ? originalPadding.top : 0) + (layoutGroup.cellSize.y + layoutGroup.spacing.y) * focusingLine;
                    if (yVal > ContentHeight - RectHeight)
                        yVal = ContentHeight - RectHeight;
                    if (yVal < 0) yVal = 0;
                    return new Vector2(scrollRect.content.anchoredPosition.x, yVal);
                }
                else if (moveType == Scrollbar.Direction.RightToLeft)
                {
                    var xVal = (index != 0 ? originalPadding.right : 0) + (layoutGroup.cellSize.x + layoutGroup.spacing.x) * focusingLine;
                    if (xVal > ContentWidth - RectWidth)
                        xVal = ContentWidth - RectWidth;
                    if (xVal < 0) xVal = 0;
                    return new Vector2(RectWidth + xVal, scrollRect.content.anchoredPosition.y);
                }
            }
            else if (layoutGroup.startCorner == GridLayoutGroup.Corner.LowerLeft)
            {
                //			if(moveType == Scrollbar.Direction.BottomToTop)
                //			if(moveType == Scrollbar.Direction.LeftToRight)
                if (moveType == Scrollbar.Direction.BottomToTop)
                {
                    var yVal = (index != 0 ? originalPadding.bottom : 0) + (layoutGroup.cellSize.y + layoutGroup.spacing.y) * focusingLine;
                    if (yVal > ContentHeight - RectHeight)
                        yVal = ContentHeight - RectHeight;
                    if (yVal < 0) yVal = 0;
                    return new Vector2(scrollRect.content.anchoredPosition.x, -RectHeight - yVal);
                }
                else if (moveType == Scrollbar.Direction.LeftToRight)
                {
                    var xVal = (index != 0 ? originalPadding.left : 0) + (layoutGroup.cellSize.x + layoutGroup.spacing.x) * focusingLine;
                    if (xVal > ContentWidth - RectWidth)
                        xVal = ContentWidth - RectWidth;
                    if (xVal < 0) xVal = 0;
                    return new Vector2(-xVal, scrollRect.content.anchoredPosition.y);
                }
            }
            else if (layoutGroup.startCorner == GridLayoutGroup.Corner.LowerRight)
            {
                //			if(moveType == Scrollbar.Direction.BottomToTop)
                //			if(moveType == Scrollbar.Direction.RightToLeft)
                if (moveType == Scrollbar.Direction.BottomToTop)
                {
                    var yVal = (index != 0 ? originalPadding.bottom : 0) + (layoutGroup.cellSize.y + layoutGroup.spacing.y) * focusingLine;
                    if (yVal > ContentHeight - RectHeight)
                        yVal = ContentHeight - RectHeight;
                    if (yVal < 0) yVal = 0;
                    return new Vector2(scrollRect.content.anchoredPosition.x, -RectHeight - yVal);
                }
                else if (moveType == Scrollbar.Direction.RightToLeft)
                {
                    var xVal = (index != 0 ? originalPadding.right : 0) + (layoutGroup.cellSize.x + layoutGroup.spacing.x) * focusingLine;
                    if (xVal > ContentWidth - RectWidth)
                        xVal = ContentWidth - RectWidth;
                    if (xVal < 0) xVal = 0;
                    return new Vector2(RectWidth + xVal, scrollRect.content.anchoredPosition.y);
                }
            }
            return scrollRect.content.anchoredPosition;
        }


        #endregion
        #region IEnumerable implementation
        public IEnumerator<object> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }
        #endregion
        #region IEnumerable implementation
        IEnumerator IEnumerable.GetEnumerator()
        {
            return innerList.GetEnumerator();
        }
        #endregion

        public GameObject itemPreset;
        public Vector3 presetScale = Vector3.one;
        public int isCenterCount;
        public UnityAction initAction = null;
        ScrollRect scrollRect;
        RectTransform scrollRectTrans;
        GridLayoutGroup layoutGroup;
        RectOffset originalPadding;
        bool isDirty = true;
        int initialFocus = 0;
        bool hasInit = false;
        Scrollbar.Direction moveType = Scrollbar.Direction.BottomToTop;
        readonly Queue<ObjectKeyPair> itemPool = new Queue<ObjectKeyPair>();
        readonly Dictionary<int, ObjectKeyPair> indexToPair = new Dictionary<int, ObjectKeyPair>();
        private class ObjectKeyPair
        {
            public object obj;
            public GameObject go;
            public ObjectKeyPair(object key, GameObject value)
            {
                obj = key;
                go = value;
            }

            public void Reset()
            {
                obj = null;
            }
        }

        int itemPerLine = 0;
        int maxVisibles = 0;
        int startLineAt = -1;
        List<object> innerList = new List<object>();
        bool isVerticalScroll = false;
        public bool bStart = false;

        void Awake()
        {
            if (bStart == false)
            {
                bStart = true;
                scrollRect = GetComponent<ScrollRect>();
                scrollRect.scrollSensitivity = 50f;
                scrollRectTrans = (RectTransform)scrollRect.transform;
                layoutGroup = scrollRect.content.GetComponent<GridLayoutGroup>();
                originalPadding = layoutGroup.padding;
                if (layoutGroup == null)
                    throw new System.NullReferenceException("There is no GridLayoutGroup in scrollRect's content");
                if (layoutGroup.GetComponent<ContentSizeFitter>() != null)
                    Destroy(layoutGroup.GetComponent<ContentSizeFitter>());
                CalculateLayout(true);
                scrollRect.onValueChanged.AddListener((vec) => OnUpdateScroll());
                Update();
            }
        }

        public void Setting()
        {
            if (bStart == false)
            {
                bStart = true;
                scrollRect = GetComponent<ScrollRect>();
                scrollRect.scrollSensitivity = 50f;
                scrollRectTrans = (RectTransform)scrollRect.transform;
                layoutGroup = scrollRect.content.GetComponent<GridLayoutGroup>();
                originalPadding = layoutGroup.padding;
                if (layoutGroup == null)
                    throw new System.NullReferenceException("There is no GridLayoutGroup in scrollRect's content");
                if (layoutGroup.GetComponent<ContentSizeFitter>() != null)
                    Destroy(layoutGroup.GetComponent<ContentSizeFitter>());
                CalculateLayout(true);
                scrollRect.onValueChanged.AddListener((vec) => OnUpdateScroll());
                Update();
            }
        }

        public void FollowParentCellSizeWidth()
        {
            RectTransform rt = layoutGroup.gameObject.GetComponent<RectTransform>();
            layoutGroup.cellSize = new Vector2(rt.sizeDelta.x, layoutGroup.cellSize.y);
        }

        public void LockScrollRect(bool bActive)
        {
            scrollRect.enabled = bActive;
        }

        //	void OnRectTransformDimensionsChange()
        //	{
        //		if(hasInit) 
        //		{
        //			var maxVisibleCache = maxVisibles;
        //			CalculateLayout();
        //			if(maxVisibles != maxVisibleCache)
        //				isDirty = true;
        //		}
        //	}

        void CalculateLayout(bool initPosition = false)
        {
            var sizeCache = new Vector2(ContentWidth, ContentHeight);
            scrollRect.content.anchorMax = new Vector2(0, 1);
            scrollRect.content.anchorMin = new Vector2(0, 1);
            scrollRect.content.sizeDelta = sizeCache;

            if (layoutGroup.constraint != GridLayoutGroup.Constraint.Flexible)
            {
                layoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
                scrollRect.content.pivot = new Vector2(0f, 1f);
                if (initPosition) scrollRect.content.anchoredPosition = new Vector2(0, 0);
                isVerticalScroll = layoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount;
                moveType = isVerticalScroll ? Scrollbar.Direction.TopToBottom : Scrollbar.Direction.LeftToRight;
                itemPerLine = layoutGroup.constraintCount;
            }
            else
            {
                if (layoutGroup.startCorner == GridLayoutGroup.Corner.UpperLeft)
                {
                    scrollRect.content.pivot = new Vector2(0f, 1f);
                    if (layoutGroup.startAxis == GridLayoutGroup.Axis.Horizontal)
                        moveType = Scrollbar.Direction.TopToBottom;
                    else
                        moveType = Scrollbar.Direction.LeftToRight;
                    if (initPosition) scrollRect.content.anchoredPosition = new Vector2(0, 0);
                    layoutGroup.childAlignment = TextAnchor.UpperLeft;
                }
                else if (layoutGroup.startCorner == GridLayoutGroup.Corner.UpperRight)
                {
                    scrollRect.content.pivot = new Vector2(1f, 1f);
                    if (layoutGroup.startAxis == GridLayoutGroup.Axis.Horizontal)
                        moveType = Scrollbar.Direction.TopToBottom;
                    else
                        moveType = Scrollbar.Direction.RightToLeft;
                    if (initPosition) scrollRect.content.anchoredPosition = new Vector2(RectWidth, 0);
                    layoutGroup.childAlignment = TextAnchor.UpperRight;
                }
                else if (layoutGroup.startCorner == GridLayoutGroup.Corner.LowerLeft)
                {
                    scrollRect.content.pivot = new Vector2(0f, 0f);
                    if (layoutGroup.startAxis == GridLayoutGroup.Axis.Horizontal)
                        moveType = Scrollbar.Direction.BottomToTop;
                    else
                        moveType = Scrollbar.Direction.LeftToRight;
                    if (initPosition) scrollRect.content.anchoredPosition = new Vector2(0, -RectHeight);
                    layoutGroup.childAlignment = TextAnchor.LowerLeft;
                }
                else
                {
                    scrollRect.content.pivot = new Vector2(1f, 0f);
                    if (layoutGroup.startAxis == GridLayoutGroup.Axis.Horizontal)
                        moveType = Scrollbar.Direction.BottomToTop;
                    else
                        moveType = Scrollbar.Direction.RightToLeft;
                    if (initPosition) scrollRect.content.anchoredPosition = new Vector2(RectWidth, -RectHeight);
                    layoutGroup.childAlignment = TextAnchor.LowerRight;
                }

                isVerticalScroll = moveType == Scrollbar.Direction.BottomToTop || moveType == Scrollbar.Direction.TopToBottom;
                itemPerLine = CalCulateCellCountPerLine();
            }

            maxVisibles = CalCulateMaxVisibles();


            RecalculateSize();
            if (!hasInit)
            {
                hasInit = true;
                FocusIndex(initialFocus);
            }
        }

        void OnUpdateScroll()
        {
            var newIndex = calculateStartIndex();
            if (newIndex < 0)
                newIndex = 0;
            if (newIndex != startLineAt || isDirty)
                UpdateScrollItems(newIndex);
        }

        public void ForceUpdate()
        {
            if (hasInit) Update();
        }

        void Update()
        {
            if (isDirty)
            {
                initAction?.Invoke();
                initAction = null;
                OnUpdateScroll();
            }
        }

        void RecalculateSize()
        {
            var maxLines = Mathf.CeilToInt((float)Count / itemPerLine);
            if (isVerticalScroll)
            {
                var height = maxLines * layoutGroup.cellSize.y + originalPadding.vertical + (maxLines - 1) * layoutGroup.spacing.y;
                scrollRect.content.sizeDelta = new Vector2(ContentWidth, height);
            }
            else
            {
                if (isCenterCount > 0 && maxLines <= isCenterCount)
                {
                    maxLines = isCenterCount;

                }
                var width = maxLines * layoutGroup.cellSize.x + originalPadding.horizontal + (maxLines - 1) * layoutGroup.spacing.x;
                scrollRect.content.sizeDelta = new Vector2(width, ContentHeight);
            }
        }

        void UpdateScrollItems(int newIndex)
        {
            if (isVerticalScroll)
            {
                var lineHeight = (layoutGroup.cellSize.y + layoutGroup.spacing.y) * newIndex;
                if (moveType == Scrollbar.Direction.BottomToTop)
                    layoutGroup.padding = new RectOffset(originalPadding.left, originalPadding.right, originalPadding.top, (int)lineHeight + originalPadding.bottom);
                else
                    layoutGroup.padding = new RectOffset(originalPadding.left, originalPadding.right, (int)lineHeight + originalPadding.top, originalPadding.bottom);
            }
            else
            {

                var lineWidth = (layoutGroup.cellSize.x + layoutGroup.spacing.x) * newIndex;
                if (moveType == Scrollbar.Direction.LeftToRight)
                    layoutGroup.padding = new RectOffset((int)lineWidth + originalPadding.left, originalPadding.right, originalPadding.top, originalPadding.bottom);
                else
                    layoutGroup.padding = new RectOffset(originalPadding.left, (int)lineWidth + originalPadding.right, originalPadding.top, originalPadding.bottom);
            }

            startLineAt = newIndex;

            var startIndex = newIndex * itemPerLine;
            var endIndex = startIndex + maxVisibles;

            var unusingIndex = new Queue<int>();
            foreach (var kv in indexToPair)
                if (kv.Key < startIndex || kv.Key >= endIndex || kv.Key >= Count)
                    unusingIndex.Enqueue(kv.Key);

            int sublingIndex = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (i >= Count) continue;

                if (indexToPair.ContainsKey(i))
                {
                    indexToPair[i].go.transform.SetSiblingIndex(sublingIndex);
                    if (indexToPair[i].obj != innerList[i])
                    {
                        indexToPair[i].obj = innerList[i];
                        indexToPair[i].go.SendMessage("OnVisible", indexToPair[i].obj, SendMessageOptions.DontRequireReceiver);
                    }
                }
                else if (unusingIndex.Count > 0)
                {
                    var oldIndex = unusingIndex.Dequeue();
                    var old = indexToPair[oldIndex];
                    indexToPair.Remove(oldIndex);
                    old.obj = innerList[i];
                    indexToPair.Add(i, old);
                    old.go.SendMessage("OnVisible", old.obj, SendMessageOptions.DontRequireReceiver);
                    old.go.transform.SetSiblingIndex(sublingIndex);
                }
                else if (itemPool.Count > 0)
                {
                    var newObjPair = itemPool.Dequeue();
                    newObjPair.go.SetActive(true);
                    newObjPair.obj = innerList[i];
                    indexToPair.Add(i, newObjPair);
                    newObjPair.go.SendMessage("OnVisible", newObjPair.obj, SendMessageOptions.DontRequireReceiver);
                    newObjPair.go.transform.SetSiblingIndex(sublingIndex);
                }
                else
                {
                    var newObj = Instantiate(itemPreset) as GameObject;
                    newObj.transform.SetParent(scrollRect.content);
                    newObj.transform.localScale = presetScale;
                    newObj.transform.localPosition = Vector3.zero;
                    newObj.SetActive(true);
                    var newPair = new ObjectKeyPair(innerList[i], newObj);
                    indexToPair.Add(i, newPair);
                    newPair.go.SendMessage("OnVisible", newPair.obj, SendMessageOptions.DontRequireReceiver);
                    newPair.go.transform.SetSiblingIndex(sublingIndex);
                }
                sublingIndex++;
            }

            while (unusingIndex.Count > 0)
            {
                var poolindex = unusingIndex.Dequeue();
                var toPool = indexToPair[poolindex];
                toPool.go.SetActive(false);
                toPool.Reset();
                itemPool.Enqueue(toPool);
                indexToPair.Remove(poolindex);
            }

            isDirty = false;
        }


        int calculateStartIndex()
        {
            if (isCenterCount > 0)
            {
                if (innerList.Count <= isCenterCount)
                {
                    layoutGroup.childAlignment = TextAnchor.UpperCenter;
                    if (isVerticalScroll)
                    {
                        scrollRect.vertical = false;
                    }
                    else
                    {
                        scrollRect.horizontal = false;
                    }
                }
                else
                {
                    layoutGroup.childAlignment = TextAnchor.UpperLeft;
                    if (isVerticalScroll)
                    {
                        scrollRect.vertical = true;
                    }
                    else
                    {
                        scrollRect.horizontal = true;
                    }
                }
            }

            //if (isCenterCount > 0 && innerList.Count <= isCenterCount)
            //{
            //	layoutGroup.childAlignment = TextAnchor.UpperCenter;
            //	if (isVerticalScroll)
            //	{
            //		scrollRect.vertical = false;
            //	}
            //	else
            //	{
            //		scrollRect.horizontal = false;
            //	}
            //}
            //else if (isCenterCount > 0)
            //{
            //	layoutGroup.childAlignment = TextAnchor.UpperLeft;
            //	if(isVerticalScroll)
            //          {
            //		scrollRect.vertical = true;
            //	}
            //          else
            //          {
            //		scrollRect.horizontal = true;
            //	}
            //}

            switch (moveType)
            {
                case Scrollbar.Direction.BottomToTop:
                    var movedFromTop = -RectHeight - scrollRect.content.anchoredPosition.y - originalPadding.bottom;
                    return Mathf.FloorToInt(movedFromTop / (layoutGroup.cellSize.y + layoutGroup.spacing.y));
                case Scrollbar.Direction.TopToBottom:
                    var movedFromBot = scrollRect.content.anchoredPosition.y - originalPadding.top;
                    return Mathf.FloorToInt(movedFromBot / (layoutGroup.cellSize.y + layoutGroup.spacing.y));
                case Scrollbar.Direction.LeftToRight:
                    var movedFromLeft = -scrollRect.content.anchoredPosition.x - originalPadding.left;
                    return Mathf.FloorToInt(movedFromLeft / (layoutGroup.cellSize.x + layoutGroup.spacing.x));
                default:
                    var movedFromRight = scrollRect.content.anchoredPosition.x - RectWidth - originalPadding.right;
                    return Mathf.FloorToInt(movedFromRight / (layoutGroup.cellSize.x + layoutGroup.spacing.x));
            }
        }

        int CalCulateCellCountPerLine()
        {
            if (!isVerticalScroll)
            {
                //add x for spacing between cells
                var lineSize = ContentHeight - originalPadding.vertical + layoutGroup.spacing.y;
                var line = Mathf.FloorToInt(lineSize / (layoutGroup.cellSize.y + layoutGroup.spacing.y));
                if (line == 0)
                {
                    line = 1;
                }
                return line;
            }
            else
            {
                //add x for spacing between cells
                var lineSize = ContentWidth - originalPadding.horizontal + layoutGroup.spacing.x;
                var line = Mathf.FloorToInt(lineSize / (layoutGroup.cellSize.x + layoutGroup.spacing.x));
                if (line == 0)
                {
                    line = 1;
                }
                return line;
            }
        }

        int CalCulateMaxVisibles()
        {
            if (isVerticalScroll)
                return (Mathf.CeilToInt(RectHeight / (layoutGroup.cellSize.y + layoutGroup.spacing.y)) + 1) * itemPerLine;
            else
                return (Mathf.CeilToInt(RectWidth / (layoutGroup.cellSize.x + layoutGroup.spacing.x)) + 1) * itemPerLine;
        }
    }
}