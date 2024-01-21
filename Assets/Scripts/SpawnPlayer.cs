using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject myPlayer;
    

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        GameObject playerObject = (GameObject)PhotonNetwork.Instantiate(this.myPlayer.name, Vector3.zero, Quaternion.identity, 0);
        playerObject.name = "Player";

        // Assuming your player prefab has a Transform component
        Transform playerTransform = playerObject.transform;

        // Set the initial position
        playerTransform.position = new Vector3(0f,50f,0f);
    }   
}
