using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Last2Fall : MonoBehaviour
{
    [SerializeField] private float coolDown = 1f;
    [SerializeField] private Color32 white = new Color32(255,255,255,255);
    [SerializeField] private Color32 yellow = new Color32(255, 162, 0,255);
    [SerializeField] private Color32 orange = new Color32(255, 72, 0,255);

    private Renderer tile;
    public GameObject tiles;
    private Tile_Active script;

    private void Start()
    {
        tile = GetComponent<Renderer>();
        script = GetComponentInParent<Tile_Active>();
    }

    private void Update()
    {
        if (script.TileActive)
        {
            coolDown -= Time.deltaTime;

            if (coolDown > 1f)
            {
                tile.material.color = white;
            }
            else if (coolDown > (2f / 3f))
            {
                tile.material.color = yellow;
            }
            else if (coolDown > 0f)
            {
                tile.material.color = orange;
            }
            else
            {
                tiles.SetActive(false);
            }
        }
    }
}
