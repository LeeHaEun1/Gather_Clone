using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AllocateID : MonoBehaviourPun
{
    PhotonView masterID;

    public void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            MakeID();
        }
    }

    public void MakeID()
    {
        gameObject.AddComponent<PhotonView>();
        PhotonView masterID = this.gameObject.GetComponent<PhotonView>();

        photonView.RPC(nameof(RpcSendID), RpcTarget.OthersBuffered, masterID.ViewID);
    }

    [PunRPC]
    public void RpcSendID(int id)
    {
        GameObject go = GameObject.Find(nameof(this.gameObject.name));
        go.AddComponent<PhotonView>();
        PhotonView goPhotonView = go.GetComponent<PhotonView>();
        goPhotonView.ViewID = id;
    }
}
