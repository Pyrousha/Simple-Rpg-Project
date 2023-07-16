using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform enemyParent;
    [SerializeField] private int numMaxEnemies;
    private List<EnemyController_Overworld> spawnedEnemies = new List<EnemyController_Overworld>();
    [SerializeField] private float distFromPlayer_Min;
    [SerializeField] private float distFromPlayer_Max;
    [SerializeField] private float spawnInterval_Min;
    [SerializeField] private float spawnInterval_Max;
    [SerializeField] private bool spawnEnemyOnAwake;

    private float nextSpawnTime;
    float timeLeft;

    void Awake()
    {
        SetInstance(this);

        if (spawnEnemyOnAwake)
            TrySpawnEnemy();
        else
            ResetSpawnTimer();

        PauseController.Instance.OnPausedStateChanged_Event += OnPausedChange;
    }

    void OnPausedChange(bool _newIsPaused)
    {
        if (_newIsPaused)
            timeLeft = nextSpawnTime - Time.time;
        else
            nextSpawnTime = Time.time + timeLeft;
    }

    void OnDestroy()
    {
        PauseController.Instance.OnPausedStateChanged_Event -= OnPausedChange;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseController.Instance.IsPaused)
            return;

        if (Time.time >= nextSpawnTime)
            TrySpawnEnemy();
    }

    private void TrySpawnEnemy()
    {
        if (spawnedEnemies.Count < numMaxEnemies)
        {
            float dir = Random.Range(0, 2 * Mathf.PI);
            Vector3 spawnPos = new Vector3(Mathf.Cos(dir), Mathf.Sin(dir), 0);
            spawnPos *= Random.Range(distFromPlayer_Min, distFromPlayer_Max);

            spawnPos += PartyManager.Instance.GetFirstAlivePlayer().transform.position;

            GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            newEnemy.transform.SetParent(enemyParent);
            spawnedEnemies.Add(newEnemy.GetComponent<EnemyController_Overworld>());

            Debug.Log("Spawned enemy!");
        }

        ResetSpawnTimer();
    }

    public void DestroyAllEnemies()
    {
        while (spawnedEnemies.Count > 0)
        {
            Destroy(spawnedEnemies[spawnedEnemies.Count - 1].gameObject);
            spawnedEnemies.RemoveAt(spawnedEnemies.Count - 1);
        }

        ResetSpawnTimer();
    }

    private void ResetSpawnTimer()
    {
        nextSpawnTime = Time.time + Random.Range(spawnInterval_Min, spawnInterval_Max);
    }
}
