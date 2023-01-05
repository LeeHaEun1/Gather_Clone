using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewFrame : MonoBehaviour
{
    public void SetActive(bool active)
    {
        gameObject.SetActive(!active);
    }
}
