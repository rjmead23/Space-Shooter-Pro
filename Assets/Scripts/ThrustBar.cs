using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrustBar : MonoBehaviour
{
    public Slider slider;
    public bool _isCoolDown = true;

    private void Start()
    {
        StartCoroutine(RechargeThrusters());
    }

    public void SetMaxThrust(int thrust)
    {
        slider.maxValue = thrust;
    }

    public void SetThrust(int thrust)
    {
        slider.value = thrust;
    }

    public void ReduceThrust(float thrust)
    {
        if (slider.value == 0 && _isCoolDown == false)
        {
            _isCoolDown = true;
            StartCoroutine(RechargeThrusters());
            return;
        }

        slider.value = slider.value -= thrust;
    }

    void IncreaseThrust(float thrust)
    {
        slider.value = slider.value + thrust;
    }

    public void ResetCoolDown()
    {
        _isCoolDown = false;
    }

    IEnumerator RechargeThrusters()
    {
 
        for (int i = 0; i < slider.maxValue; i++)
        {
            yield return new WaitForSeconds(1);
            IncreaseThrust(1);
        }
        ResetCoolDown();
    }

}
