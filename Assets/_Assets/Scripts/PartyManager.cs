using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyManager : Singleton<PartyManager>
{
    [SerializeField] private CharacterMenuDisplay[] characterDisplays;
    [field: SerializeField] public List<OverworldEntity> PartyMembers { get; private set; }

    private void Start()
    {
        //TODO: Load characters in party from save instead of serializedField

        SetCharacterDisplays();
    }

    private void SetCharacterDisplays()
    {
        //For each display, set character info
        for (int i = 0; i < characterDisplays.Length; i++)
        {
            if (PartyMembers.Count > i)
                //Has a party member for this index
                characterDisplays[i].SetEntity(PartyMembers[i]);
            else
                characterDisplays[i].SetEntity(null);
        }
    }

    public List<Selectable> AlivePartyMembers()
    {
        List<Selectable> list = new List<Selectable>();
        foreach (CharacterMenuDisplay display in characterDisplays)
        {
            if (display.Entity != null && !display.Entity.IsDead)
                list.Add(display.C_Selectable);
        }

        LinkSelectables.LinkSpecified(list, 1);

        return list;
    }
    public OverworldEntity GetFirstAlivePlayer()
    {
        foreach (OverworldEntity partyMember in PartyMembers)
        {
            if (!partyMember.IsDead)
                return partyMember;
        }

        Debug.LogError("No alive party member found!");

        return null;
    }
}
