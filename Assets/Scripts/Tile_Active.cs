using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Active : MonoBehaviour
{
    public GameObject Player;
    public bool TileActive = false; 

    void OnTriggerEnter(Collider collision)
    {
        if((collision.transform.gameObject.name == "Player"))
        {
            TileActive = true;
        }
    }

    void OnTriggerExit()
    {
        TileActive = false;
    }
}
