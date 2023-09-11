using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    private int gold;
    [field: SerializeField] public Dictionary<Item, int> Items { get; private set; } = new Dictionary<Item, int>();

    private void Awake()
    {
        //TODO: Load gold value from save
        //TODO: load items from save
    }

    public void AddGold(int _goldToGain)
    {
        gold += _goldToGain;
    }

    public void OnUseItem(Item _item)
    {
        if (Items.ContainsKey(_item))
        {
            if (Items[_item] == 1)
            {
                //Used last of this item, remove it from inventory
                Items.Remove(_item);
                return;
            }

            Items[_item] -= 1;
        }
    }

    public void GainItem(Item _item)
    {
        if (Items.ContainsKey(_item))
            Items[_item] += 1;
        else
            Items.Add(_item, 1);
    }
}
