using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentControl : MonoBehaviour
{
    
    public GameObject Clouds;
    public GameObject Clouds2;
    public GameObject Clouds3;
    public float _speedCloud = 0.1f;
    public GameObject SkyBox;
    public GameObject SkyBox2;
    public float _timePerDay = 600f;
    public Light directionalLight; // Reference to the directional light (sun)
    public GameObject sunPivot; // The pivot point for the sun rotation
    public float maxLightIntensity = 1f; // Maximum intensity during the day
    public float minLightIntensity = 0.2f; // Minimum intensity during the night

    private float _timeSinceStart = 0f;
    private Material _skyboxMaterial;
    private Material _skyboxMaterial2;

    [SerializeField] private Transform PlayerFollow;

    private void Start()
    {
        if (SkyBox)
        {
            _skyboxMaterial = SkyBox.GetComponent<Renderer>().material;
            _skyboxMaterial2 = SkyBox2.GetComponent<Renderer>().material;
        }
    }

    private void Update()
    {
        // Move Clouds
        MoveClouds(Clouds);
        MoveClouds(Clouds2);
        MoveClouds(Clouds3);

        // Update Skybox Offset
        if (_skyboxMaterial)
        {
            _timeSinceStart += Time.deltaTime;
            float offset = (_timeSinceStart / _timePerDay) % 1;
            _skyboxMaterial.SetTextureOffset("_MainTex", new Vector2(offset, 0));
            _skyboxMaterial2.SetTextureOffset("_MainTex", new Vector2(offset, 0));

            // Adjust the light intensity based on the current time
            float intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, Mathf.Sin(offset * Mathf.PI));
            if (directionalLight)
            {
                directionalLight.intensity = intensity;
            }
        }

        // Rotate the sun based on the current time
        if (sunPivot)
        {
            float rotationSpeed = 360f / _timePerDay; // Degrees per second
            sunPivot.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime); // Rotate around the X-axis
        }

        // Make sure the light points away from the pivot
        if (directionalLight && sunPivot)
        {
            directionalLight.transform.forward = (directionalLight.transform.position - sunPivot.transform.position).normalized;
        }

        directionalLight.transform.LookAt(PlayerFollow);
    }

    private void MoveClouds(GameObject cloud)
    {
        if (cloud)
        {
            Vector3 cloudPosition = cloud.transform.position;
            cloudPosition.z += _speedCloud * Time.deltaTime;
            cloud.transform.position = cloudPosition;
        }
    }
}
