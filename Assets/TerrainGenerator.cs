using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class TerrainGenerator : MonoBehaviour
{

    [Header("Material Settings")]
    public Material terrainMaterial;
    public Material cliffMaterial;

    [Header("Noise Settings")]
    public int sizeX = 150;
    public int sizeY = 150;
    public float scale = 30f;
    public float amplitude = 18.36f;

    [Header("Mesh Settings")]
    public MeshFilter meshFilter;
    private MeshCollider meshCollider;  // Add this line

    [Header("Raycast Settings")]
    [SerializeField] GameObject prefab;
    [SerializeField] int density;
    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;
    [SerializeField] Vector2 xRange;
    [SerializeField] Vector2 zRange;

    [Header("Prefab Variation Settings")]
    [SerializeField, Range(0, 1)] float rotateTowardsNormal;
    [SerializeField] Vector2 rotationRange;
    [SerializeField] Vector3 minScale;
    [SerializeField] Vector3 maxScale;

    [Header("Vegetation Settings")]
    public GameObject[] grassPrefabs; // Array for different types of grass
    public GameObject[] treePrefabs; // Array for different types of trees
    public int grassDensity;
    public int treeDensity;

    [Header("Segment Settings")]
    public int segmentsX = 1;  // Number of segments along X-axis
    public int segmentsY = 1;  // Number of segments along Y-axis
    public int segmentIndexX = 0;  // Current segment's index along X-axis
    public int segmentIndexY = 0;  // Current segment's index along Y-axis

    [Header("Cliff Settings")]
    public float cliffThreshold = 0.8f; // Noise value at which cliffs start
    public float cliffHeight = 40f; // How much height to add when creating a cliff


    [Header("Nav Mesh Settings")]
    public NavMeshSurface Surface;
    

    void Start()
    {
        meshCollider = GetComponent<MeshCollider>(); // Initialize meshCollider
        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        Renderer renderer = GetComponent<Renderer>();
        renderer.materials = new Material[] { terrainMaterial, cliffMaterial };
        Generate();
        Invoke("BuildNavMesh", 0.1f);
    }

    void BuildNavMesh()
    {
        Surface.BuildNavMesh();
    }

    public void Generate()
    {
        Mesh terrainMesh = GenerateTerrainMesh();
        meshFilter.mesh = terrainMesh;

        // Assign the generated mesh to the MeshCollider
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = terrainMesh;

        PopulateTerrainWithPrefabs();
        GenerateGrass();
        GenerateTrees();
    }


    Mesh GenerateTerrainMesh()
    {
        List<int> terrainTriangles = new List<int>();
        List<int> cliffTriangles = new List<int>();

        Vector3[] vertices = new Vector3[sizeX * sizeY];
        int[] triangles = new int[(sizeX - 1) * (sizeY - 1) * 6];
        Vector2[] uv = new Vector2[vertices.Length];
        int vertexIndex = 0;
        int triangleIndex = 0;

        Vector2 islandCenter = new Vector2(sizeX / 2f, sizeY / 2f);

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                float globalX = x + segmentIndexX * sizeX;
                float globalY = y + segmentIndexY * sizeY;
                float noiseValue = Mathf.PerlinNoise(globalX / scale, globalY / scale);

                float height = noiseValue * amplitude;

                // Add island-like falloff
                Vector2 position = new Vector2(x, y);
                float distanceToCenter = Vector2.Distance(islandCenter, position) / (sizeX / 2f);
                float falloff = Mathf.Clamp01(1f - distanceToCenter * distanceToCenter);
                height *= falloff;

                Vector3 vertexPosition = new Vector3(x, height, y);
                vertices[vertexIndex] = vertexPosition;

                if (x < sizeX - 1 && y < sizeY - 1)
                {
                    int[] currentTriangles = {
                        vertexIndex,
                        vertexIndex + sizeX,
                        vertexIndex + sizeX + 1,
                        vertexIndex,
                        vertexIndex + sizeX + 1,
                        vertexIndex + 1
                    };

                    if (noiseValue > cliffThreshold)
                    {
                        cliffTriangles.AddRange(currentTriangles);
                    }
                    else
                    {
                        terrainTriangles.AddRange(currentTriangles);
                    }
                }

                uv[vertexIndex] = new Vector2(x / (float)sizeX, y / (float)sizeY);
                vertexIndex++;
            }
        }

        Mesh mesh = new Mesh
        {
            vertices = vertices,
            uv = uv,
            subMeshCount = 2
        };

        mesh.SetTriangles(terrainTriangles.ToArray(), 0);
        mesh.SetTriangles(cliffTriangles.ToArray(), 1);
        mesh.RecalculateNormals();

        return mesh;
    }




    void PopulateTerrainWithPrefabs()
    {
        for (int i = 0; i < density; i++)
        {
            float sampleX = Random.Range(xRange.x, xRange.y);
            float sampleY = Random.Range(zRange.x, zRange.y);
            Vector3 rayStart = new Vector3(sampleX, maxHeight, sampleY);

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit))
            {
                if (hit.point.y < minHeight)
                {
                    continue;
                }

                GameObject instantiatedPrefab = Instantiate(prefab, hit.point, Quaternion.identity);
                instantiatedPrefab.transform.rotation = Quaternion.Lerp(Quaternion.FromToRotation(Vector3.up, hit.normal), Quaternion.Euler(Random.Range(rotationRange.x, rotationRange.y), 0, 0), rotateTowardsNormal);
                instantiatedPrefab.transform.localScale = new Vector3(
                    Random.Range(minScale.x, maxScale.x),
                    Random.Range(minScale.y, maxScale.y),
                    Random.Range(minScale.z, maxScale.z)
                );
            }
        }
    }

    void GenerateGrass()
    {
        for (int i = 0; i < grassDensity; i++)
        {
            if (GetRandomPositionOnTerrain(out Vector3 position))
            {
                GameObject grassPrefab = grassPrefabs[Random.Range(0, grassPrefabs.Length)];
                Instantiate(grassPrefab, position, Quaternion.identity);
            }
        }
    }

    void GenerateTrees()
    {
        for (int i = 0; i < treeDensity; i++)
        {
            if (GetRandomPositionOnTerrain(out Vector3 position))
            {
                GameObject treePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
                Instantiate(treePrefab, position, Quaternion.identity);
            }
        }
    }

    bool GetRandomPositionOnTerrain(out Vector3 position)
    {
        float sampleX = Random.Range(xRange.x, xRange.y);
        float sampleY = Random.Range(zRange.x, zRange.y);
        Vector3 rayStart = new Vector3(sampleX, maxHeight, sampleY);

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit))
        {
            position = hit.point;
            return true;
        }
        
        position = Vector3.zero;
        return false;
    }
}
