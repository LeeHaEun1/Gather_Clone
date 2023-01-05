using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using UnityEngine.UI;
using Gather.Data;

/*public class Test_Opencv1 : MonoBehaviour
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

        *//*Mat mat = OpenCvSharp.Unity.TextureToMat(this.texture);
        Mat grayMat = new Mat();
        Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);
        BackgroundSubtractorGMG bgGMG = BackgroundSubtractorGMG.Create();
        bgGMG.Apply(mat, grayMat);

        Texture2D texture = OpenCvSharp.Unity.MatToTexture(grayMat);

        RawImage rawImage = gameObject.GetComponent<RawImage>();
        rawImage.texture = texture;*//*
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
        *//*Mat mat = OpenCvSharp.Unity.TextureToMat(this.texture);
        Mat grayMat = new Mat();
        Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);*//*

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
        *//*if (receiveImage.texture != null)
        {
            receiveImage.texture = null;
        }*//*
    }
}*/

public class Test_Opencv1 : MonoBehaviour
{
    #region public members
    public Texture2D m_texture;

    public RawImage m_image_origin;
    public RawImage m_image_gray;
    public RawImage m_Image_binarization;
    public RawImage m_image_mask;
    public RawImage m_image_backgroundTransparent;

    public double v_thresh = 180;
    public double v_maxval = 255;
    #endregion

    private void Start()
    {
        #region load texture
        Mat origin = OpenCvSharp.Unity.TextureToMat(this.m_texture);
        m_image_origin.texture = OpenCvSharp.Unity.MatToTexture(origin);
        #endregion

        #region  Gray scale image
        Mat grayMat = new Mat();
        Cv2.CvtColor(origin, grayMat, ColorConversionCodes.BGR2GRAY);
        m_image_gray.texture = OpenCvSharp.Unity.MatToTexture(grayMat);
        #endregion

        #region Find Edge
        Mat thresh = new Mat();
        Cv2.Threshold(grayMat, thresh, v_thresh, v_maxval, ThresholdTypes.BinaryInv);
        m_Image_binarization.texture = OpenCvSharp.Unity.MatToTexture(thresh);
        #endregion

        #region Create Mask
        Mat Mask = OpenCvSharp.Unity.TextureToMat(OpenCvSharp.Unity.MatToTexture(grayMat));
        Point[][] contours; HierarchyIndex[] hierarchy;
        Cv2.FindContours(thresh, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, null);
        for (int i = 0; i < contours.Length; i++)
        {
            Cv2.DrawContours(Mask, new Point[][] { contours[i] }, 0, new Scalar(0, 0, 0), -1);
        }
        Mask = Mask.CvtColor(ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(Mask, Mask, v_thresh, v_maxval, ThresholdTypes.Binary);
        m_image_mask.texture = OpenCvSharp.Unity.MatToTexture(Mask);
        #endregion

        #region TransparentBackground
        Mat transparent = origin.CvtColor(ColorConversionCodes.BGR2BGRA);
        unsafe
        {
            byte* b_transparent = transparent.DataPointer;
            byte* b_mask = Mask.DataPointer;
            float pixelCount = transparent.Height * transparent.Width;

            for (int i = 0; i < pixelCount; i++)
            {
                if (b_mask[0] == 255)
                {
                    b_transparent[0] = 0;
                    b_transparent[1] = 0;
                    b_transparent[2] = 0;
                    b_transparent[3] = 0;
                }
                b_transparent = b_transparent + 4;
                b_mask = b_mask + 1;
            }
        }
        m_image_backgroundTransparent.texture = OpenCvSharp.Unity.MatToTexture(transparent);
        #endregion
    }

}
