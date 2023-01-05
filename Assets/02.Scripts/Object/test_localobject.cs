using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gather.Object
{
    public class test_localobject : LocalObject
    {
        MeshRenderer mr;

        public override void Interact()
        {
            if(mr.enabled == false)
            {
                mr.enabled = true;
            }
            else if (mr.enabled == true)
            {
                mr.enabled = false;
            }
        } 

        public override Texture2D GetIcon()
        {
            throw new System.NotImplementedException();
        }

        // Start is called before the first frame update
        void Start()
        {
            mr = GetComponent<MeshRenderer>();
            mr.enabled = false;

            canInteract = true;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
