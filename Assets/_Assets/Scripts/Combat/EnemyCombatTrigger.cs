using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatTrigger : MonoBehaviour
{
    [SerializeField] private Transform enemyTransform;

    private void OnTriggerEnter(Collider other)
    {
        CombatController.Instance.TriggerCombat(enemyTransform);
    }
}
