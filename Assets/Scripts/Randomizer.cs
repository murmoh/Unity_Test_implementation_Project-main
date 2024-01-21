using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomizer : MonoBehaviour
{
    [Header("Entity")]
    public string Player = "Player";
    [Header("Platform")]
    public GameObject r_Plat;
    public GameObject l_Plat;
    public float Rand;
    bool Active = true;
    public Left_Plat Lefty;
    public Right_Plat Righty;
    

    public void Start()
    {
        
    }


    public void Update()
    {
        if(Active)
        {
            Rand = Random.Range(0f,1f);
            Active = false;
        }
        Debug.Log(Rand);

        Generator();
    }


    public void Generator()
    {
        
        if(Rand > 0.5f)
        {
            if(Lefty.Left_Active == true)
            {
                l_Plat.SetActive(false);
            }
        }

        if(Rand < 0.5f)
        {
            if(Righty.Right_Active == true)
            {
                r_Plat.SetActive(false);
            }
        }
        
    }

}
