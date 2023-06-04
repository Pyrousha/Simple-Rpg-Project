using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Menubar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private Slider slider;
    [Header("")]
    [SerializeField] private TextMeshProUGUI xpLeftText;

    private void Awake()
    {
        //Debug
        // SetUI(6500, 60000);
    }

    public void SetUIValues(int _value, int _maxValue)
    {
        if (_maxValue > 0)
            slider.value = (float)_value / _maxValue;
        else
            slider.value = 0;

        numberText.text = Utils.NumberToStringWithSpacers(_value, ",") + "/" + Utils.NumberToStringWithSpacers(_maxValue, ",");

        if (xpLeftText != null)
        {
            string xpLeft = (_maxValue - _value).ToString();
            xpLeftText.text = xpLeft + "xp Left";
        }
    }

    public void SetUIValues(Utils.RangedInt _rangedInt)
    {
        SetUIValues(_rangedInt.Value, _rangedInt.MaxValue);
    }
}
