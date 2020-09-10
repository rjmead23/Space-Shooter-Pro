using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private float _enemyWaitTime = 5.0f;
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] powerups;
    [SerializeField]
    private GameObject[] _powerupAmmo;
    [SerializeField]
    private float _ammoPowerupFrequency = 0.6f;


    private bool _stopSpawning = false;


    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false)
        {
            // Spawn enemies at random locations
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 8, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_enemyWaitTime);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        while (_stopSpawning == false)
        {
            float randomPowerupSelection = Random.Range(0f, 1f);
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 8, 0);

            if (randomPowerupSelection <= _ammoPowerupFrequency)
            {
                Instantiate(_powerupAmmo[0], posToSpawn, Quaternion.identity);
                yield return new WaitForSeconds(Random.Range(3, 8));
            }
            else
            {
                int randomPowerup = Random.Range(0, powerups.Length);
                Instantiate(powerups[randomPowerup], posToSpawn, Quaternion.identity);
                yield return new WaitForSeconds(Random.Range(3, 8));
            }

        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

}
