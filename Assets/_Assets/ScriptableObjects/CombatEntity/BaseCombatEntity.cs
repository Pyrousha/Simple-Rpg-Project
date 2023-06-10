using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using static Utils;

[CreateAssetMenu(menuName = "Combat/BaseCombatEntity")]
public class BaseCombatEntity : ScriptableObject
{
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
    [System.Serializable]
    public struct EnemyDrops
    {
        public int XP;
        public int GP;
        //TODO: list of items to drop
    }


    [field: SerializeField] public List<StatValues> LeveledStats { get; private set; }
    [field: SerializeField] public Sprite Sprite_Up { get; private set; }
    [field: SerializeField] public Sprite Sprite_Down { get; private set; }
    [field: SerializeField] public Sprite Sprite_Left { get; private set; }
    [field: SerializeField] public Sprite Sprite_Right { get; private set; }

    [field: SerializeField, Header("Enemy-Specific")] public EnemyDrops Drops { get; private set; }


#if UNITY_EDITOR
    private void OnValidate()
    {
        LeveledStats.Sort((a, b) => a.level.CompareTo(b.level));
    }

    public StatValues GetStatsForLevel(int _level)
    {
        foreach (StatValues stats in LeveledStats)
        {
            if (stats.level == _level)
                return stats;
        }

        Debug.LogError("CombatEntity \"" + name + "\" has no stats for level " + _level);
        return new StatValues();
    }
#endif
}