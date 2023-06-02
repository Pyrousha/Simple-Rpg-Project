using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Menubar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private Slider slider;

    private void Awake()
    {
        //Debug
        // SetUI(6500, 60000);
    }

    public void SetUI(int _value, int _maxValue)
    {
        slider.value = (float)_value / _maxValue;

        numberText.text = Utils.NumberToStringWithSpacers(_value, ",") + "/" + Utils.NumberToStringWithSpacers(_maxValue, ",");
    }
}
