using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Gather.Character;

namespace Gather.Map
{
    public class PrivateArea : MonoBehaviour
    {
        public int areaID;
        private static Dictionary<int, List<PlayerConnection>> areaPlayers = new Dictionary<int, List<PlayerConnection>>();
        public static Dictionary<int, UnityEvent> onAreaPlayerChanges = new Dictionary<int, UnityEvent>();

        public Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public static PlayerConnection[] GetPlayers(int areaID)
        {
            if (areaPlayers.ContainsKey(areaID))
            {
                return areaPlayers[areaID].ToArray();
            }
            else
            {
                return null;
            }
        }

        public static void PrintPlayers(int areaID)
        {
            if (areaPlayers.ContainsKey(areaID))
            {
                string str = "players in area " + areaID + " : ";
                foreach (PlayerConnection player in areaPlayers[areaID])
                {
                    str += player.name + $"({player.GetInstanceID()})" + ", ";
                }
                Debug.Log(str);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerConnection player = other.GetComponent<PlayerConnection>();
                if (player != null)
                {
                    if (!areaPlayers.ContainsKey(areaID))
                    {
                        areaPlayers.Add(areaID, new List<PlayerConnection>());
                        areaPlayers[areaID].Add(player);
                        onAreaPlayerChanges.Add(areaID, new UnityEvent());
                    }
                    else
                    {
                        areaPlayers[areaID].Add(player);
                    }
                    animator.SetTrigger("Enter");
                    player.EnterPrivateArea(areaID);
                    onAreaPlayerChanges[areaID].Invoke();
                    print($"EnterPrivateArea: {areaID} - {player.name}");
                    PrintPlayers(areaID);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerConnection player = other.GetComponent<PlayerConnection>();
                if (player != null)
                {
                    if (areaPlayers.ContainsKey(areaID))
                        areaPlayers[areaID].Remove(player);
                    onAreaPlayerChanges[areaID].Invoke();
                    animator.SetTrigger("Exit");
                    player.ExitPrivateArea();
                }
                print($"ExitPrivateArea: {areaID} - {player.name}");
                PrintPlayers(areaID);
            }
        }
    }
}