using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gather.Network;
using System;

public class ChattingMessage : MonoBehaviour
{
    public TextMeshProUGUI sender;
    public TextMeshProUGUI time;
    public TextMeshProUGUI content;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMessage(Message message)
    {
        if (message.type == MessageType.Normal)
        {
            sender.text = $"{message.sender} To {message.receiver}";
            time.text = ((DateTime)message.time).ToString("h:mm tt");
            content.text = message.content;
        }
    }
}
