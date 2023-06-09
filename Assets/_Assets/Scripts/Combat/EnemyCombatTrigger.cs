using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatTrigger : MonoBehaviour
{
    private bool beenInCombat = false;

    [SerializeField] private bool triggerCombatOnTouchPlayer = true;
    [SerializeField] private Transform enemyTransform;
    [SerializeField] private List<OverworldEntity> enemiesToFight;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (beenInCombat == false && triggerCombatOnTouchPlayer)
        {
            OnTouchedPlayer();
            beenInCombat = true;
        }
    }

    public void OnTouchedPlayer()
    {
        CombatTransitionController.Instance.TriggerCombat(enemyTransform, enemiesToFight);
    }
}
