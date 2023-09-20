using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSpellButton : UIButton, ISelectHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemQuantity;

    public Item Item { get; private set; }
    public AttackSpell Spell { get; private set; }

    public bool IsSpell { get; private set; }

    public void SetItem(Item _item, string _quantityText)
    {
        Item = _item;
        IsSpell = false;

        itemImage.sprite = _item.Icon;
        itemName.text = _item.name;

        itemQuantity.text = _quantityText;
    }

    public void SetSpell(AttackSpell _spell)
    {
        Spell = _spell;
        IsSpell = true;

        itemImage.sprite = _spell.Description.Icon;
        itemName.text = _spell.name;

        itemQuantity.text = $"({_spell.ManaCost} mp)";
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        OverworldMenuController.Instance.OnSelectedItemSpell(this);
    }

    public void OnClick()
    {
        OverworldMenuController.Instance.OnClickedItemSpell(this);
    }
}
