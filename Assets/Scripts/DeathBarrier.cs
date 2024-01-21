using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeathBarrier : MonoBehaviour
{
    GameObject player;
    PhotonView view;
    public Transform teleportPoint;

    // Remove the reference to CharacterController

    public void Start()
    {
        // You can leave this empty if there's nothing to initialize on Start
    }

    public void OnTriggerEnter(Collider collider)
    {
        Teleport();
    }

    public void Teleport()
    {
        player = GameObject.Find("Player");

        // Assuming your player prefab has a Transform component
        Transform playerTransform = player.transform;

        // Set the player's position to the teleport point
        playerTransform.position = teleportPoint.position;

        Debug.Log("HIT");
    }
}
