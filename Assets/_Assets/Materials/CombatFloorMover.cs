using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        StartCoroutine(RampUpGridSpeed());
    }

    private IEnumerator RampUpGridSpeed()
    {
        float startTime = Time.time;
        float elapsedPercentage = 0;

        while (elapsedPercentage < 1)
        {
            elapsedPercentage = Mathf.Min(1, (Time.time - startTime) / startupTime);

            move_1 = grid1Speed * elapsedPercentage;
            move_2 = grid2Speed * elapsedPercentage;

            yield return null;
        }

        move_1 = grid1Speed;
        move_2 = grid2Speed;
    }
}
