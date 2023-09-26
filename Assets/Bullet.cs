using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f; // How long the bullet will exist before destroying itself
    public int damage = 10; // Amount of damage the bullet will do

    void Start()
    {
        Destroy(gameObject, lifeTime); // The bullet will destroy itself after a certain time
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider hitInfo)
    {
        // Apply damage here or do something else when the bullet hits an object
        Debug.Log("Bullet hit " + hitInfo.name);
        
        // Destroy the bullet upon collision
        Destroy(gameObject);
    }
}
