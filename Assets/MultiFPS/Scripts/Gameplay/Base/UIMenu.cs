using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using MultiFPS.UI;

public class UIMenu : MonoBehaviour
{
	public Animator animator;

	public RectTransform transBG;

	public Canvas backCanvas;
	public Canvas frontCanvas;

	public Button homeButton;


	[SerializeField]
	protected bool bLoaded = true;

	[SerializeField]
	protected UnityEvent _onEnableEvent;

	[SerializeField]
	protected List<EscapePopClose> escapePopList = new List<EscapePopClose>();

	public bool isRefreshMenu = true;


	public bool IS_Loaded()
	{
		return bLoaded;
	}

	public virtual void Init()
	{
		//if (backCanvas) backCanvas.worldCamera = MainCamController.instance.CamUI_Back;
		//if (frontCanvas) frontCanvas.worldCamera = MainCamController.instance.CamUI_Base;

		if (transBG)
		{
			transBG.sizeDelta = new Vector2(0f, transBG.rect.width);
		}

		if (homeButton != null)
		{
			homeButton.onClick.RemoveAllListeners();
			homeButton.onClick.AddListener(GoHome);
		}
	}

	private void GoHome()
    {
		ClientInterfaceManager.Instance.GoHomeMenu(true);
	}		

	public virtual void Show()
	{
		gameObject.SetActive(true);
		//animator.SetTrigger("ShowMenu");
		_onEnableEvent?.Invoke();
	}

	public virtual void Hide()
	{
		gameObject.SetActive(false);
		//animator.SetTrigger("HideMenu");
	}

	public virtual void SettingMenu()
	{

	}

	public virtual void AddEscapePopup(EscapePopClose escapePopup)
	{
		escapePopList.Add(escapePopup);
	}

	public void RemoveEscapePopup()
	{
		if (escapePopList.Count > 0)
			escapePopList.RemoveAt(escapePopList.Count - 1);
	}

	public bool EscapeActionHidePopup()
	{
		if (escapePopList.Count > 0)
		{
			if (escapePopList[escapePopList.Count - 1].closeButton.gameObject.activeSelf == false || escapePopList[escapePopList.Count - 1].closeButton.enabled == false || escapePopList[escapePopList.Count - 1].closeButton.interactable == false)
				return true;

			escapePopList[escapePopList.Count - 1].closeButton.onClick.Invoke();
			return true;
		}

		return false;
	}

	public virtual void OnReciveAnimationEvent(GameObject ob, string evname)
	{

	}

	public virtual void OnClick_HideMenu(bool clickHide = true)
	{
	}

	public virtual void AllPopupClearHide()
	{
	}
}
