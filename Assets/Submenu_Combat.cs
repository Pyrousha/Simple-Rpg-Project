using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Submenu_Combat : MonoBehaviour
{
    [SerializeField] private Transform buttonParent;
    [SerializeField] private GameObject buttonPrefab;
    private Animator anim;

    private List<AttackSpellButton> selectables = new List<AttackSpellButton>();
    private LinkSelectables linkSelectables;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        linkSelectables = buttonParent.GetComponent<LinkSelectables>();
    }

    public void SetStatus(bool _enabled)
    {
        if (selectables.Count == 0)
            return;

        anim.SetBool("Status", _enabled);

        if (_enabled)
            selectables[0].Select();
    }

    public void SetSpells(List<AttackSpell> _attackSpells)
    {
        while (selectables.Count < _attackSpells.Count)
        {
            GameObject newSelectableObj = Instantiate(buttonPrefab, buttonParent);
            selectables.Add(newSelectableObj.GetComponent<AttackSpellButton>());
        }

        for (int i = 0; i < selectables.Count; i++)
        {
            if (i < _attackSpells.Count)
            {
                //There is a spell for this selctable
                selectables[i].SetAttackSpell(_attackSpells[i]);
            }
            else
                selectables[i].SetAttackSpell(null);
        }

        linkSelectables.Link();
    }
}
