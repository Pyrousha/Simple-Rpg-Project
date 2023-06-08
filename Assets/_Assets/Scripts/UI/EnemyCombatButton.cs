using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyCombatButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Selectable targetButton;
    [SerializeField] private GameObject selectedArrow;
    [SerializeField] private CombatEntity enemyEntity;

    private void Awake()
    {
        targetButton = GetComponent<Selectable>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        targetButton.interactable = true;
        selectedArrow.SetActive(true);
    }

    public void OnDeselect(BaseEventData data)
    {
        targetButton.interactable = false;
        selectedArrow.SetActive(false);
    }

    public void OnClicked()
    {
        CombatController.Instance.UseMoveOnEnemy(enemyEntity);
        OnDeselect(null);
    }
}