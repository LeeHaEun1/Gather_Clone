using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettingButton : MonoBehaviour
{
    public Image enableButton;
    public Image disableButton;

    public Image popupDown;
    public Image popupUp;

    public GameObject popupPanel;

    public void OnEnableButtonClick()
    {
        if (enableButton.enabled)
        {
            enableButton.enabled = false;
            disableButton.enabled = true;
        }
        else
        {
            enableButton.enabled = true;
            disableButton.enabled = false;
        }
    }

    public void OnPopupButtonClick()
    {
        if (popupPanel.activeSelf)
        {
            popupDown.enabled = true;
            popupUp.enabled = false;
            popupPanel.SetActive(false);
        }
        else
        {
            popupDown.enabled = false;
            popupUp.enabled = true;
            popupPanel.SetActive(true);
        }
    }
}
