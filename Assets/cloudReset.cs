using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloudReset : MonoBehaviour
{
    public GameObject PosReset;
    public GameObject cloud1;
    public GameObject cloud2;
    public GameObject cloud3;


    public void OnTriggerEnter(Collider collider)
    {
        if(collider.transform.gameObject.name == "resetColider1")
        {
            cloud1.transform.position = PosReset.transform.position;
        }

        if(collider.transform.gameObject.name == "resetColider3")
        {
            cloud3.transform.position = PosReset.transform.position;
        }

        if(collider.transform.gameObject.name == "resetColider2")
        {
            cloud2.transform.position = PosReset.transform.position;
        }
    }


}
