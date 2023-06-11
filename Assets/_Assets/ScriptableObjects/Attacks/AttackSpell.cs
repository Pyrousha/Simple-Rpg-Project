using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/AttackSpell")]
public class AttackSpell : ScriptableObject
{
    [field: SerializeField] public bool IsPhysical { get; private set; } = true;
    [field: SerializeField] public bool HitsMultipleEnemies { get; private set; } = false;
    [field: SerializeField] public int NumAttacks { get; private set; } = 1;
    [field: SerializeField] public float AttackScalingMultiplier { get; private set; } = 1f;
    [field: SerializeField] public int ManaCost { get; private set; } = 0;
}
