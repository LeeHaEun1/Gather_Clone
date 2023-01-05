using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gather.Object
{
    public interface Interactable
    {
        void Interact();
        bool TryInteract();
        bool isInteractable();
        string GetName();
        Texture2D GetIcon();
    }
}
