using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = this.GetComponent<Slider>();
    }

    public void SetValue(float newValue)
    {
        slider.value = newValue;
    }

    public IEnumerator SetValueSmooth(float newValue, Action<float> handleUpdate)
    {
        float currentValue = slider.value;
        float changeInValue = currentValue - newValue;
        handleUpdate(currentValue);

        while (currentValue - newValue > Mathf.Epsilon)
        {
            currentValue -= changeInValue * Time.deltaTime;
            slider.value = currentValue;
            if (currentValue - newValue > Mathf.Epsilon)
            {
                handleUpdate(currentValue);
            }
            yield return null;
        }

        slider.value = newValue;
        handleUpdate(newValue);
    }
}
