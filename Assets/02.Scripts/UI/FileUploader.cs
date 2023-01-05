using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;
using UnityEngine.Networking; // UWR용
//using System.IO;
using SFB;

public class FileUploader : MonoBehaviour
{
    string[] paths;
    public RawImage rawImage;
    
    public void OpenExplorer()
    {
        //path = EditorUtility.OpenFilePanel("Overwrite with png", "", "png"); // Build Error 발생
        paths = StandaloneFileBrowser.OpenFilePanel("Overwrite with png", "", "png", false); // Build는 진행되는거 같은데 시간 너무 오래 걸려 테스트 못해봄..
        GetImage();
    }

    public void GetImage()
    {
        if(paths != null)
        {
            StartCoroutine(UploadImage());
        }
    }

    public IEnumerator UploadImage()
    {
        //Texture2D texture = Selection.activeObject as Texture2D;
        //WWW www = new WWW("file:///" + path);
        //rawImage.texture = www.texture;
        //var fileContent = File.ReadAllBytes(path);
        //texture.LoadImage(fileContent);


        //UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("file:///" + path);
        //UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path);
        //yield return uwr.SendWebRequest();
       
        //rawImage.texture = DownloadHandlerTexture.GetContent(uwr);



        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("file:///" + paths[0]))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                var uwrTexture = DownloadHandlerTexture.GetContent(uwr);
                rawImage.texture = uwrTexture;
                rawImage.color = new Color(1, 1, 1, 1);
            }
        }
    }
}
