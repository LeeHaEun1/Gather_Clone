using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPanel : MonoBehaviour
{
    InterfaceAnimManager interfaceAnimManager;
    
    // Start is called before the first frame update
    void Awake()
    {
        interfaceAnimManager = GetComponent<InterfaceAnimManager>();
    }

    public void Show()
    {
        interfaceAnimManager.startAppear();
    }

    public void Hide()
    {
        interfaceAnimManager.startDisappear();
    }

    public void ShowHide()
    {
        if (interfaceAnimManager.currentState == CSFHIAnimableState.disappeared)
        {
            Show();
        }
        else if (interfaceAnimManager.currentState == CSFHIAnimableState.appeared)
        {
            Hide();
        }
    }
}
