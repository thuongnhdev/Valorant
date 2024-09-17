using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopup : MonoBehaviour
{
	public Animator animator;

	public Canvas canvas;

	[SerializeField]
	protected bool bLoaded = true;

	[SerializeField]
	protected List<EscapePopClose> escapePopList = new List<EscapePopClose>();


	public virtual void Init()
	{
		this.transform.localPosition = Vector3.zero;
		this.transform.localScale = Vector3.one;

		//if (canvas) canvas.worldCamera = MainCamController.instance.CamUI_Base;
	}

	public virtual void Show()
	{
		gameObject.SetActive(true);
		//animator.SetTrigger("ShowPopup");
	}

	public virtual void Hide()
	{
		gameObject.SetActive(false);
		//animator.SetTrigger("HidePopup");
	}

	public virtual void SettingPopup()
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

	public bool IS_Loaded()
	{
		return bLoaded;
	}

	public virtual void OnClick_HidePopup()
	{

	}
}
