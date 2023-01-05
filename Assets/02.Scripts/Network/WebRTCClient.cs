using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using System;
using Photon.Pun;
using Gather.Data;
using Gather.Manager;
using Mediapipe.SelfieSegmentation;
using Player = Photon.Realtime.Player;

namespace Gather.Network
{    
    public enum MessageType
    {
        Normal,
    }

    [Serializable]
    public class Message : ISerializationCallbackReceiver
    {
        public MessageType type;
        public string sender;
        public string receiver;
        public string content;
        public DateTime time;
        [SerializeField] private JsonDateTime jsonDateTime;

        public void OnAfterDeserialize()
        {
            time = jsonDateTime;
            time = time.ToLocalTime();
        }

        public void OnBeforeSerialize()
        {
            jsonDateTime = time;
        }
    }

    public class WebRTCClient : MonoBehaviourPun
    {
        [SerializeField] private Camera cam;
        [SerializeField] private AudioClip clip;
        //[SerializeField] private RawImage sourceImage;
        [SerializeField] private PreviewImage sourceImage;
        [SerializeField] private AudioSource sourceAudio;
        //[SerializeField] private RawImage receiveImage;
        [SerializeField] private MeshRenderer receiveImage;
        [SerializeField] public AudioSource receiveAudio;
        [SerializeField] private AudioClip audioclipStereoSample;

        private RTCPeerConnection peerConnection;
        private List<RTCRtpSender> rtpSenders = new List<RTCRtpSender>();
        private VideoStreamTrack videoStreamTrack;
        private AudioStreamTrack audioStreamTrack;
        private MediaStream receiveAudioStream, receiveVideoStream;
        private DelegateOnIceConnectionChange onIceConnectionChange;
        private DelegateOnIceCandidate onIceCandidate;
        private DelegateOnTrack ontrack;
        private DelegateOnNegotiationNeeded onNegotiationNeeded;
        private WebCamTexture webCamTexture;

        private RTCDataChannel sendMessageChannel;
        private RTCDataChannel receiveMessageChannel;

        public Texture2D errorImage;

        Dictionary<string, RTCPeerConnection> peerConnections = new Dictionary<string, RTCPeerConnection>();

        private MediaStream sourceStream;
        public string ownerID;

        public UnityEvent<WebRTCClient> onDestroy;
        public UnityEvent<Message> onReceiveMessage;

        static bool printDebug = false;

        SelfieSegmentation segmentation;
        [SerializeField] SelfieSegmentationResource segmentResource;
        public RawImage segmentImage;
        public Texture receiveTexture;

        // 삭제
        public Material holo_mat;

        public RTCPeerConnection PeerConnection
        {
            get
            {
                return peerConnection;
            }
        }

        public float DistanceFromPlayer
        {
            get
            {
                return Vector3.Distance(transform.position, GameManager.Instance.characterController.player.transform.position);
            }
        }

        private void Awake()
        {
            //WebRTC.Initialize(WebRTCSettings.LimitTextureSize);
            ownerID = photonView.Owner.UserId;
            //photonView.RPC(nameof(Call), photonView.Controller, ownerID);
        }

        void Start()
        {
            //WebRTCManager.Instance.AddClient(this);

            if (photonView.IsMine)
            {
                sourceStream = new MediaStream();
                sourceImage = GameObject.FindWithTag("PreviewImage").GetComponent<PreviewImage>();
                //Setup();
                StartCoroutine(CaptureAudioStart());
                StartCoroutine(CaptureVideoStart());
                OnHologramTemplateChange();
            }
            else
            {
                segmentation = new SelfieSegmentation(segmentResource);
            }
            //PhotonNetwork.LocalPlayer.UserId
            //Connect();
            //Test();
            if (!photonView.IsMine)
            {
                //StartCoroutine(TestMessageSend());
            }
        }

        private void Update()
        {

            //receiveImage.texture = tex;
            if (!photonView.IsMine)
            {
                receiveTexture = receiveImage.material.GetTexture("_BaseMap");
                if (receiveTexture != null)
                {
                    segmentation.ProcessImage(receiveTexture);
                    receiveImage.material.SetTexture("_SegmentMask", segmentation.texture);
                }
            }
            
            //print("update: " + segmentation.texture.updateCount);
            //Debug.LogAssertion("update: " + tex.name);
            //receiveImage.material.SetTexture("_SegmentMask", segmentation.texture);
            //segmentImage.texture = segmentation.texture;
        }

