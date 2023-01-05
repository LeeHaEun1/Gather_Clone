using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Photon.Pun;

namespace Gather.Test
{
    public class CRDT
    {
        // SortedList: Ű�� �������� ���ĵǰ� Ű�� �ε����� �׼����� �� �ִ� Ű/�� ���� �÷����� ��Ÿ���ϴ�.
        SortedList<CRDTID, char> charcters;
        public Action OnCRDTChanged; // �̰� ȣ���ϸ� �ش� Ŭ����?�Լ�? �ȿ� ��� ��� �ĵ� ȣ��Ǵ� ���
        PhotonView photonView;

        // CRDT ������
        public CRDT(PhotonView pv)
        {
            charcters = new SortedList<CRDTID, char>();
            photonView = pv;
        }

        // charcters to string
        // value��, �� char���� �����Ͽ� string���� ������ش�
        public string ListToString()
        {
            string str = "";
            foreach (KeyValuePair<CRDTID, char> entry in charcters)
            {
                str += entry.Value;
            }
            return str;
        }

        // �� ��Ȳ�� �°� ������ CRDTID(newID)�� �̿��� �ű� �� Insert
        // insert charcter from charcters with beforeId and afterId and timestamp
        public CRDTID GetInsertID(int index, char value)
        {
            CRDTID newID = null;

            // ���� ��ġ �յ� �ε����� ID�� ����, �� ���� ID�� �����Ѵ�.

            // (1) ���� ���̵� ���� ���
            // ���� �ƹ��� ���� ���� ��� �ʱⰪ�� �־��ش�.
            if (charcters.Count==0)
            {
                newID = new CRDTID();
            }

            // (2) �� �� ���̵� �ִ� ���
            // "isBefore" : �̿��� �տ� ������ ��� True �ڴ� False
            // �տ� �ִ� ID�� ���ٸ� ���� �ε������� �ϳ��� ���� �����Ѵ�.
            else if (index == 0) 
            {
                //newID = new CRDTID(charcters.Keys[index + 1], true);
                newID = new CRDTID(charcters.Keys[index], true);
            }
            // �ڿ� �ִ� ID�� ���ٸ� ���� �ε������� �ϳ��� ���ؼ� �����Ѵ�.
            //else if (index == charcters.Count) 
            else if (index == charcters.Count) 
            {
                newID = new CRDTID(charcters.Keys[index - 1], false);

                // **** IndexOutOfRange (0922_0911)
                //newID = new CRDTID(charcters.Keys[index], false);
            }

            // (3) ���� ���̵� ��� �ִ� ���
            // �յ� �ε����� ��� �ִٸ�, �� �ε��� ������ ID�� �����Ѵ�.
            else
            {
                // �ε��� �ȳ�ġ���� Ȯ�� �ʿ�
                // ���� ���ǵ鿡 ���� �ƿ� ����� ���� ��Ȳ�� ��������..?
                //if (index - 1 < 0 || index + 1 > charcters.Keys.Count)
                //{
                //    return;
                //}
                //newID = new CRDTID(charcters.Keys[index - 1], charcters.Keys[index + 1]);
                newID = new CRDTID(charcters.Keys[index - 1], charcters.Keys[index]);
            }

            // �� ��Ȳ�� �°� ������ CRDTID(newID)�� �̿��� �ű� �� Insert
            //charcters.Add(newID, value); // ******* RPC 

            // ***************** NullReferenceException (20:20)
            //photonView.RPC(nameof(RpcInsert), RpcTarget.All, JsonUtility.ToJson(newID), (int)value);

            return newID;
        }

        public void LocalInsert(string newID, int value)
        {
            CRDTID crdtId = JsonUtility.FromJson<CRDTID>(newID);

            Debug.Log($"Local Insert ({photonView.IsMine}) : " + (char)value);
            charcters.Add(crdtId, (char)value);
        }
        
