using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using Mirror;



public class PlayerHealthSystem : NetworkBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private Image healthbar;
    [SerializeField] private float fullHealth = 100.0f;
    private float currentHealth;
    [SerializeField] private float decreaseHealthAmount = 100;
    [SerializeField] private bool[] currentWeapon;
    [SerializeField] private bool Regenerate;
    [SerializeField] private float _time = 1.5f;
    [SerializeField] private float regenDelay = 2.0f;
    private float regenDelayTimer;
    [SerializeField] private bool hitRangeActive;
    public int collidingMobsCount = 0;



    void Start()
    {
        currentHealth = fullHealth;
        regenDelayTimer = regenDelay;  // Initialize the regen delay timer
        UpdateHealthBar();

        if (currentWeapon != null && currentWeapon.Length > 0)
        {
            currentWeapon[0] = true;
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if(currentHealth < 100)
        {
            Regenerate = true;
        }
        else
        {
            Regenerate = false;
        }

        if (hitRangeActive)
        {
            regenDelayTimer = regenDelay;  // Reset the delay timer when attacked
            DecreaseHealth(decreaseHealthAmount);  // Apply deltaTime to make it frame rate independent
        }
        else
        {
            regenDelayTimer -= Time.deltaTime;  // Count down the delay timer

            if (Regenerate && regenDelayTimer <= 0 && _time <= 0)
            {
                reGen();
                
            }
            _time -= Time.deltaTime;  // Reduce the regeneration timer when not in hit range
        }
    }


    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.gameObject.tag == "Mob")
        {
            collidingMobsCount++;
            UpdateHitRangeActiveStatus();
        }
        else
        {

        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.transform.gameObject.tag == "Mob")
        {
            collidingMobsCount--;
            UpdateHitRangeActiveStatus();
        }
        else
        {

        }
    }

    void reGen()
    {
        if (currentHealth < fullHealth && !hitRangeActive)
        {
            currentHealth += 10f * Time.deltaTime;  // Regenerate health

            if (currentHealth >= fullHealth)
            {
                currentHealth = fullHealth;  // Cap health at fullHealth
            }

            if(currentHealth >= 100f)
            {
                _time = 1.5f;  // Reset the timer
            }

            UpdateHealthBar();  // Update the health bar here
        }
    }


    public void EnemyKilled(GameObject enemy)
    {
        collidingMobsCount--;
        UpdateHitRangeActiveStatus();
        Debug.Log("Enemy killed. collidingMobsCount: " + collidingMobsCount);
    }


    private void DecreaseHealth(float baseAmount)
    {
        float actualAmount = baseAmount * collidingMobsCount * Time.deltaTime;
        currentHealth -= actualAmount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
        UpdateHealthBar();
    }

    void UpdateHitRangeActiveStatus()
    {
        hitRangeActive = (collidingMobsCount > 0);
    }




    private void UpdateHealthBar()
    {
        if (healthbar != null)
        {
            healthbar.fillAmount = currentHealth / fullHealth;
        }
        else
        {
            Debug.LogWarning("Healthbar Image is not set");
        }
    }


    void SetDecreaseHealthAmount()
    {
        if (currentWeapon != null && currentWeapon.Length > 0 && currentWeapon[0])
        {
            decreaseHealthAmount = 50;
        }
        else
        {
            decreaseHealthAmount = 0;
        }
    }
}