        IEnumerator TestMessageSend()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                Message message = new Message();
                message.type = MessageType.Normal;
                message.content = "Hello";
                SendDataChannel(JsonUtility.ToJson(message));
            }
        }

        private void Setup()
        {
            if (printDebug) Debug.Log("Set up source/receive streams");
            var videoTrack = cam.CaptureStreamTrack(VideoSetting.StreamSize.x, VideoSetting.StreamSize.y, 0);
            sourceStream.AddTrack(videoTrack);
            //receiveImage.texture = cam.targetTexture;
            receiveImage.material.SetTexture("_BaseMap", cam.targetTexture);

            /*sourceAudio.clip = audioclipStereoSample;
            sourceAudio.loop = true;
            sourceAudio.Play();
            var audioTrack = new AudioStreamTrack(sourceAudio);
            sourceStream.AddTrack(audioTrack);*/
        }

        public void OnHologramTemplateChange()
        {
            Debug.LogAssertion($"{GetInstanceID()} OnHologramTemplateChange");
            photonView.RPC(nameof(OnHologramTemplateChangeRPC), RpcTarget.OthersBuffered, VideoSetting.hologramTamplateIndex);
        }

        [PunRPC]
        public void OnHologramTemplateChangeRPC(int index)
        {
            Debug.LogAssertion($"{GetInstanceID()} OnHologramTemplateChangeRPC:"+index);
            Transform _transform = receiveImage.gameObject.transform;
            HologramTamplate tamplate = VideoSetting.hologramTamplates[index];
            _transform.localScale = new Vector3(tamplate.x, tamplate.y, 1);
            _transform.localPosition = new Vector3(0, tamplate.y / 2, 0);
        }

        public bool SendMessage(Message message)
        {
            return SendDataChannel(JsonUtility.ToJson(message));
        }

        bool SendDataChannel(string message)
        {
            if (sendMessageChannel.ReadyState == RTCDataChannelState.Open)
            {
                sendMessageChannel.Send(message);
                return true;
            }
            return false;
        }

        bool SendDataChannel(byte[] bytes)
        {
            sendMessageChannel.Send(bytes);
            return true;
        }

        void HandleReceiveMessage(byte[] bytes)
        {
            string str = System.Text.Encoding.UTF8.GetString(bytes);
            Message message = JsonUtility.FromJson<Message>(str);
            Debug.Log(message.content);
            onReceiveMessage.Invoke(message);
        }

        public void Connect()
        {
            if (!photonView.IsMine && peerConnection == null)
                StartCoroutine(CreatOfferInit());
        }

        public void Disconnect()
        {
            if (peerConnection != null)
            {
                //receiveImage.texture = null;
                receiveImage.gameObject.SetActive(false);
                receiveAudio.clip = null;
                receiveAudio.gameObject.SetActive(false);

                StopAllCoroutines();
                peerConnection.Close();
                peerConnection.Dispose();
                peerConnection = null;
                photonView.RPC(nameof(RemoteDisconnect), photonView.Controller, PhotonNetwork.LocalPlayer);
            }
        }

        [PunRPC]
        public void RemoteDisconnect(Photon.Realtime.Player fromPlayer)
        {
            if (peerConnections.ContainsKey(fromPlayer.UserId))
            {
                peerConnections[fromPlayer.UserId].Close();
                peerConnections[fromPlayer.UserId].Dispose();
                peerConnections[fromPlayer.UserId] = null;
                peerConnections.Remove(fromPlayer.UserId);
            }
        }

        RTCPeerConnection CreateLocalPeer()
        {
            Debug.LogAssertion($"rtcConfiguration: {NetworkSetting.rtcConfiguration}");
            RTCPeerConnection pc = new RTCPeerConnection(ref NetworkSetting.rtcConfiguration);
            sendMessageChannel = pc.CreateDataChannel("message");
            sendMessageChannel.OnOpen = () =>
            {
                if (printDebug) Debug.Log($"{GetInstanceID()} messageChannel open");
            };
            sendMessageChannel.OnClose = () =>
            {
                if (printDebug) Debug.Log($"{GetInstanceID()} messageChannel close");
            };

            pc.OnIceCandidate = candidate =>
            {
                OnIceCandidate(candidate, photonView.Controller);
            };
            pc.OnIceConnectionChange = state =>
            {
                OnIceConnectionChange(pc, state);
            };
            pc.OnTrack = e =>
            {
                OnTrack(e);
            };

            pc.OnConnectionStateChange = state =>
            {
                if (printDebug) Debug.Log($"1 {GetInstanceID()} OnConnectionStateChange: {state}");
            };
            pc.OnNegotiationNeeded = () =>
            {
                StartCoroutine(CreatOffer(pc, photonView.Controller));
            };
            return pc;
        }

        RTCPeerConnection CreateRemotePeer(Photon.Realtime.Player fromPlayer)
        {
            RTCPeerConnection pc = new RTCPeerConnection(ref NetworkSetting.rtcConfiguration);
            pc.OnDataChannel = channel =>
            {
                if (printDebug) print("OnDataChannel");
                receiveMessageChannel = channel;
                receiveMessageChannel.OnMessage = HandleReceiveMessage;
            };

            pc.OnIceCandidate = candidate =>
            {
                OnIceCandidate(candidate, fromPlayer);
            };
            pc.OnIceGatheringStateChange = state =>
            {
                if (printDebug) Debug.Log($"2 {GetInstanceID()} OnIceGatheringStateChange {state}");
            };
            pc.OnIceConnectionChange = state =>
            {
                OnIceConnectionChange(pc, state);
            };
            pc.OnTrack = e =>
            {
                OnTrack(e);
            };

            pc.OnConnectionStateChange = state =>
            {
                if (printDebug) Debug.Log($"2 {GetInstanceID()} OnConnectionStateChange: {state}");
            };
            pc.OnNegotiationNeeded = () =>
            {
                StartCoroutine(CreatOffer(pc, fromPlayer));
            };
            peerConnections.Add(fromPlayer.UserId, pc);
            return pc;
        }

        public IEnumerator CreatOfferInit()
        {
            peerConnection = CreateLocalPeer();

            var op = peerConnection.CreateOffer();
            yield return op;

            if (!op.IsError)
            {
                if (peerConnection.SignalingState != RTCSignalingState.Stable)
                {
                    if (printDebug) Debug.LogError($"{GetInstanceID()} signaling state is not stable.");
                    yield break;
                }
                RTCSessionDescription desc = op.Desc;

                var op2 = peerConnection.SetLocalDescription(ref desc);
                yield return op2;

                if (!op2.IsError)
                {
                    OnSetLocalSuccess(peerConnection);
                    photonView.RPC(nameof(CreateAnswerInit), photonView.Controller, JsonUtility.ToJson(desc), PhotonNetwork.LocalPlayer);
                }
                else
                {
                    var error = op2.Error;
                    OnSetSessionDescriptionError(ref error);
                }
            }
            else
            {
                OnCreateSessionDescriptionError(op.Error);
            }
        }

        [PunRPC]
        public void CreateAnswerInit(string description, Photon.Realtime.Player fromPlayer)
        {
            if (photonView.IsMine)
                StartCoroutine(OnCreateAnswerInit(JsonUtility.FromJson<RTCSessionDescription>(description), fromPlayer));
        }

        public IEnumerator OnCreateAnswerInit(RTCSessionDescription description, Photon.Realtime.Player fromPlayer)
        {
            RTCPeerConnection pc = new RTCPeerConnection(ref NetworkSetting.rtcConfiguration);
            pc.OnDataChannel = channel =>
            {
                receiveMessageChannel = channel;
                receiveMessageChannel.OnMessage = HandleReceiveMessage;
            };

            pc.OnIceCandidate = candidate =>
            {
                if (printDebug) Debug.Log($"2 {GetInstanceID()} OnIceCandidate");
                OnIceCandidate(candidate, fromPlayer);
            };
            pc.OnIceGatheringStateChange = state =>
            {
                if (printDebug) Debug.Log($"2 {GetInstanceID()} OnIceGatheringStateChange {state}");
            };
            pc.OnIceConnectionChange = state =>
            {
                if (printDebug) Debug.Log($"2 {GetInstanceID()} OnIceConnectionChange");
                OnIceConnectionChange(pc, state);
            };
            pc.OnTrack = e =>
            {
                if (printDebug) Debug.Log($"2 {GetInstanceID()} OnTrack");
                OnTrack(e);
            };

            pc.OnConnectionStateChange = state =>
            {
                if (printDebug) Debug.Log($"2 {GetInstanceID()} OnConnectionStateChange: {state}");
            };
            pc.OnNegotiationNeeded = () =>
            {
                if (printDebug) Debug.Log($"2 {GetInstanceID()} OnNegotiationNeeded");
                StartCoroutine(CreatOffer(pc, fromPlayer));
            };

            var op = pc.SetRemoteDescription(ref description);
            yield return op;
            if (!op.IsError)
            {
                OnSetRemoteSuccess(pc);
                var op2 = pc.CreateAnswer();
                yield return op2;
                if (!op2.IsError)
                {
                    var desc = op2.Desc;
                    if (printDebug) Debug.Log($"Answer from {GetInstanceID()}:\n{desc.sdp}");
                    if (printDebug) Debug.Log($"{GetInstanceID()} setLocalDescription start");
                    var op3 = pc.SetLocalDescription(ref desc);
                    yield return op3;

                    if (!op3.IsError)
                    {
                        OnSetLocalSuccess(pc);
                        photonView.RPC(nameof(ReceiveAnswerInit), fromPlayer, JsonUtility.ToJson(desc));
                        peerConnections.Add(fromPlayer.UserId, pc);
                    }
                    else
                    {
                        var error = op3.Error;
                        OnSetSessionDescriptionError(ref error);
                    }
                }
                else
                {
                    OnCreateSessionDescriptionError(op2.Error);
                }
            }
            else
            {
                var error = op.Error;
                OnSetSessionDescriptionError(ref error);
            }
        }

        [PunRPC]
        public void ReceiveAnswerInit(string desc)
        {
            if (!photonView.IsMine)
                StartCoroutine(OnReceiveAnswerInit(JsonUtility.FromJson<RTCSessionDescription>(desc)));
        }

        public IEnumerator OnReceiveAnswerInit(RTCSessionDescription desc)
        {
            var op2 = peerConnection.SetRemoteDescription(ref desc);
            yield return op2;
            if (!op2.IsError)
            {
                OnSetRemoteSuccess(peerConnection);
                photonView.RPC(nameof(AddTracks), photonView.Controller, PhotonNetwork.LocalPlayer);
            }
            else
            {
                var error = op2.Error;
                OnSetSessionDescriptionError(ref error);
            }
        }

        [PunRPC]
        public void AddTracks(Photon.Realtime.Player fromPlayer)
        {
            var pc1VideoSenders = new List<RTCRtpSender>();
            foreach (var track in sourceStream.GetTracks())
            {
                var pc1Sender = peerConnections[fromPlayer.UserId].AddTrack(track, sourceStream);

                if (track.Kind == TrackKind.Video)
                {
                    pc1VideoSenders.Add(pc1Sender);
                }
            }

            if (VideoSetting.UseVideoCodec != null)
            {
                var codecs = new[] { VideoSetting.UseVideoCodec };
                foreach (var transceiver in peerConnections[fromPlayer.UserId].GetTransceivers())
                {
                    if (pc1VideoSenders.Contains(transceiver.Sender))
                    {
                        transceiver.SetCodecPreferences(codecs);
                    }
                }
            }
        }

        public void AddTracks()
        {
            var pc1VideoSenders = new List<RTCRtpSender>();
            foreach (var track in sourceStream.GetTracks())
            {
                foreach (var pc in peerConnections.Values)
                {
                    var pc1Sender = pc.AddTrack(track, sourceStream);

                    if (track.Kind == TrackKind.Video)
                    {
                        pc1VideoSenders.Add(pc1Sender);
                    }
                }
            }

            if (VideoSetting.UseVideoCodec != null)
            {
                var codecs = new[] { VideoSetting.UseVideoCodec };

                foreach (var pc in peerConnections.Values)
                {
                    foreach (var transceiver in pc.GetTransceivers())
                    {
                        if (pc1VideoSenders.Contains(transceiver.Sender))
                        {
                            transceiver.SetCodecPreferences(codecs);
                        }
                    }
                }
            }
        }

        public IEnumerator CreatOffer(RTCPeerConnection pc, Photon.Realtime.Player toPlayer)
        {
            var op = pc.CreateOffer();
            yield return op;

            if (!op.IsError)
            {
                if (pc.SignalingState != RTCSignalingState.Stable)
                {
                    if (printDebug) Debug.LogError($"{GetInstanceID()} signaling state is not stable.");
                    yield break;
                }
                RTCSessionDescription desc = op.Desc;

                var op2 = pc.SetLocalDescription(ref desc);
                yield return op2;

                if (!op2.IsError)
                {
                    OnSetLocalSuccess(pc);
                    photonView.RPC(nameof(CreateAnswer), toPlayer, JsonUtility.ToJson(desc), PhotonNetwork.LocalPlayer);
                }
                else
                {
                    var error = op2.Error;
                    OnSetSessionDescriptionError(ref error);
                }
            }
            else
            {
                OnCreateSessionDescriptionError(op.Error);
            }
        }

        [PunRPC]
        public void CreateAnswer(string description, Photon.Realtime.Player fromPlayer)
        {
            StartCoroutine(OnCreateAnswer(JsonUtility.FromJson<RTCSessionDescription>(description), fromPlayer));
        }

        public IEnumerator OnCreateAnswer(RTCSessionDescription description, Photon.Realtime.Player fromPlayer)
        {
            RTCPeerConnection pc;
            if (photonView.IsMine)
            {
                if (peerConnections.ContainsKey(fromPlayer.UserId))
                {
                    pc = peerConnections[fromPlayer.UserId];
                }
                else
                {
                    pc = CreateRemotePeer(fromPlayer);
                }
            }
            else
            {
                pc = peerConnection;
            }

            var op = pc.SetRemoteDescription(ref description);
            yield return op;
            if (!op.IsError)
            {
                OnSetRemoteSuccess(pc);
                var op2 = pc.CreateAnswer();
                yield return op2;
                if (!op2.IsError)
                {
                    var desc = op2.Desc;
                    var op3 = pc.SetLocalDescription(ref desc);
                    yield return op3;

                    if (!op3.IsError)
                    {
                        OnSetLocalSuccess(pc);
                        photonView.RPC(nameof(ReceiveAnswer), fromPlayer, JsonUtility.ToJson(desc), PhotonNetwork.LocalPlayer);
                    }
                    else
                    {
                        var error = op3.Error;
                        OnSetSessionDescriptionError(ref error);
                    }
                }
                else
                {
                    OnCreateSessionDescriptionError(op2.Error);
                }
            }
            else
            {
                var error = op.Error;
                OnSetSessionDescriptionError(ref error);
            }
        }

        [PunRPC]
        public void ReceiveAnswer(string desc, Photon.Realtime.Player fromPlayer)
        {
            if (printDebug) print($"ReceiveAnswer {fromPlayer}\n {desc}");
            StartCoroutine(OnReceiveAnswer(JsonUtility.FromJson<RTCSessionDescription>(desc), fromPlayer));
        }

        public IEnumerator OnReceiveAnswer(RTCSessionDescription desc, Photon.Realtime.Player fromPlayer)
        {
            if (printDebug) Debug.Log($"{GetInstanceID()} setRemoteDescription start");
            RTCPeerConnection pc;
            if (photonView.IsMine)
            {
                if (peerConnections.ContainsKey(fromPlayer.UserId))
                {
                    pc = peerConnections[fromPlayer.UserId];
                }
                else
                {
                    pc = CreateRemotePeer(fromPlayer);
                    
                }
            }
            else
            {
                pc = peerConnection;
            }
            var op2 = pc.SetRemoteDescription(ref desc);
            yield return op2;
            if (!op2.IsError)
            {
                OnSetRemoteSuccess(pc);
                if (!photonView.IsMine)
                    photonView.RPC(nameof(AddTracks), photonView.Controller, PhotonNetwork.LocalPlayer);
            }
            else
            {
                var error = op2.Error;
                OnSetSessionDescriptionError(ref error);
            }
        }


        void OnTrack(RTCTrackEvent e)
        {
            if (e.Track is VideoStreamTrack video)
            {
                Debug.LogAssertion("OnTrack video");
                receiveImage.gameObject.SetActive(true);
                video.OnVideoReceived += tex =>
                {
                    //receiveImage.texture = tex;
                    receiveImage.material.SetTexture("_BaseMap", tex);
                    //segmentation.ProcessImage(tex);
                    //print("update: " + segmentation.texture.updateCount);
                    Debug.LogAssertion("update: " + tex.name);
                    //receiveImage.material.SetTexture("_SegmentMask", segmentation.texture);
                    //segmentImage.texture = segmentation.texture;
                };
                IEnumerable<MediaStream> streams = e.Streams;
                if (streams.Count() > 0)
                {
                    receiveVideoStream = streams.First();
                    receiveVideoStream.OnRemoveTrack = ev =>
                    {
                        //receiveImage.texture = null;
                        ev.Track.Dispose();
                    };
                }
            }

            if (e.Track is AudioStreamTrack audioTrack)
            {
                Debug.LogAssertion("OnTrack audio");
                receiveAudio.gameObject.SetActive(true);
                receiveAudio.SetTrack(audioTrack);
                receiveAudio.loop = true;
                receiveAudio.Play();
                receiveAudioStream = e.Streams.First();
                receiveAudioStream.OnRemoveTrack = ev =>
                {
                    receiveAudio.Stop();
                    receiveAudio.clip = null;
                    ev.Track.Dispose();
                };
            }
        }

        private void OnIceCandidate(RTCIceCandidate candidate, Photon.Realtime.Player toPlayer)
        {
            RTCIceCandidateInit candidateInit = new RTCIceCandidateInit
            {
                candidate = candidate.Candidate,
                sdpMid = candidate.SdpMid,
                sdpMLineIndex = candidate.SdpMLineIndex
            };

            photonView.RPC(nameof(OnIceCandidateReceive), toPlayer, JsonUtility.ToJson(candidateInit), PhotonNetwork.LocalPlayer);
        }

        [PunRPC]
        public void OnIceCandidateReceive(string json, Photon.Realtime.Player fromPlayer)
        {
            RTCIceCandidateInit candidateInit = JsonUtility.FromJson<RTCIceCandidateInit>(json);
            RTCIceCandidate candidate = new RTCIceCandidate(candidateInit);

            RTCPeerConnection pc;
            if (photonView.IsMine)
            {
                pc = peerConnections[fromPlayer.UserId];
            }
            else
            {
                pc = peerConnection;
            }
            pc.AddIceCandidate(candidate);
        }

        private void OnSetLocalSuccess(RTCPeerConnection pc)
        {
            if (printDebug) Debug.Log($"{GetInstanceID()} SetLocalDescription complete");
        }

        static void OnSetSessionDescriptionError(ref RTCError error)
        {
            if (printDebug) Debug.LogError($"Error Detail Type: {error.message}");
        }

        private void OnSetRemoteSuccess(RTCPeerConnection pc)
        {
            if (printDebug) Debug.Log($"{GetInstanceID()} SetRemoteDescription complete");
        }

        private static void OnCreateSessionDescriptionError(RTCError error)
        {
            if (printDebug) Debug.LogError($"Error Detail Type: {error.message}");
        }

        private void OnIceConnectionChange(RTCPeerConnection pc, RTCIceConnectionState state)
        {
            switch (state)
            {
                case RTCIceConnectionState.New:
                    if (printDebug) Debug.Log($"{GetInstanceID()} IceConnectionState: New");
                    break;
                case RTCIceConnectionState.Checking:
                    if (printDebug) Debug.Log($"{GetInstanceID()} IceConnectionState: Checking");
                    break;
                case RTCIceConnectionState.Closed:
                    if (printDebug) Debug.Log($"{GetInstanceID()} IceConnectionState: Closed");
                    break;
                case RTCIceConnectionState.Completed:
                    if (printDebug) Debug.Log($"{GetInstanceID()} IceConnectionState: Completed");
                    break;
                case RTCIceConnectionState.Connected:
                    if (printDebug) Debug.Log($"{GetInstanceID()} IceConnectionState: Connected");
                    break;
                case RTCIceConnectionState.Disconnected:
                    if (printDebug) Debug.Log($"{GetInstanceID()} IceConnectionState: Disconnected");
                    OnConnectError();
                    break;
                case RTCIceConnectionState.Failed:
                    if (printDebug) Debug.Log($"{GetInstanceID()} IceConnectionState: Failed");
                    OnConnectError();
                    break;
                case RTCIceConnectionState.Max:
                    if (printDebug) Debug.Log($"{GetInstanceID()} IceConnectionState: Max");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public void OnConnectError()
        {
            //sourceImage.texture = errorImage;
            //receiveImage.texture = errorImage;
        }

        public IEnumerator CaptureAudioStart()
        {
            if (Microphone.devices.Length == 0)
            {
                if (printDebug) Debug.LogFormat("Microphone device not found");
                yield break;
            }
            
            if (Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            }
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                if (printDebug) Debug.LogFormat("authorization for using the device is denied");
                yield break;
            }
            
            var deviceName = Microphone.devices[0];
            Microphone.GetDeviceCaps(deviceName, out int minFreq, out int maxFreq);
            var micClip = Microphone.Start(deviceName, true, 1, 48000);

            // set the latency to “0” samples before the audio starts to play.
            // 조건이 끝나지를 않아서 무한루프에 빠짐, 일단 1000프레임만 기다리도록 수정
            int cnt = 1000;
            //while (!(Microphone.GetPosition(deviceName) > 0) && cnt-->0) { }
            yield return new WaitWhile(() => !(Microphone.GetPosition(deviceName) > 0) && cnt-- > 0);

            foreach (var track in sourceStream.GetAudioTracks())
            {
                track.Stop();
                track.Dispose();
            }

            sourceAudio.clip = micClip;
            sourceAudio.loop = true;
            sourceAudio.Play();
            audioStreamTrack = new AudioStreamTrack(sourceAudio);
            sourceStream.AddTrack(audioStreamTrack);

            AddTracks();
            yield break;
        }

        public void CaptureAudioStop()
        {
            if (sourceAudio.clip != null)
            {
                Microphone.End(Microphone.devices[0]);
                sourceAudio.Stop();
                sourceAudio.clip = null;
                audioStreamTrack.Dispose();
            }
        }

        public IEnumerator CaptureVideoStart()
        {
            if (WebCamTexture.devices.Length == 0)
            {
                if (printDebug) Debug.LogFormat("WebCam device not found");
                yield break;
            }
            
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                if (printDebug) Debug.LogFormat("authorization for using the device is denied");
                yield break;
            }

            //WebCamDevice userCameraDevice = WebCamTexture.devices[0];
            //webCamTexture = new WebCamTexture(userCameraDevice.name, WebRTCSettings.StreamSize.x, WebRTCSettings.StreamSize.y, 30);
            //webCamTexture?.Stop();
            webCamTexture = new WebCamTexture(VideoSetting.SelectedDevice.name, VideoSetting.StreamSize.x, VideoSetting.StreamSize.y, 30);
            webCamTexture.Play();
            yield return new WaitUntil(() => webCamTexture.didUpdateThisFrame);

            videoStreamTrack = new VideoStreamTrack(webCamTexture);
            //segmentTrack = new VideoStreamTrack(webCamTexture);
            //sourceImage.texture = webCamTexture;
            sourceImage.mainTexture = webCamTexture;
            //sourceImage.MainTexture = webCamTexture;
            //receiveImage.texture = webCamTexture;

            //sourceStream.GetVideoTracks().First().Dispose();
            foreach (var track in sourceStream.GetVideoTracks())
            {
                track.Dispose();
            }
            sourceStream.AddTrack(videoStreamTrack);
            //sourceStream.AddTrack(segmentTrack);

            AddTracks();
            yield break;
        }

        public void CaptureVideoStop()
        {
            webCamTexture?.Stop();
            webCamTexture = null;
            videoStreamTrack.Dispose();
            /*if (sourceImage.texture != null)
            {
                sourceImage.texture = null;
            }*/
            /*if (receiveImage.texture != null)
            {
                receiveImage.texture = null;
            }*/
        }

        public void SetAudioActive(bool isActive)
        {
            photonView.RPC(nameof(SetAudioActiveRPC), RpcTarget.OthersBuffered, !isActive);
        }

        public void SetVideoActive(bool isActive)
        {
            photonView.RPC(nameof(SetVideoActiveRPC), RpcTarget.OthersBuffered, !isActive);
        }

        [PunRPC]
        public void SetAudioActiveRPC(bool isActive)
        {
            if (!isActive)
            {
                receiveAudio.mute = true;
            }
            else
            {
                receiveAudio.mute = false;
            }
        }

        [PunRPC]
        public void SetVideoActiveRPC(bool isActive)
        {
            if (isActive)
            {
                receiveImage.enabled = true;
            }
            else
            {
                receiveImage.enabled = false;
            }
        }

        public void SetBlur(float phase)
        {
            receiveImage.material.SetFloat("_AlphaMultiply", phase);
        }

        private void OnDestroy()
        {
            if (photonView.IsMine)
            {
                if (webCamTexture != null)
                {
                    CaptureVideoStop();
                }
                if (sourceAudio.clip != null)
                {
                    CaptureAudioStop();
                }
                WebRTC.Dispose();
            }
            onDestroy.Invoke(this);
        }
    }
}