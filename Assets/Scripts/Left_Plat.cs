using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Left_Plat : MonoBehaviour
{
    public string Player = "Player";
    public bool Left_Active = false; 

    void OnTriggerEnter(Collider collision)
    {
        if(collision.transform.gameObject.name == Player)
        {
            Left_Active = true;
        }
    }
}