using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSort : MonoBehaviour
{
    SpriteRenderer sprRend;
    [SerializeField] private bool SortOnUpdate;

    private void Awake()
    {
        sprRend = GetComponent<SpriteRenderer>();
        sprRend.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10);
    }

    private void FixedUpdate()
    {
        if (SortOnUpdate)
            sprRend.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10);
    }
}
