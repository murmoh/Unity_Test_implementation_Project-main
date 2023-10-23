using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollow : MonoBehaviour
{
    [SerializeField] private Transform PlayerFollow;
    

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(PlayerFollow);
    }
}
