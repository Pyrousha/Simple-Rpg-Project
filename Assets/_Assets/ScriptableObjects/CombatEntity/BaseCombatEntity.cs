using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using static Utils;

[CreateAssetMenu(menuName = "Combat/BaseCombatEntity")]
public class BaseCombatEntity : ScriptableObject
{
    [System.Serializable]
    public enum StatEnum
    {
        Health,
        Atk_P,
        Atk_M,
        Def_P,
        Def_M,
        Speed,
        MaxXp
    }

    [SerializeField] private int level;
    public int Level => level;

    [SerializeField] private int maxHp;
    public int MaxHp => maxHp;
    [SerializeField] private int maxMp;
    public int MaxMp => maxMp;

    [SerializeField] private int atk_P;
    public int Atk_P => atk_P;
    [SerializeField] private int atk_M;
    public int Atk_M => atk_M;
    [SerializeField] private int def_P;
    public int Def_P => def_P;
    [SerializeField] private int def_M;
    public int Def_M => def_M;
    [SerializeField] private int speed;
    public int Speed => speed;

    [Header("Playable Character-Specific Field")]
    [SerializeField] private int maxXp = -999;
    public int MaxXp => maxXp;
    public List<List<UpgradeBlock>> upgrades;
}

[System.Serializable]
public struct UpgradeBlock
{
    public BaseCombatEntity.StatEnum statType;
    public int amountToAdd;
}