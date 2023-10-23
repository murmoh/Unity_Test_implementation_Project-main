using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Points : MonoBehaviour
{
    public int points;
    public TextMeshProUGUI UI_Points;


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
