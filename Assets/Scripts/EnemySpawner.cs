using System.Collections;
using UnityEngine;

[System.Serializable]
public class SpawnEntry
{
    public GameObject prefab;
    public float delay;
    public int lane;
    public int waveNumber = 1;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform[] lanes;
    [SerializeField] private SpawnEntry[] wave;

    public bool allSpawned;
    public int spawned;
    public int currentWave = 1;

    public int Total { get { return wave.Length; } }

    public int TotalWaves
    {
        get
        {
            int max = 1;
            foreach (SpawnEntry e in wave) if (e.waveNumber > max) max = e.waveNumber;
            return max;
        }
    }

    IEnumerator Start()
    {
        Enemy.AliveCount = 0;
        Enemy.All.Clear();

        foreach (SpawnEntry entry in wave)
        {
            yield return new WaitForSeconds(entry.delay);
            Transform lane = lanes[entry.lane];
            Instantiate(entry.prefab, lane.position, Quaternion.identity);
            spawned++;
            currentWave = entry.waveNumber;
        }
        allSpawned = true;
    }
}