using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [SerializeField] public Image Loadings;
    private const float Load_Max = 100f;
    public float Load = Load_Max;


    void Start()
    {
        Loadings = GetComponent<Image>();
    }

    void Update()
    {
    
        Loadings.fillAmount += Load / Load_Max * 0.01f;
        if(Loadings.fillAmount == 1)
        {
            Loadings.fillAmount = 0;
        }
    }
}
