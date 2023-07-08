using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PopupIndicator : Singleton<PopupIndicator>
{
    private Animator anim;

    [SerializeField] private TextMeshProUGUI textSpacer;
    [SerializeField] private TextMeshProUGUI realText;

    [SerializeField] private Transform canvasTransform;

    private Vector3 startingPos;
    private bool active = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!active)
            return;

        canvasTransform.position = startingPos + new Vector3(0, 0.25f / 6, 0) * Mathf.Sin(Time.time * 2.5f);
    }

    public void ShowPopup(Transform _transform, String _textToShow)
    {
        startingPos = _transform.position + new Vector3(0, 1, 0);
        canvasTransform.position = startingPos;

        textSpacer.text = _textToShow;
        realText.text = _textToShow;

        anim.SetBool("Status", true);
        active = true;
    }

    public void ClosePopup()
    {
        anim.SetBool("Status", false);
        active = false;
    }
}