        // *************** RPC Insert �Լ� ���� ***************
        public void RpcInsert(string newID, int value)
        {
            CRDTID crdtId = JsonUtility.FromJson<CRDTID>(newID);

            Debug.Log($"RPC Insert ({photonView.IsMine}) : " + (char)value);
            charcters.Add(crdtId, (char)value);
            // �Լ� ȣ��
            // ************�ٵ� �̰� �������� ȣ��Ǵ� �Լ� �ƴѰ�..? ���⼭ ���ִ°Ը³�????!?
            OnCRDTChanged();
        }

        public void LocalDelete(int index)
        {
            charcters.RemoveAt(index);
        }

        // delete charcter from charcters with index
        /*public void Delete(int index)
        {
            // �ε����� ID�� ����, �� ID�� ���� �����Ѵ�.
            //charcters.RemoveAt(index);
            photonView.RPC("RpcDelete", RpcTarget.All, index);
        }*/

        // *************** RPC Delete �Լ� ���� ***************
        public void RpcDelete(int index)
        {
            charcters.RemoveAt(index);
            // �Լ� ȣ��
            // ************�ٵ� �̰� �������� ȣ��Ǵ� �Լ� �ƴѰ�..? ���⼭ ���ִ°Ը³�????!?
            if (!photonView.IsMine) 
                OnCRDTChanged();
        }

        // test �뵵
        // �ʱ� ID ���� �Լ�
        public void Test()
        {
            CRDTID id = new CRDTID();
            CRDTID id2 = new CRDTID(id, false);
            CRDTID id3 = new CRDTID(id, id2);
        }

        //private void Update()
        //{
        //    photonView.RPC("RpcInsert", RpcTarget.All);
        //    photonView.RPC("RpcDelete", RpcTarget.All);
        //}

    }

    // CRDTID Ŭ������ IComparable�� ���̸� �⺻ ���� ������ �ǳ�? �ȵǸ� �̰� ����� ���
    /*public class CRDTIDComparer : IComparer<CRDTID>
    {
        public int Compare(CRDTID x, CRDTID y)
        {
            return x.CompareTo(y);
        }
    }*/



    // ������, IComparable �������̽� ��ӹ���
    public class CRDTID : IComparable
    {
        //string id;
        // ex. id ����: [0, -1, 5, 4, ,,,]
        public List<int> id;
        public ulong timestamp;    //id�� ������ Ÿ�ӽ������� ���� �Է��� ���� ������

        // [��Ȳ�� ���� CRDT ���� ����]
        /// <summary>
        /// ���� ���̵� ���� ��� ������ ID ����
        /// </summary>
        public CRDTID()
        {
            // ������ �Ǵ� 1���� ����Ʈ ����, 0���� �ʱ�ȭ
            id = new List<int>() { 0 };

            timestamp = GetTimestamp();
        }

        /// <summary>
        /// �� �� ���̵� ������ ����Ǵ� ���̵� ����
        /// </summary>
        /// <param name="neighborId"> �̿��� ���̵� </param>
        /// <param name="isBefore"> �̿��� �տ� ������ ��� True �ڴ� False </param>
        // �� ������ ������ ���� 1�� ����Ʈ�� ID�� ������ �ȴٰ� ����
        // �� �� ���̵� �ִ� = �� ���̳� �� �ڿ� �����ϴ� ��Ȳ
        public CRDTID(CRDTID neighborId, bool isBefore)
        {
            id = new List<int>();

            // neighborId �տ� �����Ҷ� 
            if (isBefore) 
            {
                id.Add(neighborId.id[0] - 1);
            }
            // neighborId �ڿ� �����Ҷ�
            else
            {
                id.Add(neighborId.id[id.Count] + 1);
            }

            timestamp = GetTimestamp();
        }

