using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using static BaseCombatEntity;

public class CombatEntity : MonoBehaviour
{
    [System.Serializable]
    public enum StatType_Enum
    {
        MaxHp,
        MaxMp,
        Atk_P,
        Atk_M,
        Def_P,
        Def_M,
        Speed
    }

    [SerializeField] private BaseCombatEntity baseStats;
    [Space(10)]
    [SerializeField, ContextMenuItem("TakeDamage", nameof(DEBUG_TAKE_DAMAGE))] private Menubar hpBar;
    [SerializeField] private Menubar mpBar;

    private int level;
    public int Level => level;

    private int hp;
    public int Hp => hp;
    [System.NonSerialized] public ModifiableStat MaxHp;

    private int mp;
    public int Mp => mp;
    [System.NonSerialized] public ModifiableStat MaxMp;

    [System.NonSerialized] public ModifiableStat Atk_P;
    [System.NonSerialized] public ModifiableStat Atk_M;
    [System.NonSerialized] public ModifiableStat Def_P;
    [System.NonSerialized] public ModifiableStat Def_M;
    [System.NonSerialized] public ModifiableStat Speed;
    [Header("Playable Character-Specific Field")]
    [SerializeField] private Sprite menuPortraitSprite;
    public Sprite MenuPortraitSprite => menuPortraitSprite;
    [System.NonSerialized] public Utils.RangedInt Xp;

    private void Awake()
    {
        if (baseStats == null)
        {
            Debug.LogWarning("No baseStats assigned for CombatEntity \"" + gameObject.name + "\"");
            return;
        }

        StatValues statsToLoad = baseStats.leveledStats[0];

        //TODO: load data from storage
        // if ([local_save_data_exists])
        // {
        //     level = [code_here_lol];
        //     statsToLoad = baseStats.GetStatsForLevel(level);

        //     hp = [code_here_lol];
        //     mp = [code_here_lol];
        //     Xp = [code_here_lol];
        // }
        // else
        // {
        //No existing data for this entity (only playable characters have data saved)

        level = statsToLoad.level;
        hp = statsToLoad.maxHp;
        mp = statsToLoad.maxMp;
        Xp = new Utils.RangedInt(0, statsToLoad.maxXp);
        // }

        //Load from baseStats
        MaxHp = new ModifiableStat(statsToLoad.maxHp);
        MaxMp = new ModifiableStat(statsToLoad.maxMp);
        Atk_P = new ModifiableStat(statsToLoad.atk_P);
        Atk_M = new ModifiableStat(statsToLoad.atk_M);
        Def_P = new ModifiableStat(statsToLoad.def_P);
        Def_M = new ModifiableStat(statsToLoad.def_M);
        Speed = new ModifiableStat(statsToLoad.speed);

        SetHealthBars = SetHealthUI;
        SetManaBars = SetManaUI;

        if (hpBar != null)
            SetHealthUI(hp, MaxHp.Value);
        if (mpBar != null)
            SetManaUI(mp, MaxMp.Value);
    }

    public event Action<int, int> SetHealthBars;
    private void SetHealthUI(int _hp, int _maxHp)
    {
        if (hpBar != null)
            hpBar.SetUIValues(hp, MaxHp.Value);
    }

    public event Action<int, int> SetManaBars;
    private void SetManaUI(int _mp, int _maxMp)
    {
        if (mpBar != null)
            mpBar.SetUIValues(_mp, _maxMp);
    }

    public void TakeDamage(int _damage)
    {
        hp = Mathf.Clamp(hp - _damage, 0, MaxHp.Value);

        SetHealthBars?.Invoke(hp, MaxHp.Value);

        if (hp == 0)
            Die();
    }

    public void UseMana(int _manaToUse)
    {
        mp = Mathf.Clamp(mp - _manaToUse, 0, MaxMp.Value);

        SetManaBars?.Invoke(mp, MaxMp.Value);
    }

    public void DEBUG_TAKE_DAMAGE()
    {
        TakeDamage(1);
    }

