using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrivateAreaPassword : MonoBehaviour
{
    public GameObject passwordPanel;
    public TMP_InputField passwordField;

    private void Start()
    {
        passwordPanel.SetActive(false);
    }

    private void Update()
    {
        if(passwordField.text == "1234")
        {
            passwordPanel.SetActive(false);
            this.gameObject.layer = 21;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            passwordPanel.SetActive(true);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            passwordPanel.SetActive(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            passwordPanel.SetActive(false);
        }
    }
}
