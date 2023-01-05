using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;

public class test_Note : MonoBehaviourPun //, IPunObservable
{
    // TextMeshPro - Input Field 컴포넌트 담을 변수
    TMP_InputField input;
    // TMP_InputField의 Text 담을 변수 -> 굳이 필요없을수도..? (input.text로 사용)
    string inputText;


    string before;

    // Start is called before the first frame update
    void Start()
    {
        // TextMeshPro - Input Field 컴포넌트
        input = GetComponent<TMP_InputField>();
        // TMP_InputField의 Text
        //inputText = input.text;


        this.transform.parent = GameObject.Find("Canvas").transform;
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        before = input.text;
    }

    // Update is called once per frame
    void Update()
    {
        //inputText = input.text;

        // 대상 Test
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    input.enabled = false;
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    input.text = ""; // 이거로 노트 내용 지우기 성공!!
        //}
        //inputText = input.text;

    }


    // OnValueChanged 시에 호출될함수
    // -> 이 안에서 RpcSyncText 호출해줄것임
    // (public 붙여줘야 인스펙터창에서 보인다!!)
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

    // 직전에 저장해둔 문자열과 비교해서 동일하면 실행하지 않도록 해야하나????
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


    // IPunObservable 인터페이스 구현
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
