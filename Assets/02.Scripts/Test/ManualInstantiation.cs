using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class ManualInstantiation : MonoBehaviourPun
{
    public byte CustomManualInstantiationEventCode { get; private set; }

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnObject();
        }
    }
    public void SpawnObject()
    {
        //GameObject sceneObject = Instantiate(ObjectPrefab);
        //PhotonView photonView = sceneObject.GetComponent<PhotonView>();
        gameObject.AddComponent<PhotonView>();
        PhotonView photonView = this.gameObject.GetComponent<PhotonView>();
        //photonView.ViewID = PhotonNetwork.AllocateViewID(photonView);

        if (PhotonNetwork.AllocateViewID(photonView))
        {
            object[] data = new object[]
            {
                transform.position, transform.rotation, photonView.ViewID
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            //PhotonNetwork.RaiseEvent()
        }
        else
        {
            Debug.LogError("Failed to allocate a ViewId.");

            //Destroy(gameObject);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == CustomManualInstantiationEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            //GameObject sceneObject = (GameObject)Instantiate(ObjectPrefab, (Vector3)data[0], (Quaternion)data[1]);
            PhotonView photonView = GetComponent<PhotonView>();
            photonView.ViewID = (int)data[2];
        }
    }
}
