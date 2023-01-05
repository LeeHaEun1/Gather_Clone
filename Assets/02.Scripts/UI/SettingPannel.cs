using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPannel : MonoBehaviour
{
    public Image micEnable;
    public Image micDisable;

    public Image cameraEnable;
    public Image cameraDisable;

    public GameObject audioPopup;
    public GameObject videoPopup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickMicEnable()
    {
        if (micEnable.enabled)
        {
            micEnable.enabled = false;
            micDisable.enabled = true;
        }
        else
        {
            micEnable.enabled = true;
            micDisable.enabled = false;
        }
    }

    public void OnClickCameraEnable()
    {
        if (cameraEnable.enabled)
        {
            cameraEnable.enabled = false;
            cameraDisable.enabled = true;
        }
        else
        {
            cameraEnable.enabled = true;
            cameraDisable.enabled = false;
        }
    }

    public void OnAudioPopupButtonClick()
    {
        audioPopup.SetActive(!audioPopup.activeSelf);
    }

    public void OnVideoPopupButtonClick()
    {
        videoPopup.SetActive(!videoPopup.activeSelf);
    }
}