        /// <summary>
        /// �� �� ���̵� ������ ����Ǵ� ���̵� ����
        /// </summary>
        /// <param name="beforeId">���� ���̵�</param>
        /// <param name="afterId">���� ���̵�</param>
        public CRDTID(CRDTID beforeId, CRDTID afterId)
        {
            id = new List<int>();

            // ���� �ܰ���(= depth�� ���� ���)
            // (1) beforeId�� ���ҵ� �״�� ����ְ� (2) 0 �ϳ��� �߰�
            // ex. [-1, -1] & [-1, 0] ���̿� ���� -> [-1, -1, 0]
            // ex2. [1] & [2] ���̿� [1, 0] ����
            if (beforeId.id.Count == afterId.id.Count)
            {
                for(int index = 0; index < beforeId.id.Count; index++)
                {
                    id.Add(beforeId.id[index]);
                }
                id.Add(0);
                //if (beforeId.id[beforeId.id.Count - 1] == afterId.id[beforeId.id.Count - 1]) { }
                //else if (beforeId.id[beforeId.id.Count - 1] > afterId.id[beforeId.id.Count - 1]) { }
                //else { }
            }

            // ���� �� �ܰ谡 ���� ���(= depth�� ���� ���)
            // (1) beforeId�� ������ ���Ҹ� ������ ���ҵ� �״�� �߰��ϰ� (2) beforeId�� ������ ���ҿ� +1�� ���� �߰�
            // ex. [0, 0] & [1] ���̿� [0, 1] ����
            // ex. [-1, -1, 1] & [-1, 0] ���̿� [-1, -1, 2] ����
            else if (beforeId.id.Count > afterId.id.Count)
            {
                for (int index = 0; index < beforeId.id.Count - 1; index++)
                {
                    id.Add(beforeId.id[index]);
                }
                id.Add(beforeId.id[beforeId.id.Count - 1] + 1);
                //for (int index=0; index< afterId.id.Count; index++)
                //{
                //    if (beforeId.id[index] == afterId.id[index])
                //    {
                //        id.Add(beforeId.id[index]);
                //    }
                //    else if (beforeId.id[index] > afterId.id[index]) { }
                //    else { }
                //}
            }

            // �ڰ� �� �ܰ谡 ���� ���(= depth�� ���� ���)
            // (1) afterId�� ������ ���Ҹ� ������ ���ҵ� �״�� �߰��ϰ� (2) afterId�� ������ ���ҿ� -1�� ���� �߰�
            // ex. [-1, -1] & [-1, -1, 0] -> [-1, -1, -1]
            // ex. [1] & [1, 0] -> [1, -1]
            else
            {
                for (int index = 0; index < afterId.id.Count - 1; index++)
                {
                    id.Add(afterId.id[index]);
                }
                id.Add(afterId.id[afterId.id.Count - 1] - 1);
            }

            timestamp = GetTimestamp();
        }


        // �ֽ� �Է��ϼ��� �� ū �� ��ȯ
        ulong GetTimestamp()
        {
            TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            // 10�� ���� ���Ϸ� �ڸ��� �Ҽ����� ���ش�.
            return ((ulong)timeSpan.TotalMilliseconds % 100000);
        }


        // [id ��¿� �Լ�]
        // C# ��¿� ���� �Լ�
        public override string ToString()
        {
            string result = "";
            foreach (int i in id)
            {
                result += "i";
            }
            return result + ":" + timestamp.ToString();
        }


