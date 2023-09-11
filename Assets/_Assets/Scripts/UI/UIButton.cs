using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIButton : Button, ISelectHandler, IDeselectHandler
{
    [Space(25)]
    [Header("UIButton Stuff")]

    private List<Graphic> graphics = new List<Graphic>();
    private Button button;

    private Color disabledColor_normal;
    [SerializeField] private Color disabledColor_selected;
    [SerializeField] private DescriptionBox.DescriptionInfo description;

    public Selectable C_Selectable { get; private set; }

    protected override void Awake()
    {
        graphics = new List<Graphic>(GetComponentsInChildren<Graphic>());
        button = GetComponent<Button>();
        disabledColor_normal = button.colors.disabledColor;

        C_Selectable = GetComponent<Selectable>();
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        var targetColor =
            state == SelectionState.Disabled ? colors.disabledColor :
            state == SelectionState.Highlighted ? colors.highlightedColor :
            state == SelectionState.Normal ? colors.normalColor :
            state == SelectionState.Pressed ? colors.pressedColor :
            state == SelectionState.Selected ? colors.selectedColor : Color.white;

        foreach (Graphic graphic in graphics)
        {
            graphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        if (!button.interactable)
        {
            ColorBlock cb = button.colors;
            cb.disabledColor = disabledColor_selected;
            button.colors = cb;
        }

        if (description != null)
            DescriptionBox.Instance?.SetUI(description);
        else
            DescriptionBox.Instance?.SetStatus(false);

        base.OnSelect(eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        if (!button.interactable)
        {
            ColorBlock cb = button.colors;
            cb.disabledColor = disabledColor_normal;
            button.colors = cb;
        }

        base.OnDeselect(eventData);
    }
}