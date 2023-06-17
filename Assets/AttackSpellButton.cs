using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class AttackSpellButton : MonoBehaviour
{
    public AttackSpell AttackSpell { get; private set; }

    [SerializeField] private TextMeshProUGUI text;
    private Selectable selectable;

    private void Awake()
    {
        selectable = GetComponent<Selectable>();
    }

    public void SetAttackSpell(AttackSpell _newSpell)
    {
        if (_newSpell == null)
        {
            gameObject.SetActive(false);
            return;
        }

        AttackSpell = _newSpell;
        text.text = _newSpell.name;

        gameObject.SetActive(true);
    }

    public void Select()
    {
        selectable.Select();
    }
}
