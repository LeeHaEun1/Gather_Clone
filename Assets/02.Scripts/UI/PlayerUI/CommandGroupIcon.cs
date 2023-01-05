using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Gather.UI.PlayerUI
{        
    public class CommandGroupIcon : MonoBehaviour
    {
        Image bg;
        Image icon;
        Button button;

        public UnityEvent onClick
        {
            get
            {
                return button.onClick;
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            Image[] images = GetComponentsInChildren<Image>(true);
            bg = images[0];
            icon = images[1];
            button = GetComponent<Button>();
        }

        public void SetIcon(Sprite sprite)
        {
            icon.sprite = sprite;
        }

        public void SetEnable(bool enable)
        {
            bg.enabled = enable;
            icon.enabled = enable;
            button.enabled = enable;
        }
    }
}