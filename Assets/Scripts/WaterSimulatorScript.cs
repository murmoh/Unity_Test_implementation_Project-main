using System.Collections.Generic;
using UnityEngine;

public class WaterSimulatorScript : MonoBehaviour
{
    public GameObject waterCubePrefab;
    public int width = 10;
    public int length = 10;
    public float cubeSize = 1.0f;
    public float waveSpeed = 0.5f;
    public float waveHeight = 1.0f;
    public Tile_Terrain_Generator terrainGenerator;


    private List<GameObject> waterCubes;

    void Start()
    {
        waterCubes = new List<GameObject>();
        GenerateWaterCubes();
    }

    void Update()
    {
        UpdateWaterCubes();
    }

    void GenerateWaterCubes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                Vector3 terrainPosition = new Vector3(x * cubeSize, 0, z * cubeSize);
                Vector3 cubePosition = terrainPosition + new Vector3(0, terrainGenerator.heightScale + cubeSize / 2, 0);
                GameObject cube = Instantiate(waterCubePrefab, cubePosition, Quaternion.identity, transform);
                waterCubes.Add(cube);
            }
        }
    }


    void UpdateWaterCubes()
    {
        for (int i = 0; i < waterCubes.Count; i++)
        {
            GameObject cube = waterCubes[i];
            Vector3 position = cube.transform.position;
            position.y = Mathf.Sin(Time.time * waveSpeed + (position.x + position.z) * waveSpeed) * waveHeight;
            cube.transform.position = position;
        }
    }
}
