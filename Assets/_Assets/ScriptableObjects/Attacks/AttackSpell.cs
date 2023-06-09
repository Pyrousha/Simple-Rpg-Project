using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/AttackSpell")]
public class AttackSpell : ScriptableObject
{
    [field: SerializeField] public bool IsPhysical { get; private set; } = true;
    [field: SerializeField] public float AttackScalingMultiplier { get; private set; } = 1f;
}
