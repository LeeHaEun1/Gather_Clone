using Gather.Character;
using Gather.Network;
using Mediapipe.SelfieSegmentation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewImage : MonoBehaviour
{
    MeshRenderer meshRenderer;
    SelfieSegmentation segmentation;
    [SerializeField] SelfieSegmentationResource segmentResource;
    public Texture mainTexture;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        segmentation = new SelfieSegmentation(segmentResource);
    }

    // Update is called once per frame
    void Update()
    {
        if (mainTexture != null)
        {
            meshRenderer.material.SetTexture("_BaseMap", mainTexture);
            segmentation.ProcessImage(mainTexture);
            meshRenderer.material.SetTexture("_SegmentMask", segmentation.texture);
        }
    }

    public void OnChangePlayer(Player player)
    {
        transform.SetParent(player.transform.Find("Player UI").Find("Preview Frame"));
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        //transform.position = player.transform.Find("Preview Image Position").position;
        //transform.rotation = player.transform.Find("Preview Image Position").rotation;
    }
}
