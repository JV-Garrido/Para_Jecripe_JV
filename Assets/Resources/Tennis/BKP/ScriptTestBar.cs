using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptTestBar : MonoBehaviour
{

    public Slider slider;
    public Image fill;
    private float red;
    private float green;
    // Start is called before the first frame update
    void Start()
    {
        slider.value = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            slider.value = slider.value + Time.deltaTime;
            if (slider.value > 1f)
            {
                slider.value = 1f;
            }
        }
        else
        {
            slider.value = 0f;
        }

        if (slider.value > 0.5f)
        {
            red = 255f;
            green = 255f * (1f - 2f * (slider.value - 0.5f));
        }
        else
        {
            green = 255f;
            red = 255f * 2f * slider.value;
            
        }
        fill.color = new Color32(Convert.ToByte(red), Convert.ToByte(green), 0, 255);
    }
}
