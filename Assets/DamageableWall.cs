using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableWall : MonoBehaviour
{
    GameObject voxel_Wall;

    void Start()
    {
        voxel_Wall = gameObject; // Correctly reference the GameObject this script is attached to
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Bullet(Clone)")
        {
            voxel_Wall.SetActive(false); // Deactivate the GameObject
        }
    }
}
