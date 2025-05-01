using UnityEngine;

using System.Collections;
using UnityEngine;

public class SpawnManager1 : MonoBehaviour
{
    public Transform[] spawnPoints;  
    public GameObject enemyPrefab;
    public GameObject powerUpPrefab;

    [System.Serializable]
    public class Wave
    {
        public int totalSpawnEnemies;  
        public int numberOfRandomSpawnPoint;  
        public float delayStart;  
        public float spawnInterval;  
        public int numberOfPowerUp;  
    }

    public Wave[] waves;  

    private void Start()
    {
        StartCoroutine(SpawnWaveRoutine());
    }

    IEnumerator SpawnWaveRoutine()
    {
        for (int waveIndex = 0; waveIndex < waves.Length; waveIndex++)
        {
            Wave currentWave = waves[waveIndex];  

            yield return new WaitForSeconds(currentWave.delayStart);

            Transform[] randomSpawnPoints = GetRandomSpawnPoints(currentWave.numberOfRandomSpawnPoint);

            for (int i = 0; i < currentWave.totalSpawnEnemies; i++)
            {
                SpawnEnemy(randomSpawnPoints);
                yield return new WaitForSeconds(currentWave.spawnInterval);  
            }

            SpawnPowerUps(currentWave.numberOfPowerUp);

            Debug.Log($"Wave {waveIndex + 1} completed.");
        }
    }

    Transform[] GetRandomSpawnPoints(int numberOfRandomSpawnPoint)
    {
        Transform[] randomSpawnPoints = new Transform[numberOfRandomSpawnPoint];
        System.Collections.Generic.List<Transform> spawnList = new System.Collections.Generic.List<Transform>(spawnPoints);

        for (int i = 0; i < numberOfRandomSpawnPoint; i++)
        {
            int randomIndex = Random.Range(0, spawnList.Count);
            randomSpawnPoints[i] = spawnList[randomIndex];
            spawnList.RemoveAt(randomIndex); 
        }

        return randomSpawnPoints;
    }

    void SpawnEnemy(Transform[] randomSpawnPoints)
    {
        int randomIndex = Random.Range(0, randomSpawnPoints.Length);
        Instantiate(enemyPrefab, randomSpawnPoints[randomIndex].position, Quaternion.identity);
    }

    void SpawnPowerUps(int numberOfPowerUp)
    {
        for (int i = 0; i < numberOfPowerUp; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Instantiate(powerUpPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
            Debug.Log("PowerUp spawned at: " + spawnPoints[randomIndex].position);
        }
    }
}
