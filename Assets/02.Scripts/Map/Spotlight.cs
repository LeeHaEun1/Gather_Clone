using Gather.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spotlight : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerConnection player = other.GetComponent<PlayerConnection>();
            if (player != null)
            {
                player.EnterSpotlight();
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
                player.ExitSpotlight();
            }
        }
    }
}
