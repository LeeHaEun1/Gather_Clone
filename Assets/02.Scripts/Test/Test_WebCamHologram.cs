using System.Collections;
using System.Collections.Generic;
using Unity.WebRTC;
using UnityEngine;
using UnityEngine.UI;

public class Test_WebCamHologram : MonoBehaviour
{
    public RawImage previewImage;
    public RenderTexture renderTexture;
    public MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CaptureVideoStart());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator CaptureVideoStart()
    {
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogFormat("WebCam device not found");
            yield break;
        }

        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.LogFormat("authorization for using the device is denied");
            yield break;
        }

        WebCamTexture webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name, 1280, 720, 30);
        webCamTexture.Play();
        yield return new WaitUntil(() => webCamTexture.didUpdateThisFrame);

        //VideoStreamTrack videoStreamTrack = new VideoStreamTrack(webCamTexture);
        previewImage.texture = webCamTexture;
        previewImage.material.mainTexture = webCamTexture;
        //renderTexture = new RenderTexture(webCamTexture.width, webCamTexture.height, 24);
        //previewImage.material.SetTexture("_MainTex", renderTexture);
        previewImage.material.SetTexture("_BaseMap", webCamTexture);

        meshRenderer.material.SetTexture("_BaseMap", webCamTexture);
        
        yield break;
    }
}
