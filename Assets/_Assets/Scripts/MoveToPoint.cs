using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPoint : MonoBehaviour
{
    public void MoveToPosition(Vector3 _startPos, Vector3 _targetPos, float _duration)
    {
        transform.position = _startPos;
        transform.forward = (_targetPos - _startPos).normalized;

        transform.GetChild(0).gameObject.SetActive(true);

        StartCoroutine(LerpToPosition(_startPos, _targetPos, _duration));
    }

    private IEnumerator LerpToPosition(Vector3 _startPos, Vector3 _targetPos, float _duration)
    {
        float startTime = Time.time;
        float elapsedPercentage = 0;

        while (elapsedPercentage < 1)
        {
            elapsedPercentage = Mathf.Min(1, (Time.time - startTime) / _duration);
            elapsedPercentage = Accelerate(elapsedPercentage);

            transform.position = Vector3.Lerp(_startPos, _targetPos, elapsedPercentage);

            yield return null;
        }

        transform.position = _targetPos;

        Destroy(gameObject);
    }

    private float Parametric(float _x)
    {
        return _x * _x / (2 * (_x * _x - _x) + 1);
    }

    private float Accelerate(float _x)
    {
        return _x * _x;
    }
}
