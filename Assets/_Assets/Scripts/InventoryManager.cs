using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    private int gold;

    private void Awake()
    {
        //TODO: Load gold value from save
    }

    public void AddGold(int _goldToGain)
    {
        gold += gold;
    }
}
