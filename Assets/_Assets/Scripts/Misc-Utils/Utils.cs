using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static List<Transform> GetChildrenFromParent(Transform parent)
    {
        List<Transform> toReturn = new List<Transform>();

        for (int i = 0; i < parent.childCount; i++)
        {
            toReturn.Add(parent.GetChild(i));
        }

        return toReturn;
    }

    public static float RoundToNearest(float value, float step)
    {
        return Mathf.Round(value / step) * step;
    }

    public static string NumberToStringWithSpacers(int _number, string _spacer)
    {
        string numberString = Mathf.Floor(_number).ToString();
        List<string> spacedNumbers = new();
        while (numberString.Length > 0)
        {
            if (numberString.Length > 3)
            {
                //Split and continue
                spacedNumbers.Insert(0, numberString.Substring(numberString.Length - 3));
                numberString = numberString.Substring(0, numberString.Length - 3);
            }
            else
            {
                // <= 3 numbers, no need to split more
                spacedNumbers.Insert(0, numberString);
                break;
            }
        }

        string numberToReturnString = "";
        while (spacedNumbers.Count > 0)
        {
            numberToReturnString += spacedNumbers[0];
            spacedNumbers.RemoveAt(0);

            if (spacedNumbers.Count > 0)
            {
                //Need to add spacer next
                numberToReturnString += _spacer;
            }
        }

        return numberToReturnString;
    }

    [System.Serializable]
    public class RangedInt
    {
        private int value;
        public int Value => value;

        private int maxValue;
        public int MaxValue => maxValue;

        public RangedInt(int _value, int _maxValue)
        {
            if (_value > _maxValue)
                Debug.LogError("Cannot have value be greater than maxvalue (value: " + _value + ", maxValue: " + _maxValue + ")");
            else
            {
                value = _value;
                maxValue = _maxValue;
            }
        }

        public void SetValue(int _value)
        {
            if (_value > maxValue)
                Debug.LogError("Cannot have value be greater than maxvalue (value: " + _value + ", maxValue: " + maxValue + ")");
            else
                value = _value;
        }

        public void SetMaxValue(int _maxValue)
        {
            if (_maxValue < value)
                Debug.LogError("Cannot have maxValue be smaller than value (value: " + value + ", maxValue: " + _maxValue + ")");
            else
                maxValue = _maxValue;
        }

        public void SetValueAndMax(int _value, int _maxValue)
        {
            if (_value > _maxValue)
                Debug.LogError("Cannot have value be greater than maxvalue (value: " + _value + ", maxValue: " + _maxValue + ")");
            else
            {
                value = _value;
                maxValue = _maxValue;
            }
        }
    }
}
