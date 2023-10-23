using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f; // How long the bullet will exist before destroying itself
    public int damage = 10; // Amount of damage the bullet will do
    public GameObject bloodPrefab;

    void Start()
    {
        Destroy(gameObject, lifeTime); // The bullet will destroy itself after a certain time
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision hitInfo)
    {
        
        if (hitInfo.gameObject.tag == "Mob")
        {
            // Create blood effect at the point of collision before destroying the bullet
            Vector3 position = hitInfo.contacts[0].point;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.contacts[0].normal);
            GameObject blood = Instantiate(bloodPrefab, position, rotation);
            Destroy(blood, lifeTime);
        }
        
        // Optionally, you might want to set the parent of the blood effect to the hit object
        // blood.transform.SetParent(hitInfo.transform);

        // Destroy the bullet upon collision
        Destroy(gameObject);
    }
}
