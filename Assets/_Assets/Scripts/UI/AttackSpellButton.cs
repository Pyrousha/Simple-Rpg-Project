using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AttackSpellButton : MonoBehaviour, ISelectHandler
{
    private AttackSpell attackSpell;

    [SerializeField] private TextMeshProUGUI text;
    public Selectable Selectable { get; private set; }

    private void Awake()
    {
        Selectable = GetComponent<Selectable>();
    }

    public void SetAttackSpell(AttackSpell _newSpell)
    {
        if (_newSpell == null)
        {
            gameObject.SetActive(false);
            return;
        }

        attackSpell = _newSpell;
        text.text = _newSpell.name;

        gameObject.SetActive(true);
    }

    public void Select()
    {
        Selectable.Select();
    }

    public void OnClicked()
    {
        CombatController.Instance.OnSpellClicked(Selectable, attackSpell);
    }

    public void OnSelect(BaseEventData eventData)
    {
        DescriptionBox.Instance.SetUI(attackSpell.Description);
    }
}
