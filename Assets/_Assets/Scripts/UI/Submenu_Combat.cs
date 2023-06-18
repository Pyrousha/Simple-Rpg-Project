using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Submenu_Combat : MonoBehaviour
{
    [SerializeField] private Transform buttonParent;
    [SerializeField] private GameObject buttonPrefab;
    private Animator anim;

    private List<AttackSpellButton> attackSpellButtons = new List<AttackSpellButton>();
    private LinkSelectables linkSelectables;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        linkSelectables = buttonParent.GetComponent<LinkSelectables>();
    }

    public void SetStatus(bool _enabled)
    {
        if (attackSpellButtons.Count == 0)
            return;

        anim.SetBool("Status", _enabled);

        if (_enabled)
            attackSpellButtons[0].Select();
    }

    public List<Selectable> SetSpells(List<AttackSpell> _attackSpells, int _mpLeft)
    {
        while (attackSpellButtons.Count < _attackSpells.Count)
        {
            GameObject newSelectableObj = Instantiate(buttonPrefab, buttonParent);
            attackSpellButtons.Add(newSelectableObj.GetComponent<AttackSpellButton>());
        }

        for (int i = 0; i < attackSpellButtons.Count; i++)
        {
            if (i < _attackSpells.Count)
            {
                //There is a spell for this selctable
                attackSpellButtons[i].SetAttackSpell(_attackSpells[i]);
                attackSpellButtons[i].Selectable.interactable = (_mpLeft >= _attackSpells[i].ManaCost);
            }
            else
                attackSpellButtons[i].SetAttackSpell(null);
        }

        linkSelectables.Link();

        List<Selectable> selectablesToReturn = new List<Selectable>();
        foreach (AttackSpellButton button in attackSpellButtons)
        {
            selectablesToReturn.Add(button.Selectable);
        }
        return selectablesToReturn;
    }
}
