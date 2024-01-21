using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class Points : MonoBehaviour
{
    PhotonView view;
    public int points;
    public TextMeshProUGUI UI_Points;

    public void Start()
    {
        view = GetComponent<PhotonView>();
    }

    public void OnKillEnemy()
    {
        AddPoints(50);
        UpdatePointsText();
    }


    public void AddPoints(int amount)
    {
        points += amount;
    }


    public void UpdatePointsText()
    {
        UI_Points.text = "$" + points;
    }
}
