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
        if (PauseController.Instance.IsPaused)
            return;

        if (beenInCombat == false && triggerCombatOnTouchPlayer)
        {
            OnTouchedPlayer();
            beenInCombat = true;
        }
    }

    public void OnTouchedPlayer()
    {
        int numEnemiesToFight = Random.Range(1, enemiesToFight.Count + 1);

        while (enemiesToFight.Count > numEnemiesToFight)
            enemiesToFight.RemoveAt(enemiesToFight.Count - 1);

        CombatTransitionController.Instance.TriggerCombat(enemyTransform, enemiesToFight);
    }
}
