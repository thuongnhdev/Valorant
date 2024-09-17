using MultiFPS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoController : MonoBehaviour
{
    [SerializeField] Button confirmBtn;
    [SerializeField] TMP_InputField inputField;

    private void Awake()
    {
        if(confirmBtn != null)
        {
            confirmBtn.onClick.AddListener(() =>
            {
                ConfirmAction();
            });
        }
    }

    public void InitPop()
    {
        gameObject.SetActive(true);
        inputField.text = UserSettings.UserNickname;
    }

    private void SetUserName()
    {
        if(inputField != null)
        {
            UserSettings.UserNickname = inputField.text;
        }
    }

    private void ConfirmAction()
    {
        SetUserName();
        gameObject.SetActive(false);
    }
}
