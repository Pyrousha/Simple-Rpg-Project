using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using BeauRoutine;

public class DamageNumber : MonoBehaviour
{
    public enum DamageNumberEnum
    {
        Damage,
        Healing,
        Mana
    }

    [SerializeField] private Animator anim;
    [SerializeField] private Image bgImage;
    [SerializeField] private TextMeshProUGUI textNumber;

    [SerializeField] private Color damageColor;
    [SerializeField] private Color healingColor;
    [SerializeField] private Color manaColor;

    public void SetVisuals(DamageNumberEnum _damageType, int _number, float _duration)
    {
        switch (_damageType)
        {
            case DamageNumberEnum.Damage:
                bgImage.color = damageColor;
                break;
            case DamageNumberEnum.Healing:
                bgImage.color = healingColor;
                break;
            case DamageNumberEnum.Mana:
                bgImage.color = manaColor;
                break;
        }

        textNumber.text = _number.ToString();

        anim.enabled = true;
        anim.speed = 1.0f / _duration;

        Routine.Start(this, DestroyAfterSeconds(_duration));
    }

    private IEnumerator DestroyAfterSeconds(float _timeToWait)
    {
        yield return _timeToWait;

        Destroy(gameObject);
    }
}
