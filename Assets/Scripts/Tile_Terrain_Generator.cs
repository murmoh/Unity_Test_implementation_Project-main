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
    public float depthScale = 1.0f;
    public float pathWidth = 0.1f;
    public float riverWidth = 0.1f;
    public float riverDepth = 1.0f;
    public Color green;
    public Color darkGreen;
    public Color normalGreen;

    public void Start()
    {
        Generator();
    }

    Color ChooseColor()
    {
        int colorIndex = Random.Range(0, 3);
        switch (colorIndex)
        {
            case 0:
                return green;
            case 1:
                return darkGreen;
            case 2:
                return normalGreen;
            default:
                return Color.white;
        }
    }

    private float GeneratePath(float x, float z, float width)
    {
        float pathValue = Mathf.Sin(x * width) + Mathf.Cos(z * width);
        return pathValue;
    }

    private float GenerateRiver(float x, float z, float width)
    {
        float riverValue = Mathf.Abs(Mathf.Sin(x * width) + Mathf.Cos(z * width));
        return riverValue;
    }

    public void Generator()
    {
        seed = Random.Range(1, 1000000);
        if (Generated)
        {
            for (int x = 0; x < 10; x++)
            {
                for (int z = 0; z < 10; z++)
                {
                    float y = 0;
                    float perlinValue = Mathf.PerlinNoise(seed + x * 0.1f, seed + y * 0.1f + z * 0.1f);
                    float pathValue = GeneratePath(x, z, pathWidth);
                    float riverValue = GenerateRiver(x, z, riverWidth);
                    y = (perlinValue * heightScale - depthScale) * (1.0f - pathValue) - riverValue * riverDepth;
                    Vector3 spawnPosition = player.position + new Vector3(x, -y, z);
                    Vector3 Grass_Block_SpawnPosition = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);
                    GameObject new_Grass_Block = Instantiate(Grass_Block, Grass_Block_SpawnPosition, Quaternion.identity);
                    new_Grass_Block.GetComponent<Renderer>().material.color = ChooseColor();
                }
            }
        }
    }
}
