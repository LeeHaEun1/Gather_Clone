using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using UnityEngine.UI;
using Gather.Data;

public class Test_Opencv : MonoBehaviour
{
	public Texture2D texture;
    private WebCamTexture webCamTexture;
    public RawImage sourceImage;
    Mat receiveMat;
    Mat sendMat;
    BackgroundSubtractorGMG bgGMG;

    // Use this for initialization
    void Start()
	{
        sourceImage = GetComponent<RawImage>();

        sendMat = new Mat();

        /*Mat mat = OpenCvSharp.Unity.TextureToMat(this.texture);
        Mat grayMat = new Mat();
        Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);
        BackgroundSubtractorGMG bgGMG = BackgroundSubtractorGMG.Create();
        bgGMG.Apply(mat, grayMat);

        Texture2D texture = OpenCvSharp.Unity.MatToTexture(grayMat);

        RawImage rawImage = gameObject.GetComponent<RawImage>();
        rawImage.texture = texture;*/
        //		Renderer renderer = gameObject.GetComponent<Renderer> ();
        //		renderer.material.mainTexture = texture;

        StartCoroutine(CaptureVideoStart());
    }

    // Update is called once per frame
    void Update()
    {
        print(sendMat);
        bgGMG.Apply(receiveMat, sendMat);
        sourceImage.texture = OpenCvSharp.Unity.MatToTexture(sendMat);
    }

    public void bgGMGApply()
    {
        /*Mat mat = OpenCvSharp.Unity.TextureToMat(this.texture);
        Mat grayMat = new Mat();
        Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);*/

        //Mat mat = OpenCvSharp.Unity.TextureToMat(sourceImage.mainTexture);

        print(receiveMat);
        print(sendMat);
        bgGMG = BackgroundSubtractorGMG.Create();
        bgGMG.Apply(receiveMat, sendMat);
        print(receiveMat);
        print(sendMat);

        Texture2D texture = OpenCvSharp.Unity.MatToTexture(sendMat);
        print("texture : " + texture);

        sourceImage.texture = texture;
    }

    private IEnumerator CaptureVideoStart()
    {
        print(0000000000);
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

        //WebCamDevice userCameraDevice = WebCamTexture.devices[0];
        //webCamTexture = new WebCamTexture(userCameraDevice.name, WebRTCSettings.StreamSize.x, WebRTCSettings.StreamSize.y, 30);
        //webCamTexture?.Stop();
        //webCamTexture = new WebCamTexture( , VideoSetting.StreamSize.x, VideoSetting.StreamSize.y, 30);
        webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name, VideoSetting.StreamSize.x, VideoSetting.StreamSize.y, 30);
        webCamTexture.Play();
        print(1111111);
        yield return new WaitUntil(() => webCamTexture.didUpdateThisFrame);

        print(2222222);
        sourceImage.texture = webCamTexture;
        //receiveImage.texture = webCamTexture;
        receiveMat = OpenCvSharp.Unity.TextureToMat(webCamTexture);

        bgGMGApply();

        yield break;
    }

    void CaptureVideoStop()
    {
        webCamTexture?.Stop();
        webCamTexture = null;
        if (sourceImage.texture != null)
        {
            sourceImage.texture = null;
        }
        /*if (receiveImage.texture != null)
        {
            receiveImage.texture = null;
        }*/
    }
}
