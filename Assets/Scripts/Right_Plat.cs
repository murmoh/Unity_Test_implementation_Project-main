using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Right_Plat : MonoBehaviour
{
    public string Player = "Player";
    public bool Right_Active = false; 

    void OnTriggerEnter(Collider collision)
    {
        if(collision.transform.gameObject.name == Player)
        {
            Right_Active = true;
            Debug.Log("asd");
        }
    }
}