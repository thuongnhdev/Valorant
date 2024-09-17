using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapePopClose : MonoBehaviour
{
    public Button closeButton;
    public UIMenu parentMenu;
    public UIPopup parentPopup;


    void OnEnable()
    {
        if (parentMenu != null)
        {
            parentMenu.AddEscapePopup(this);
        }
        else if (parentPopup != null)
        {
            parentPopup.AddEscapePopup(this);
        }
    }

    void OnDisable()
    {
        if (parentMenu != null)
        {
            parentMenu.RemoveEscapePopup();
        }
        else if (parentPopup != null)
        {
            parentPopup.RemoveEscapePopup();
        }
    }
}
