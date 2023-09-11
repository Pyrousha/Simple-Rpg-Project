using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemButton : UIButton, ISelectHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemQuantity;

    private Item item;
    private AttackSpell spell;

    bool isSpell = false;

    public void SetItem(Item _item, string _quantityText)
    {
        item = _item;
        isSpell = false;

        itemImage.sprite = _item.Icon;
        itemName.text = _item.name;

        itemQuantity.text = _quantityText;
    }

    public void SetSpell(AttackSpell _spell)
    {
        spell = _spell;
        isSpell = true;

        itemImage.sprite = _spell.Description.Icon;
        itemName.text = _spell.name;

        itemQuantity.text = $"({_spell.ManaCost} mp)";
    }

    public override void OnSelect(BaseEventData eventData)
    {
        //OverworldMenuController.Instance.OnSelectedItem()
    }
}
