using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;
using System;

public class WebRTCManager : MonoBehaviour
{
    public static WebRTCManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
        WebRTC.Initialize();
    }

    private void OnDestroy()
    {
        WebRTC.Dispose();
    }

    private void Start()
    {
        print("WebRTCManager Start");
        StartCoroutine(WebRTC.Update());

        //ConnectClients();
    }

    private void Update()
    {
        /*if (!connected && Time.time > 3f)
        {
            connected = true;
            ConnectClients();
        }*/
    }
}
