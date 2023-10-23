using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private Image healthbar;
    [SerializeField] private float fullHealth = 100.0f;
    private float currentHealth;
    [SerializeField] private float decreaseHealthAmount = 0;
    [SerializeField] private bool[] currentWeapon;
    public Bullet damage;

    [Header("Regeneration Settings")]
    [SerializeField] private float timeToStartRegeneration = 5f; // Time to wait before starting regeneration
    [SerializeField] private float regenerationRate = 1f; // How fast to regenerate
    private float lastTimeDamaged; // Last time when damage was taken

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackStrength = 10.0f;
    [SerializeField] private float knockbackDuration = 0.2f;
    private Vector3 originalPosition;

    [Header("Drops")]
    public GameObject Ammo;
    private float ammoDropRate = 0.15f; // 15% chance to drop ammo

    [Header("Score Settings")]
    public Points score;

    void Start()
    {
        score = FindObjectOfType<Points>();
        GameObject bullet = GameObject.FindGameObjectWithTag("bullet");
        if (bullet != null)
        {
            damage = bullet.GetComponent<Bullet>();
        }

        currentHealth = fullHealth;
        UpdateHealthBar();

        if (currentWeapon != null && currentWeapon.Length > 0)
        {
            currentWeapon[0] = true;
        }

        lastTimeDamaged = Time.time;
        originalPosition = transform.position;
    }


    void Update()
    {
        if (damage == null)
        {
            GameObject bullet = GameObject.FindGameObjectWithTag("bullet");
            if (bullet != null)
            {
                damage = bullet.GetComponent<Bullet>();
            }
        }

        // Check for regeneration
        if (Time.time - lastTimeDamaged > timeToStartRegeneration && currentHealth < fullHealth)
        {
            currentHealth += regenerationRate * Time.deltaTime;
            if (currentHealth > fullHealth)
            {
                currentHealth = fullHealth;
            }
            UpdateHealthBar();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 15% chance to drop ammo
        if (Random.Range(0f, 1f) <= ammoDropRate)
        {
            GameObject DropSpawner = Instantiate(Ammo, gameObject.transform.position, Quaternion.identity);
        }

        PlayerHealthSystem playerHealthSystem = GameObject.FindObjectOfType<PlayerHealthSystem>();
        if (playerHealthSystem != null)
        {
            playerHealthSystem.EnemyKilled(this.gameObject);
        }

        Destroy(gameObject);
    }

    

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "bullet")
        {
            SetDecreaseHealthAmount();
            DecreaseHealth(decreaseHealthAmount);
            lastTimeDamaged = Time.time; // Reset the timer
        }
    }

    private void DecreaseHealth(float amount)
    {
        currentHealth -= amount;
        score.AddPoints(50);
        score.UpdatePointsText();
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            // Destroy or disable gameObject (commented out for testing)
            // Destroy(gameObject);
        }
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthbar.fillAmount = currentHealth / fullHealth;
    }

    void SetDecreaseHealthAmount()
    {
        if (currentWeapon != null && currentWeapon.Length > 0 && currentWeapon[0])
        {
            decreaseHealthAmount = damage != null ? damage.damage : 0;
        }
        else
        {
            decreaseHealthAmount = 0;
        }
    }

}
