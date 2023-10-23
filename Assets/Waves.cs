using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;
using UnityEngine;

public class Waves : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private TextMeshProUGUI waveText; 
    [SerializeField] private int waveNum = 0;
    public float timer = 10f;
    private int increase;

    [Header("Spawner Setting")]
    [SerializeField] private GameObject entity;  
    [SerializeField] private Transform spawnerPlat;
    [SerializeField] private bool spawn;  
    public GameObject[] mobs;  
    [SerializeField] private int amount;
    [SerializeField] private int amountLimit = 20;  // Set initial limit
    [SerializeField] private float timePerSpawn = 2f;

    [Header("Nav Mesh Settings")]
    public NavMeshSurface Surface;
    public GameObject stage;
    public float stageDble = 0f;
    public Transform stageSpawn;
    public Transform spawner;
    
    void Start()
    {
        spawn = true;
        Surface.BuildNavMesh();
    }

    private void Update()
    {
        UpdateMobsArray();
        Spawner();
        WaveOn();
    }

    private void UpdateMobsArray()
    {
        mobs = GameObject.FindGameObjectsWithTag("Mob");
    }

    private void Spawner()
    {
        if (spawn && amountLimit > 0)
        {
            if (timePerSpawn <= 0f)
            {
                for(int i = 0; i <= (amountLimit / 2); i++)
                {
                    Vector3 randomPosition = RandomPositionWithinSpawner(); 
                    Instantiate(entity, randomPosition, spawnerPlat.rotation);
                    amount = 1;
                    amountLimit -= amount;
                }
                
                timePerSpawn = 10f;
            }

            timePerSpawn -= Time.deltaTime;
        }
    }

    private Vector3 RandomPositionWithinSpawner()
    {
        Renderer rend = spawnerPlat.GetComponent<Renderer>();
        Vector3 minBounds = rend.bounds.min;
        Vector3 maxBounds = rend.bounds.max;

        float x = Random.Range(minBounds.x, maxBounds.x);
        float z = Random.Range(minBounds.z, maxBounds.z);

        // Calculate the height based on the terrain at the (x, z) position
        float y = GetTerrainHeightAtPosition(new Vector3(x, 0, z));

        return new Vector3(x, y, z);
    }

    private float GetTerrainHeightAtPosition(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 100, Vector3.down, out hit, 200, LayerMask.GetMask("NavMeshSet")))
        {
            return hit.point.y;
        }
        return 0f; // Return a default height if not found
    }

    private void WaveOn()
    {
        if(amountLimit == 0)
        {
            spawn = false;
        }

        if (mobs.Length == 0 && !spawn)
        {
            waveNum += 1;
            increase += 2;
            amountLimit = 5 * increase;
            waveText.text = "Wave: " + waveNum;
            spawn = true;
        }
    }

}
