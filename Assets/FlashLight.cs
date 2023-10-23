using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    public GameObject _flashLight;
    public Light pointLight;
    public float maxLightIntensity = 2f;
    public bool on = false;
    public AudioClip switchSound; // Reference to the switch sound effect
    private AudioSource audioSource; // Reference to the AudioSource component

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // If there's no AudioSource component on the flashlight, add one.
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Update()
    {
        LightController();
    }

    public void LightController()
    {
        if (Input.GetKeyDown("f"))
        {
            if(on)
            {
                pointLight.intensity = maxLightIntensity;
                on = false;
            }
            else if(!on)
            {
                pointLight.intensity = 0;
                on = true;
            }

            // Play the switch sound effect
            if (switchSound != null && audioSource != null)
            {
                audioSource.clip = switchSound;
                audioSource.Play();
            }
        }
    }
}
