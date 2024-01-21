using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;

public class Attacking : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private Image healthbar;
    [SerializeField] private float maxHealth = 100.0f;
    private float currentHealth;
    [SerializeField] private bool Regenerate;
    [SerializeField] private float regenDelay = 2.0f;
    private float regenDelayTimer;
    private float _time = 1.5f; // Added this line to declare _time variable

    private bool isInvulnerable = false;
    private float invulnerabilityDuration = 2.0f;
    private float respawnCooldown = 5.0f;
    private PhotonView view;

    private Vector3 respawnPosition = new Vector3(0, 10, 0);

    private void Start()
    {
        view = GetComponent<PhotonView>();
        currentHealth = maxHealth;
        regenDelayTimer = regenDelay;
        UpdateHealthBar();
    }

    private void Update()
    {
        if (Regenerate)
        {
            RegenerateHealth();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bullet") && !isInvulnerable)
        {
            TakeDamage(20f);
            UpdateHealthBar();
        }
    }

    private void TakeDamage(float damage)
    {
        if (!isInvulnerable)
        {
            currentHealth -= damage;
            Debug.Log("Player Health: " + currentHealth);

            if (currentHealth <= 0)
            {
                StartCoroutine(Respawn());
            }
        }
    }

    IEnumerator Respawn()
    {
        isInvulnerable = true;
        currentHealth = maxHealth;

        // Disable the CharacterController and set the respawn position
        GetComponent<CharacterController>().enabled = false;
        transform.position = respawnPosition;

        // Additional respawn logic (if needed)
        // For example, disable player controls during respawn

        yield return new WaitForSeconds(respawnCooldown);

        // Enable the CharacterController after the respawn cooldown
        GetComponent<CharacterController>().enabled = true;

        // Reset invulnerability after respawn
        isInvulnerable = false;
    }

    private void RegenerateHealth()
    {
        if (currentHealth < maxHealth)
        {
            regenDelayTimer -= Time.deltaTime;

            if (regenDelayTimer <= 0)
            {
                currentHealth += Time.deltaTime * (_time / maxHealth);
                UpdateHealthBar();
            }
        }
        else
        {
            regenDelayTimer = regenDelay;
        }
    }

    private void UpdateHealthBar()
    {
        if (healthbar != null)
        {
            healthbar.fillAmount = currentHealth / maxHealth;
        }
        else
        {
            Debug.LogWarning("Healthbar Image is not set");
        }
    }
}
