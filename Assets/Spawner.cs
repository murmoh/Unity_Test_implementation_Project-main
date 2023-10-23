using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public NavMeshSurface Surface;
    public GameObject _Zombie;
    public int SpawnAmount = 3;
    public Transform spawnPos;
    public bool Spawn;

    void Start()
    {
        Spawn = false;
    }

    void Update()
    {
        if(Spawn)
        {
            for (int i = 0; i < SpawnAmount; i++)
            {
                GameObject zombie = Instantiate(_Zombie, spawnPos.position, spawnPos.rotation);
            }
            Spawn = false;
        }

    }
}
