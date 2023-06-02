using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Button button;
    [SerializeField] private TextMeshProUGUI text;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Debug.Log(gameObject.name + "Selected");

        text.color = button.colors.selectedColor;
    }

    public void OnDeselect(BaseEventData data)
    {
        // Debug.Log(gameObject.name + "Deselected");

        text.color = button.colors.normalColor;
    }
}
