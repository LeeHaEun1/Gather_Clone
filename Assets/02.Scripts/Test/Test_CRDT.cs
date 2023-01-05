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
        // SortedList: 키를 기준으로 정렬되고 키와 인덱스로 액세스할 수 있는 키/값 쌍의 컬렉션을 나타냅니다.
        SortedList<CRDTID, char> charcters;
        public Action OnCRDTChanged; // 이거 호출하면 해당 클래스?함수? 안에 담긴 모든 식들 호출되는 기능
        PhotonView photonView;

        // CRDT 생성자
        public CRDT(PhotonView pv)
        {
            charcters = new SortedList<CRDTID, char>();
            photonView = pv;
        }

        // charcters to string
        // value값, 즉 char들을 연결하여 string으로 만들어준다
        public string ListToString()
        {
            string str = "";
            foreach (KeyValuePair<CRDTID, char> entry in charcters)
            {
                str += entry.Value;
            }
            return str;
        }

        // 각 상황에 맞게 생성한 CRDTID(newID)를 이용해 신규 값 Insert
        // insert charcter from charcters with beforeId and afterId and timestamp
        public CRDTID GetInsertID(int index, char value)
        {
            CRDTID newID = null;

            // 삽입 위치 앞뒤 인덱스의 ID를 구해, 그 사이 ID를 생성한다.

            // (1) 기존 아이디 없는 경우
            // 아직 아무런 값이 없을 경우 초기값을 넣어준다.
            if (charcters.Count==0)
            {
                newID = new CRDTID();
            }

            // (2) 한 쪽 아이디만 있는 경우
            // "isBefore" : 이웃의 앞에 삽입할 경우 True 뒤는 False
            // 앞에 있는 ID가 없다면 뒤의 인덱스에서 하나를 빼서 생성한다.
            else if (index == 0) 
            {
                //newID = new CRDTID(charcters.Keys[index + 1], true);
                newID = new CRDTID(charcters.Keys[index], true);
            }
            // 뒤에 있는 ID가 없다면 앞의 인덱스에서 하나를 더해서 생성한다.
            //else if (index == charcters.Count) 
            else if (index == charcters.Count) 
            {
                newID = new CRDTID(charcters.Keys[index - 1], false);

                // **** IndexOutOfRange (0922_0911)
                //newID = new CRDTID(charcters.Keys[index], false);
            }

            // (3) 양쪽 아이디가 모두 있는 경우
            // 앞뒤 인덱스가 모두 있다면, 두 인덱스 사이의 ID를 생성한다.
            else
            {
                // 인덱스 안넘치는지 확인 필요
                // 앞의 조건들에 의해 아예 여기로 들어올 상황이 없을수도..?
                //if (index - 1 < 0 || index + 1 > charcters.Keys.Count)
                //{
                //    return;
                //}
                //newID = new CRDTID(charcters.Keys[index - 1], charcters.Keys[index + 1]);
                newID = new CRDTID(charcters.Keys[index - 1], charcters.Keys[index]);
            }

            // 각 상황에 맞게 생성한 CRDTID(newID)를 이용해 신규 값 Insert
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
        
        // *************** RPC Insert 함수 구현 ***************
        public void RpcInsert(string newID, int value)
        {
            CRDTID crdtId = JsonUtility.FromJson<CRDTID>(newID);

            Debug.Log($"RPC Insert ({photonView.IsMine}) : " + (char)value);
            charcters.Add(crdtId, (char)value);
            // 함수 호출
            // ************근데 이건 원격으로 호출되는 함수 아닌가..? 여기서 해주는게맞나????!?
            OnCRDTChanged();
        }

        public void LocalDelete(int index)
        {
            charcters.RemoveAt(index);
        }

        // delete charcter from charcters with index
        /*public void Delete(int index)
        {
            // 인덱스의 ID를 구해, 그 ID로 값을 삭제한다.
            //charcters.RemoveAt(index);
            photonView.RPC("RpcDelete", RpcTarget.All, index);
        }*/

        // *************** RPC Delete 함수 구현 ***************
        public void RpcDelete(int index)
        {
            charcters.RemoveAt(index);
            // 함수 호출
            // ************근데 이건 원격으로 호출되는 함수 아닌가..? 여기서 해주는게맞나????!?
            if (!photonView.IsMine) 
                OnCRDTChanged();
        }

        // test 용도
        // 초기 ID 생성 함수
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

    // CRDTID 클래스에 IComparable를 붙이면 기본 정렬 기준이 되나? 안되면 이거 살려서 사용
    /*public class CRDTIDComparer : IComparer<CRDTID>
    {
        public int Compare(CRDTID x, CRDTID y)
        {
            return x.CompareTo(y);
        }
    }*/



    // 생성자, IComparable 인터페이스 상속받음
    public class CRDTID : IComparable
    {
        //string id;
        // ex. id 형식: [0, -1, 5, 4, ,,,]
        public List<int> id;
        public ulong timestamp;    //id가 같을때 타임스탬프로 먼저 입력한 것을 앞으로

        // [상황에 따른 CRDT 생성 로직]
        /// <summary>
        /// 기존 아이디가 없을 경우 최초의 ID 생성
        /// </summary>
        public CRDTID()
        {
            // 기준이 되는 1차원 리스트 생성, 0으로 초기화
            id = new List<int>() { 0 };

            timestamp = GetTimestamp();
        }

        /// <summary>
        /// 한 쪽 아이디만 있을때 연결되는 아이디 생성
        /// </summary>
        /// <param name="neighborId"> 이웃의 아이디 </param>
        /// <param name="isBefore"> 이웃의 앞에 삽입할 경우 True 뒤는 False </param>
        // 양 끝단은 무조건 길이 1의 리스트를 ID로 가지게 된다고 가정
        // 한 쪽 아이디만 있다 = 맨 앞이나 맨 뒤에 삽입하는 상황
        public CRDTID(CRDTID neighborId, bool isBefore)
        {
            id = new List<int>();

            // neighborId 앞에 생성할때 
            if (isBefore) 
            {
                id.Add(neighborId.id[0] - 1);
            }
            // neighborId 뒤에 생성할때
            else
            {
                id.Add(neighborId.id[id.Count] + 1);
            }

            timestamp = GetTimestamp();
        }

        /// <summary>
        /// 양 쪽 아이디가 있을때 연결되는 아이디 생성
        /// </summary>
        /// <param name="beforeId">앞의 아이디</param>
        /// <param name="afterId">뒤의 아이디</param>
        public CRDTID(CRDTID beforeId, CRDTID afterId)
        {
            id = new List<int>();

            // 같은 단계라면(= depth가 같은 경우)
            // (1) beforeId의 원소들 그대로 집어넣고 (2) 0 하나만 추가
            // ex. [-1, -1] & [-1, 0] 사이에 삽입 -> [-1, -1, 0]
            // ex2. [1] & [2] 사이에 [1, 0] 삽입
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

            // 앞이 더 단계가 낮을 경우(= depth가 깊은 경우)
            // (1) beforeId의 마지막 원소를 제외한 원소들 그대로 추가하고 (2) beforeId의 마지막 원소에 +1한 원소 추가
            // ex. [0, 0] & [1] 사이에 [0, 1] 생성
            // ex. [-1, -1, 1] & [-1, 0] 사이에 [-1, -1, 2] 생성
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

            // 뒤가 더 단계가 낮을 경우(= depth가 깊은 경우)
            // (1) afterId의 마지막 원소를 제외한 원소들 그대로 추가하고 (2) afterId의 마지막 원소에 -1한 원소 추가
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


        // 최신 입력일수록 더 큰 값 반환
        ulong GetTimestamp()
        {
            TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            // 10초 단위 이하로 자르고 소수점도 없앤다.
            return ((ulong)timeSpan.TotalMilliseconds % 100000);
        }


        // [id 출력용 함수]
        // C# 출력에 쓰는 함수
        public override string ToString()
        {
            string result = "";
            foreach (int i in id)
            {
                result += "i";
            }
            return result + ":" + timestamp.ToString();
        }


        // [CRDTID의 정렬 기준이 되는 함수? 인터페이스 정의]
        // 먼저 ID로 정렬하고, 같은 ID는 타임스탬프로 정렬한다.
        // 현재의 객체가 대상 객체(= other)보다 작으면 0보다 작은 값을, 같으면 0을, 크면 0보다 큰 값을 반환.
        // **** (검토要) 작으면 = 앞선, 크면 = 뒤에위치
        int IComparable.CompareTo(object obj)
        {
            // 1. 형변환
            CRDTID other = (CRDTID)obj;

            // 2. other == null인 경우
            if (other == null)
            {
                throw new ApplicationException("CRDTID 개체가 아닙니다.");
            }

            // 3. other != null인 경우
            // for문 이용해 index=0번부터 비교해가며 같은 위치에서 더 작은 값을 가지고 있는 대상이 더 앞서는 것
            // => 비교대상 구간에서 끝까지 동일하다면?? (1) 길이가 더 긴 것(=depth가 더 깊은 것)이 있다면 그것이 더 뒤에 오는 것 (2) 그렇지 않고 길이가 같다면 timestamp 비교 
            // for문 돌리는 길이는 둘 중 depth가 더 얕은 것의 길이로 해야함
            // ******** timestamp도 고려해야함!!!

            // ************************* CS0161 오류로 임시 주석처리: 수정 필요!!!
            int forLength = Math.Min(id.Count, other.id.Count);

            for (int index = 0; index < forLength; index++)
            {
                // (1) 같은 depth에서 비교대상보다 작은 숫자 나오는 순간 더 앞에 존재함 판단
                if (id[index] < other.id[index])
                {
                    return -1;
                }
                // (2) 같은 depth에서 비교대상보다 큰 숫자 나오는 순간 더 뒤에 존재함 판단
                else if (id[index] > other.id[index])
                {
                    return 1;
                }
                // (3) 비교구간에서 모든 원소가 같다면
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
                        // 더 늦게 입력된 것(=timestamp가 큰 것)을 더 앞에 삽입 (맞나???????????)
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


            // ****************** 에러방지용으로 copilot 임시 주석 해제******************

            // ****************** copilot이 짠거 참고용으로 그냥 올린거라 완전 갈아야합니다
            // 현재 depth만으로 비교중이니 옳지 않음!!
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



    // inputField에 붙일 클래스
    public class Test_CRDT : MonoBehaviourPun //, IPunObservable
    {
        TMPro.TMP_InputField inputField; // 진짜 노트의 인풋필드
        // 기존의 텍스트를 저장
        string text;
        // 기존의 CRDT를 저장, 본체
        // CRDT: SortedList<CRDTID, char>
        CRDT crdt;


        // [할당]
        // Start is called before the first frame update
        void Start()
        {
            // 나 자신에 붙은 컴포넌트 TMP_InputField 가져오기 => 그럼 이 Class를 InputField에 붙여야된다는 뜻이겠지..?
            inputField = GetComponent<TMP_InputField>();
            // TMP_InputField의 text(실제 바뀌는 내용)           
            text = inputField.text;

            // CRDT 초기화 및 이벤트 등록
            crdt = new CRDT(photonView);
            crdt.OnCRDTChanged = OnCRDTChanged; // ********

            // 테스트용 추후 삭제
            //crdt.Test();
            //StartCoroutine(test());

            // Canvas의 자식으로 할당
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

        // test 용도
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
        // CRDT가 원격에서 변경되었을때 호출: 원격 -> 로컬 notice
        public void OnCRDTChanged()
        {
            inputField.text = crdt.ListToString(); // value들을 이어 string반환하는 함수

            Debug.LogAssertion("OnCRDTChanged: " + inputField.text);
            text = inputField.text; // 상기 내용을 기존 텍스트 저장하는 변수에 담음
            isTest = true;
        }

        Queue<int> changeQueue = new Queue<int>();

        // 로컬 -> 원격 notice
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
                // 기존 문자열 이상의 인덱스는 삽입
                // 저장된 문자열(text)의 길이가 i 이하라면
                if (text.Length <= i)
                {
                    Debug.Log($"Insert: {value[i]}");

                    // ***************** OutOfRangeException
                    //crdt.Insert(i, value[i]);
                    Insert(i, value[i], timestamp);
                    break;
                }
                // 저장된 문자열(text)의 길이가 i 초과이고, text[i]와 value[i]의 값이 다르다면
                else if (text[i] != value[i])
                {
                    // 기존 문자의 다음 문자와 같다면, 하나의 문자가 삭제된 것
                    if (text.Length > value.Length && text[i + 1] == value[i])
                    {
                        Debug.Log($"Delete: {text[i]}");

                        //crdt.Delete(i);
                        Delete(i, timestamp);

                        isDeleted = true;
                        break;
                    }
                    // 기존 문자의 다음 문자와 다르다면, 하나의 문자가 삽입된 것
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

            // 기존 문자열 이상의 인덱스는 삭제된것
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
            // 기존 문자열을 갱신
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
            // 인덱스의 ID를 구해, 그 ID로 값을 삭제한다.
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