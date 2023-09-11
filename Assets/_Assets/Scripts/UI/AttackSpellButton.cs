using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackSpellButton : UIButton, ISelectHandler
{
    private AttackSpell attackSpell;

    [SerializeField] private TextMeshProUGUI text;

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

    public void OnClicked()
    {
        CombatController.Instance.OnSpellClicked(C_Selectable, attackSpell);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        DescriptionBox.Instance.SetUI(attackSpell.Description);

        base.OnSelect(eventData);
    }
}
