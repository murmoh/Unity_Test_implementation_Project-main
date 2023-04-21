using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Terrain_Generator : MonoBehaviour
{
    public GameObject Grass_Block;
    public Transform player;
    public bool Generated = true;
    public int seed;
    public float heightScale = 1f;

    public void Start()
    {
        Generator();
    }

    public void Generator()
    {
        seed = Random.Range(1, 1000000);
        if(Generated)
        {
            for (int x = 0; x < 10; x++)
            {
                for (int z = 0; z < 10; z++)
                {
                    float y = 0;
                    float perlinValue = Mathf.PerlinNoise(seed + x * 0.1f, seed + y * 0.1f + z * 0.1f);
                    y = perlinValue * heightScale;
                    float random = Mathf.PerlinNoise(0,6);
                    Vector3 spawnPosition = player.position + new Vector3(x, -y, z);
                    Vector3 Grass_Block_SpawnPosition = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);
                    GameObject new_Grass_Block = Instantiate(Grass_Block, Grass_Block_SpawnPosition, Quaternion.identity);
                }
            }
        }

    }

}
