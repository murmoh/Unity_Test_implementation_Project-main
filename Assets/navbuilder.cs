using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class navbuilder : MonoBehaviour
{
    public NavMeshSurface Surface;
    // Start is called before the first frame update
    void Start()
    {
        Surface.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
