using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignWithCamera : MonoBehaviour
{
    public enum FaceCameraMode
    {
        MatchForwardDirection,
        MatchYRotation,
        Nothing
    }

    private Transform cameraTransform;
    [field: SerializeField] public FaceCameraMode Mode { get; private set; } = FaceCameraMode.MatchYRotation;

    private void Awake()
    {
        cameraTransform = CombatTransitionController.Instance.CombatCamera;
    }

    // Update is called once per frame
    void Update()
    {
        switch (Mode)
        {
            case FaceCameraMode.MatchForwardDirection:
                transform.rotation = Quaternion.LookRotation(cameraTransform.forward, cameraTransform.up);
                break;
            case FaceCameraMode.MatchYRotation:
                Vector3 newEuler = transform.eulerAngles;
                newEuler.y = cameraTransform.eulerAngles.y;
                transform.eulerAngles = newEuler;
                break;
            case FaceCameraMode.Nothing:
                break;
        }
    }

    public void StandUp()
    {
        Mode = FaceCameraMode.MatchForwardDirection;
    }

    public void StartFallOver()
    {
        StartCoroutine(FallOver(0.5f));
    }

    private IEnumerator FallOver(float _lerpDuration)
    {
        Mode = FaceCameraMode.MatchYRotation;

        float startXRotation = transform.localEulerAngles.x;

        float targetXRotation = 90.0f;

        float startTime = Time.time;
        float elapsedPercentage = 0;

        while (elapsedPercentage < 1)
        {
            elapsedPercentage = Mathf.Min(1, (Time.time - startTime) / _lerpDuration);
            elapsedPercentage = Utils.Accelerate(elapsedPercentage);

            transform.localEulerAngles = new Vector3(
                Mathf.Lerp(startXRotation, targetXRotation, elapsedPercentage), transform.localEulerAngles.y, transform.localEulerAngles.z);

            yield return null;
        }
    }
}
