using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class CountDownTimer : MonoBehaviour
{
    public float currentTime = -1f;
    public TextMeshProUGUI count;

    public Image progressBar;

    // start time
    float timeDuration;

    public WholeSampleSceneManager wholeSampleSceneManager;

    // Start is called before the first frame update
    void Start()
    {
        wholeSampleSceneManager = GetComponent<WholeSampleSceneManager>();
        progressBar.fillAmount = 0f;
        progressBar = GameObject.Find("Timer1_Cover").GetComponent<Image>();
    }

    public void StartCountDown( float startingTime)
    {
        progressBar = GameObject.Find("Timer1_Cover").GetComponent<Image>();
        count = GameObject.Find("T1").GetComponent<TextMeshProUGUI>();
        timeDuration = startingTime;
        currentTime = startingTime;
        count.text = startingTime.ToString();
        progressBar.fillAmount = 0f;
        wholeSampleSceneManager.StartAnimation();
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= 1 * Time.deltaTime;
            if (currentTime < 10)
            {
                count.text = " "+ Mathf.RoundToInt(currentTime).ToString();
            }
            else
            {
                count.text = Mathf.RoundToInt(currentTime).ToString();
            }
            
            progressBar.fillAmount += 1* Time.deltaTime / timeDuration;
            
        }
        else if (Mathf.RoundToInt(currentTime) == 0)
        {
            currentTime = -1f;
            wholeSampleSceneManager.StopAnimation();
            count.text = "";
            wholeSampleSceneManager.ChangeView();
        }
    }

}
