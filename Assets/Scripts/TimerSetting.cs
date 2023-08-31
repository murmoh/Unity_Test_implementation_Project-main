using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimerSetting : MonoBehaviour
{

    public TextMeshProUGUI Timer;
    public Image loadbar;
    public bool Active;
    private float startTime = 60f;  // New variable to count the number of messages
    private float endTime = 0f;  // Total number of messages, should match with server messages

    void Start()
    {
        Active = false;
    }

    
    void Update()
    {
        if(Active)
            Timer.text = string.Format("{0}",Mathf.Round(startTime));
    }


    public void OnTriggerEnter(Collider collider)
    {
        if(collider.transform.gameObject.tag == "Player")
        {
            Active = true;
        }
    }


}
