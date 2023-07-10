using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeauRoutine;

public class CombatFloorMover : MonoBehaviour
{
    public GameObject bg_1;
    public GameObject bg_2;

    [SerializeField] private Vector3 grid1Speed;
    [SerializeField] private Vector3 grid2Speed;

    private Vector3 move_1;
    private Vector3 move_2;

    [SerializeField] private float startupTime;

    // Update is called once per frame
    void Update()
    {
        bg_1.transform.position += move_1 * Time.deltaTime;
        if (bg_1.transform.localPosition.x > 1)
            bg_1.transform.localPosition += new Vector3(-1, 0, 0);
        if (bg_1.transform.localPosition.x < 1)
            bg_1.transform.localPosition += new Vector3(1, 0, 0);
        if (bg_1.transform.localPosition.y > 1)
            bg_1.transform.localPosition += new Vector3(0, -1, 0);
        if (bg_1.transform.localPosition.y < 1)
            bg_1.transform.localPosition += new Vector3(0, 1, 0);

        bg_2.transform.position += move_2 * Time.deltaTime;
        if (bg_2.transform.localPosition.x > 1)
            bg_2.transform.localPosition += new Vector3(-1, 0, 0);
        if (bg_2.transform.localPosition.x < 1)
            bg_2.transform.localPosition += new Vector3(1, 0, 0);
        if (bg_2.transform.localPosition.y > 1)
            bg_2.transform.localPosition += new Vector3(0, -1, 0);
        if (bg_2.transform.localPosition.y < 1)
            bg_2.transform.localPosition += new Vector3(0, 1, 0);
    }

    public void StartGridMove()
    {
        Routine.Start(this, RampUpGridSpeed());
    }

    private IEnumerator RampUpGridSpeed()
    {
        yield return Tween.Float(0, 1, (elapsedPercentage) =>
        {
            move_1 = grid1Speed * elapsedPercentage;
            move_2 = grid2Speed * elapsedPercentage;
        }, startupTime);
    }
}
