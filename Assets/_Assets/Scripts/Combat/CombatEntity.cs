using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class CombatEntity : MonoBehaviour
{
    [SerializeField] private BaseCombatEntity baseStats;

    private int level;
    public int Level => level;

    private int hp;
    public int Hp => hp;
    [System.NonSerialized] public StatBlock maxHp;

    private int mp;
    public int Mp => mp;
    [System.NonSerialized] public StatBlock maxMp;

    [System.NonSerialized] public StatBlock Atk_P;
    [System.NonSerialized] public StatBlock Atk_M;
    [System.NonSerialized] public StatBlock Def_P;
    [System.NonSerialized] public StatBlock Def_M;
    [System.NonSerialized] public StatBlock Speed;
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

        //Load from baseStats
        maxHp = new StatBlock(baseStats.MaxHp);
        maxMp = new StatBlock(baseStats.MaxMp);
        Atk_P = new StatBlock(baseStats.Atk_P);
        Atk_M = new StatBlock(baseStats.Atk_M);
        Def_P = new StatBlock(baseStats.Def_P);
        Def_M = new StatBlock(baseStats.Def_M);
        Speed = new StatBlock(baseStats.Speed);

        //TEMP: don't load any persistent stats
        level = baseStats.Level;
        hp = maxHp.Value;
        mp = maxMp.Value;
        Xp = new Utils.RangedInt(0, baseStats.MaxXp);
    }
}


#region StatBlock Stuff

[System.Serializable]
public class StatBlock
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

    public StatBlock(int _baseValue)
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