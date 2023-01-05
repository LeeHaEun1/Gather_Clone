using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;

public class test_Note : MonoBehaviourPun //, IPunObservable
{
    // TextMeshPro - Input Field ������Ʈ ���� ����
    TMP_InputField input;
    // TMP_InputField�� Text ���� ���� -> ���� �ʿ��������..? (input.text�� ���)
    string inputText;


    string before;

    // Start is called before the first frame update
    void Start()
    {
        // TextMeshPro - Input Field ������Ʈ
        input = GetComponent<TMP_InputField>();
        // TMP_InputField�� Text
        //inputText = input.text;


        this.transform.parent = GameObject.Find("Canvas").transform;
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        before = input.text;
    }

    // Update is called once per frame
    void Update()
    {
        //inputText = input.text;

        // ��� Test
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    input.enabled = false;
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    input.text = ""; // �̰ŷ� ��Ʈ ���� ����� ����!!
        //}
        //inputText = input.text;

    }


    // OnValueChanged �ÿ� ȣ����Լ�
    // -> �� �ȿ��� RpcSyncText ȣ�����ٰ���
    // (public �ٿ���� �ν�����â���� ���δ�!!)
    public void OnValueChanged()
    {
        if(input.text == before)
        {
            return;
        }
        //if (photonView.IsMine)
        //{
        //    photonView.RPC("RpcSyncText", RpcTarget.All, input.text);
        //}
        //else
        //{
        //    input.text = input.text;
        //}
        //if (photonView.IsMine)
        //{
        //    photonView.RPC("RpcSyncText", RpcTarget.AllBufferedViaServer, input.text);

        //DateTime time = new DateTime();
        //time.Millisecond
        TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
        

        before = input.text;
        photonView.RPC("RpcSyncText", RpcTarget.AllBuffered, input.text, timeSpan.TotalMilliseconds, PhotonNetwork.LocalPlayer);
        
        //}

        //print($"{GetInstanceID()}aaaaaaaaaaaaaaaaaaaa");
    }

    double lastTimestamp;

    // ������ �����ص� ���ڿ��� ���ؼ� �����ϸ� �������� �ʵ��� �ؾ��ϳ�????
    [PunRPC]
    public void RpcSyncText(string s, double timestamp, Photon.Realtime.Player player)
    {
        if (timestamp <= lastTimestamp)
            return;
        if (input.text == s)
            return;

        input.text = s;
        lastTimestamp = timestamp;


        //print($"{GetInstanceID()} bbbbbbbbbbbbbbbbbbbbbb");
        print($"{player}: {s}");
    }


    // IPunObservable �������̽� ����
    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    //throw new System.NotImplementedException();

    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext(input.text);
    //    }
    //    else
    //    {
    //        input.text = (string)stream.ReceiveNext();
    //    }
    //}
}
