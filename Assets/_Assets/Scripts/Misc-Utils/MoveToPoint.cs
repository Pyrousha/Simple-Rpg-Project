using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeauRoutine;

public class MoveToPoint : MonoBehaviour
{
    public void MoveToPosition(Vector3 _startPos, Vector3 _targetPos, float _duration)
    {
        transform.position = _startPos;
        transform.forward = (_targetPos - _startPos).normalized;

        //TODO: Fix this (ewwwwwwwwww)
        transform.GetChild(0).gameObject.SetActive(true);

        Routine.Start(this, MoveRoutine(_targetPos, _duration, Curve.QuadIn));
    }

    private IEnumerator MoveRoutine(Vector3 _targetPos, float _duration, Curve _curve)
    {
        yield return transform.MoveTo(_targetPos, _duration).Ease(_curve);

        Destroy(gameObject);
    }
}
