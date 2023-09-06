using UnityEngine;

public class SubmenuSelectable : UIButton
{
    public enum SubmenuType
    {
        Item_Key,
        Item_Consumable,
        Equipment,
        AbilitySpell
    }

    [field: SerializeField] public SubmenuType Type { get; private set; }

    public void OnClicked()
    {
        OverworldMenuController.Instance.OnClickedSubmenuSelectable(this);
    }
}
