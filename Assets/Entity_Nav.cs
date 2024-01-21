using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Entity_Nav : MonoBehaviour
{
    private GameObject[] Target;
    public GameObject Entity;
    private NavMeshAgent nav;
    public float speed = 5f;
    float timer = 3f;

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        nav.speed = speed;
    }
    
    void Update()
    {
        timer -= Time.deltaTime;

        Target = GameObject.FindGameObjectsWithTag("Player");
        if(timer <= 0)
        {
            if(Target != null)
            {
                nav.destination = Target[0].transform.position;
            }
        }
    }
}
