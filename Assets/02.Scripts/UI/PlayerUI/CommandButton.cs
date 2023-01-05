using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gather.UI.PlayerUI
{
    public class CommandButton : MonoBehaviour
    {
        Image bg;
        Text title;
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
            bg = GetComponent<Image>();
            title = GetComponentInChildren<Text>();
            button = GetComponent<Button>();
        }

        public void SetText(string text)
        {
            this.title.text = text;
        }

        public void SetEnable(bool enable)
        {
            bg.enabled = enable;
            title.enabled = enable;
            button.enabled = enable;
        }
    }
}