    private void Die()
    {
        Debug.Log("Oh no! " + gameObject.name + " Died...");
    }

    public ModifiableStat GetStatBlockOfType(StatType_Enum _statType)
    {
        switch (_statType)
        {
            case StatType_Enum.MaxHp:
                return MaxHp;
            case StatType_Enum.MaxMp:
                return MaxMp;
            case StatType_Enum.Atk_P:
                return Atk_P;
            case StatType_Enum.Atk_M:
                return Atk_M;
            case StatType_Enum.Def_P:
                return Def_P;
            case StatType_Enum.Def_M:
                return Def_M;
            case StatType_Enum.Speed:
                return Speed;
        }

        Debug.LogError("Cannot find StatBlock for StatType \"" + _statType.ToString() + "\" on entity \"" + gameObject.name + "\"");
        return null;
    }
}


#region StatBlock Stuff

[System.Serializable]
/// <summary>
/// Represents a stat that can be modified with buffs and debufs (for example spells or equiptment)
/// </summary>
public class ModifiableStat
{
    [SerializeField] private int baseValue;
    public void SetBaseValue(int _baseValue)
    {
        if (_baseValue != baseValue)
        {
            //Need to recalculate
            baseValue = _baseValue;
            isDirty = true;
        }
    }
    public void AddToBaseValue(int _amountToAdd)
    {
        if (_amountToAdd != 0)
        {
            //Need to recalculate
            baseValue += _amountToAdd;
            isDirty = true;
        }
    }
    private bool isDirty = true;
    private int _value;
    public int Value
    {
        get
        {
            if (isDirty)
            {
                _value = CalculateFinalValue();
                isDirty = false;
            }
            return _value;
        }
    }

    private readonly List<StatModifier> statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;

    public ModifiableStat(int _baseValue)
    {
        baseValue = _baseValue;
        statModifiers = new List<StatModifier>();
        StatModifiers = statModifiers.AsReadOnly();
    }


    public void AddModifier(StatModifier mod)
    {
        isDirty = true;
        statModifiers.Add(mod);
        statModifiers.Sort(CompareStatMods);
    }

    public bool RemoveModifier(StatModifier mod)
    {
        if (statModifiers.Remove(mod))
        {
            isDirty = true;
            return true;
        }

        return false;
    }

    public bool RemoveModifiersFromSource(object _source)
    {
        bool removed = false;

        for (int i = statModifiers.Count; i >= 0; i--)
        {
            if (statModifiers[i].Source == _source)
            {
                statModifiers.RemoveAt(i);

                isDirty = true;
                removed = true;
            }
        }

        return removed;
    }

    private int CompareStatMods(StatModifier a, StatModifier b)
    {
        return a.CalculationOrder.CompareTo(b.CalculationOrder);
    }

    private int CalculateFinalValue()
    {
        float finalValue = baseValue;
        float sumPercentAdd = 0;
        float sumPercentMult = 0;

        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier currMod = statModifiers[i];

            switch (currMod.Type)
            {
                case StatModifier.StatModEnum.Flat:
                    finalValue += currMod.Value;
                    break;
                case StatModifier.StatModEnum.Percent_Additive:
                    sumPercentAdd += currMod.Value;
                    break;
                case StatModifier.StatModEnum.Percent_Multiplicative:
                    sumPercentMult += currMod.Value;
                    break;
            }
        }

        finalValue *= 1 + sumPercentAdd;
        finalValue *= 1 + sumPercentMult;

        return Mathf.RoundToInt(finalValue);
    }
}

public class StatModifier
{
    public enum StatModEnum
    {
        Flat = 100,
        Percent_Additive = 200,
        Percent_Multiplicative = 300
    }

    public readonly float Value;
    public readonly StatModEnum Type;
    public readonly int CalculationOrder;
    public readonly object Source;

    public StatModifier(float _value, StatModEnum _type, object _source = null)
    {
        Value = _value;
        Type = _type;
        CalculationOrder = (int)Type;

        Source = _source;
    }
}
#endregion