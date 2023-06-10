using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;
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

    [field: SerializeField] public bool IsPlayer { get; private set; }
    [field: SerializeField] public BaseCombatEntity BaseStats { get; private set; }
    [field: SerializeField] public AttackSpell BasicAttack { get; private set; }
    [field: SerializeField] public List<AttackSpell> Spells { get; private set; }
    [Space(10)]
    [SerializeField] private Menubar hpBar;
    [SerializeField] private Menubar mpBar;

    public int Level { get; private set; }

    public bool IsDead { get; private set; }

    public int Hp { get; private set; }
    [System.NonSerialized] public ModifiableStat MaxHp;

    public int Mp { get; private set; }
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

    [field: SerializeField, Header("Enemy-Specific Field")] public Selectable EnemyButton { get; private set; }

    private void Awake()
    {
        if (BaseStats == null)
        {
            Debug.LogWarning("No baseStats assigned for CombatEntity \"" + gameObject.name + "\"");
            return;
        }

        StatValues statsToLoad = BaseStats.LeveledStats[0];

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

        Level = statsToLoad.level;
        Hp = statsToLoad.maxHp;
        Mp = statsToLoad.maxMp;
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

        SetHealthBars?.Invoke(Hp, MaxHp.Value);
        SetManaBars?.Invoke(Mp, MaxMp.Value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_baseStatIndex"></param>
    /// <returns> Is entity max level </returns>
    private bool LevelUpToIndex(int _baseStatIndex)
    {
        if (_baseStatIndex < BaseStats.LeveledStats.Count)
        {
            //Valid level
            StatValues newStats = BaseStats.LeveledStats[_baseStatIndex];

            Hp += newStats.maxHp - MaxHp.BaseValue;
            Mp += newStats.maxMp - MaxMp.BaseValue;
            Xp.SetMaxValue(newStats.maxXp);
            Debug.Log("Max Xp: " + Xp.MaxValue);

            MaxHp.SetBaseValue(newStats.maxHp);
            MaxMp.SetBaseValue(newStats.maxMp);
            Atk_P.SetBaseValue(newStats.atk_P);
            Atk_M.SetBaseValue(newStats.atk_M);
            Def_P.SetBaseValue(newStats.def_P);
            Def_M.SetBaseValue(newStats.def_M);
            Speed.SetBaseValue(newStats.speed);

            SetHealthBars?.Invoke(Hp, MaxHp.Value);
            SetManaBars?.Invoke(Mp, MaxMp.Value);

            return _baseStatIndex == BaseStats.LeveledStats.Count - 1;
        }
        else
        {
            //Invalid level
            Debug.LogError(BaseStats.name + " does not contain leveledStats with index " + _baseStatIndex);
            return true;
        }
    }

    public event Action<int, int> SetHealthBars;
    private void SetHealthUI(int _hp, int _maxHp)
    {
        if (hpBar != null)
            hpBar.SetUIValues(Hp, MaxHp.Value);
    }

    public event Action<int, int> SetManaBars;
    private void SetManaUI(int _mp, int _maxMp)
    {
        if (mpBar != null)
            mpBar.SetUIValues(_mp, _maxMp);
    }

    public event Action<Utils.RangedInt, int> SetXpBarAndLevelNum;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_atkValue"></param>
    /// <param name="_isPhysical"></param>
    /// <returns>Amount of damage taken (not stopped by hitting 0 hp)</returns>
    public int TakeDamage(float _atkValue, bool _isPhysical)
    {
        if (_atkValue < 0)
        {
            Debug.LogError("Damage should not be negative, use Heal() instead.\nReturning.");
            return 0;
        }

        int defValue;
        if (_isPhysical)
            defValue = Def_P.Value;
        else
            defValue = Def_M.Value;

        float damageToTake;
        if (_atkValue >= defValue)
            damageToTake = _atkValue * 2 - defValue;
        else
            damageToTake = _atkValue * _atkValue - defValue;

        //Randomly have attack go from 90% to 110% damage
        float randomMultiplier = UnityEngine.Random.Range(0.9f, 1.1f);
        damageToTake *= randomMultiplier;

        int damageToTake_int = Mathf.Max(1, Mathf.RoundToInt(damageToTake));

        Hp = Mathf.Clamp(Hp - damageToTake_int, 0, MaxHp.Value);

        SetHealthBars?.Invoke(Hp, MaxHp.Value);

        if (Hp == 0)
            Die();

        return damageToTake_int;
    }

    public void Heal(int _healAmount)
    {
        Hp = Mathf.Clamp(Hp + _healAmount, 0, MaxHp.Value);

        SetHealthBars?.Invoke(Hp, MaxHp.Value);
    }

    public void UseMana(int _manaToUse)
    {
        Mp = Mathf.Clamp(Mp - _manaToUse, 0, MaxMp.Value);

        SetManaBars?.Invoke(Mp, MaxMp.Value);
    }

    private void Die()
    {
        IsDead = true;
        Debug.Log("Oh no! " + gameObject.name + " Died...");
    }

    public void GainXP(int _xpToGain)
    {
        if (Level == BaseStats.LeveledStats[^1].level)
        {
            //Already max level
            return;
        }

        Xp.AddToValue(_xpToGain);
        while (Xp.Value >= Xp.MaxValue)
        {
            //Level up!
            Level++;

            Debug.Log(gameObject.name + " leveled up to level " + Level + "!");

            Xp.AddToValue(-Xp.MaxValue);
            if (LevelUpToIndex(Level - 1))
            {
                //Max level
                Xp.SetValue(Xp.MaxValue);
                break;
            }
        }

        SetXpBarAndLevelNum?.Invoke(Xp, Level);
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
    public int BaseValue => baseValue;
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