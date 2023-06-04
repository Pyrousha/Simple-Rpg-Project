using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using static Utils;

[CreateAssetMenu(menuName = "Combat/BaseCombatEntity")]
public class BaseCombatEntity : ScriptableObject
{
    public List<StatValues> leveledStats;

    [System.Serializable]
    public struct StatValues
    {
        public int level;
        // [Space(10)]
        public int maxHp;
        public int maxMp;

        public int atk_P;
        public int atk_M;
        public int def_P;
        public int def_M;

        public int speed;

        public int maxXp;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        leveledStats.Sort((a, b) => a.level.CompareTo(b.level));
    }

    public StatValues GetStatsForLevel(int _level)
    {
        foreach (StatValues stats in leveledStats)
        {
            if (stats.level == _level)
                return stats;
        }

        Debug.LogError("CombatEntity \"" + name + "\" has no stats for level " + _level);
        return new StatValues();
    }
#endif
}