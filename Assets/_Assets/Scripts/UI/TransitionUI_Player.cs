using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionUI_Player : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private SpriteRenderer sprite;

    // Update is called once per frame
    void Update()
    {
        image.sprite = sprite.sprite;
    }
}