        // [CRDTID�� ���� ������ �Ǵ� �Լ�? �������̽� ����]
        // ���� ID�� �����ϰ�, ���� ID�� Ÿ�ӽ������� �����Ѵ�.
        // ������ ��ü�� ��� ��ü(= other)���� ������ 0���� ���� ����, ������ 0��, ũ�� 0���� ū ���� ��ȯ.
        // **** (�����) ������ = �ռ�, ũ�� = �ڿ���ġ
        int IComparable.CompareTo(object obj)
        {
            // 1. ����ȯ
            CRDTID other = (CRDTID)obj;

            // 2. other == null�� ���
            if (other == null)
            {
                throw new ApplicationException("CRDTID ��ü�� �ƴմϴ�.");
            }

            // 3. other != null�� ���
            // for�� �̿��� index=0������ ���ذ��� ���� ��ġ���� �� ���� ���� ������ �ִ� ����� �� �ռ��� ��
            // => �񱳴�� �������� ������ �����ϴٸ�?? (1) ���̰� �� �� ��(=depth�� �� ���� ��)�� �ִٸ� �װ��� �� �ڿ� ���� �� (2) �׷��� �ʰ� ���̰� ���ٸ� timestamp �� 
            // for�� ������ ���̴� �� �� depth�� �� ���� ���� ���̷� �ؾ���
            // ******** timestamp�� ����ؾ���!!!

            // ************************* CS0161 ������ �ӽ� �ּ�ó��: ���� �ʿ�!!!
            int forLength = Math.Min(id.Count, other.id.Count);

            for (int index = 0; index < forLength; index++)
            {
                // (1) ���� depth���� �񱳴�󺸴� ���� ���� ������ ���� �� �տ� ������ �Ǵ�
                if (id[index] < other.id[index])
                {
                    return -1;
                }
                // (2) ���� depth���� �񱳴�󺸴� ū ���� ������ ���� �� �ڿ� ������ �Ǵ�
                else if (id[index] > other.id[index])
                {
                    return 1;
                }
                // (3) �񱳱������� ��� ���Ұ� ���ٸ�
                else
                {
                    if (index == forLength - 1)
                    {
                        if (id.Count < other.id.Count)
                        {
                            return -1;
                        }
                        else if (id.Count > other.id.Count)
                        {
                            return 1;
                        }
                        // �� �ʰ� �Էµ� ��(=timestamp�� ū ��)�� �� �տ� ���� (�³�???????????)
                        else
                        {
                            if (timestamp > other.timestamp)
                            {
                                return -1;
                            }
                            else if (timestamp < other.timestamp)
                            {
                                return 1;
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }
                }
            }
            return 0;


            // ****************** �������������� copilot �ӽ� �ּ� ����******************

            // ****************** copilot�� §�� ��������� �׳� �ø��Ŷ� ���� ���ƾ��մϴ�
            // ���� depth������ �����̴� ���� ����!!
            //if (id.Count < other.id.Count) return -1;
            //else if (id.Count > other.id.Count) return 1;
            //else
            //{
            //    for (int index = 0; index < id.Count; index++)
            //    {
            //        if (id[index] < other.id[index])
            //            return -1;
            //        else if (id[index] > other.id[index])
            //            return 1;
            //    }
            //    return 0;
            //}
        }
    }



    // inputField�� ���� Ŭ����
    public class Test_CRDT : MonoBehaviourPun //, IPunObservable
    {
        TMPro.TMP_InputField inputField; // ��¥ ��Ʈ�� ��ǲ�ʵ�
        // ������ �ؽ�Ʈ�� ����
        string text;
        // ������ CRDT�� ����, ��ü
        // CRDT: SortedList<CRDTID, char>
        CRDT crdt;


        // [�Ҵ�]
        // Start is called before the first frame update
        void Start()
        {
            // �� �ڽſ� ���� ������Ʈ TMP_InputField �������� => �׷� �� Class�� InputField�� �ٿ��ߵȴٴ� ���̰���..?
            inputField = GetComponent<TMP_InputField>();
            // TMP_InputField�� text(���� �ٲ�� ����)           
            text = inputField.text;

            // CRDT �ʱ�ȭ �� �̺�Ʈ ���
            crdt = new CRDT(photonView);
            crdt.OnCRDTChanged = OnCRDTChanged; // ********

            // �׽�Ʈ�� ���� ����
            //crdt.Test();
            //StartCoroutine(test());

            // Canvas�� �ڽ����� �Ҵ�
            this.transform.parent = GameObject.Find("CanvasNote").transform;
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                crdt = new CRDT(photonView);
                text = inputField.text;
            }
        }

        // test �뵵
        int index = 0;
        IEnumerator test()
        {
            while(true)
            {
                yield return new WaitForSeconds(1);
                inputField.text += index.ToString();
                index++;
            }    
        }
        bool isTest = false;
        // CRDT�� ���ݿ��� ����Ǿ����� ȣ��: ���� -> ���� notice
        public void OnCRDTChanged()
        {
            inputField.text = crdt.ListToString(); // value���� �̾� string��ȯ�ϴ� �Լ�

            Debug.LogAssertion("OnCRDTChanged: " + inputField.text);
            text = inputField.text; // ��� ������ ���� �ؽ�Ʈ �����ϴ� ������ ����
            isTest = true;
        }

        Queue<int> changeQueue = new Queue<int>();

        // ���� -> ���� notice
        public void OnValueChanged(string value)
        {
            int timestamp = (int)((ulong)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds % 100000);
            print(timestamp);
                        
            Debug.Log($"OnValueChanged: {value} {isTest}");
            if (isTest)
            {
                isTest = false;
                return;
            }
            //if(text == "")
            //{
            //    return;
            //}
            bool isDeleted = false;

            for (int i = 0; i < value.Length; i++)
            {
                // ���� ���ڿ� �̻��� �ε����� ����
                // ����� ���ڿ�(text)�� ���̰� i ���϶��
                if (text.Length <= i)
                {
                    Debug.Log($"Insert: {value[i]}");

                    // ***************** OutOfRangeException
                    //crdt.Insert(i, value[i]);
                    Insert(i, value[i], timestamp);
                    break;
                }
                // ����� ���ڿ�(text)�� ���̰� i �ʰ��̰�, text[i]�� value[i]�� ���� �ٸ��ٸ�
                else if (text[i] != value[i])
                {
                    // ���� ������ ���� ���ڿ� ���ٸ�, �ϳ��� ���ڰ� ������ ��
                    if (text.Length > value.Length && text[i + 1] == value[i])
                    {
                        Debug.Log($"Delete: {text[i]}");

                        //crdt.Delete(i);
                        Delete(i, timestamp);

                        isDeleted = true;
                        break;
                    }
                    // ���� ������ ���� ���ڿ� �ٸ��ٸ�, �ϳ��� ���ڰ� ���Ե� ��
                    else
                    {
                        Debug.Log($"Insert: {value[i]}");
                        
                        //crdt.Insert(i, value[i]);
                        Insert(i, value[i], timestamp);
                        break;
                    }
                }
            }

            if(!isDeleted && text.Length > value.Length)
            {
                //crdt.Delete(text.Length - 1);
                Delete(text.Length - 1, timestamp);
            }

            // ���� ���ڿ� �̻��� �ε����� �����Ȱ�
            //for (int i = value.Length; i < text.Length; i++)
            //{
            //    if (text[i] != value[value.Length-1])
            //    {
            //        Debug.Log($"Delete: {text[i]}");

            //        crdt.Delete(i);
            //        break;
            //    }
            //}

            print("ccccccccccccccccccc"+crdt.ListToString());
            // ���� ���ڿ��� ����
            text = value;


            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~
        }

        public void Insert(int i, char c, int timestamp)
        {
            //photonView.RPC(nameof(RpcInsert), RpcTarget.AllBuffered, JsonUtility.ToJson(crdt.GetInsertID(i, c)), (int)c, timestamp);
            photonView.RPC(nameof(RpcInsert), RpcTarget.OthersBuffered, JsonUtility.ToJson(crdt.GetInsertID(i, c)), (int)c, timestamp);
        }

        public void Delete(int index, int timestamp)
        {
            // �ε����� ID�� ����, �� ID�� ���� �����Ѵ�.
            //charcters.RemoveAt(index);
            photonView.RPC(nameof(RpcDelete), RpcTarget.OthersBuffered, index, timestamp);
        }

        [PunRPC]
        public void RpcInsert(string newID, int value, int timestamp)
        {
            changeQueue.Enqueue(timestamp);
            crdt.RpcInsert(newID, value);
        }

        [PunRPC]
        public void RpcDelete(int index, int timestamp)
        {
            changeQueue.Enqueue(timestamp);
            crdt.RpcDelete(index);
        }


        //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        //{
        //    throw new NotImplementedException();
        //}                                            
    }
}