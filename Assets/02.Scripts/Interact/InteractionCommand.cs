using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Photon.Pun; // 0925 LHE


namespace Gather.Interact
{
    [System.Serializable]
    public abstract class InteractCommandBase : IComparable 
    {
        public int id;
        public byte authority;
        public int priority;
        public bool isActivate;
        public string password;
        public bool isPassword;
        public string title;

        public abstract void Execute();
        public abstract bool IsExecutable();

        public abstract bool CheckAuthority(byte authority);

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (obj is InteractCommandBase)
            {
                int res = priority.CompareTo(((InteractCommandBase)obj).priority);
                if (res == 0)
                    return id.CompareTo(((InteractCommandBase)obj).id);
                else
                    return res;
            }
            else
                return 1;
        }
    }
        
    [System.Serializable]
    public class InteractCommand<T>: InteractCommandBase
    {
        public T target;
        
        public InteractCommand()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
        }

        public override void Execute() { }

        public override bool IsExecutable()
        {
            //CheckAuthority()
            return isActivate;
        }

        public override bool CheckAuthority(byte authority)
        {
            // 추후 편집, 열람 분리
            return (this.authority & authority) == this.authority;
        }
    }
}