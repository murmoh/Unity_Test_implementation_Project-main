using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class InPartyStart : MonoBehaviourPunCallbacks
{
    public InputField createInput;


    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom("991239129932");
    }


    public override void OnJoinedLobby()
    {
        PhotonNetwork.LoadLevel("SampleScene");
    }
}
