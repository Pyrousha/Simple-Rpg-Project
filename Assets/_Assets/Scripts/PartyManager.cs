using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : Singleton<PartyManager>
{
    [SerializeField] private CharacterMenuDisplay[] characterDisplays;
    [field: SerializeField] public CombatEntity[] PartyMembers { get; private set; }

    private void Awake()
    {
        SetCharacterDisplays();
    }

    private void SetCharacterDisplays()
    {
        //For each display, set character info
        for (int i = 0; i < characterDisplays.Length; i++)
        {
            if (PartyMembers.Length > i)
                //Has a party member for this index
                characterDisplays[i].SetEntity(PartyMembers[i]);
            else
                characterDisplays[i].SetEntity(null);
        }
    }
}
