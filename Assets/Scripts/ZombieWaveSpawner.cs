using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ZombieWaveSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject[] zombiePrefabs;      // drag Zombie + ZombieSpitter prefabs here
    public TMP_Text killCounterText;        // optional

    [Header("Rules")]
    public int zombiesPerWave = 5;
    public int killsToEscape = 30;
    public bool fivePerSide = false;        // true = 5 on the left AND 5 on the right each wave
    public float spawnDistance = 10f;       // how far left/right of the player
    public float spawnYOffset = 0f;         // adjust so zombies land on the floor
    public float delayBetweenSpawns = 0.3f;
    public float delayBetweenWaves = 1.5f;

    [Header("When quota is reached")]
    public GameObject exitToEnable;         // e.g. your Exit object (optional)
    public UnityEvent onQuotaReached;       // e.g. load next scene

    int kills, spawned, aliveInWave;

    void Start()
    {
        UpdateUI();
        if (exitToEnable != null) exitToEnable.SetActive(false);
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        while (spawned < killsToEscape)
        {
            int toSpawn = fivePerSide ? zombiesPerWave * 2 : zombiesPerWave;
            toSpawn = Mathf.Min(toSpawn, killsToEscape - spawned);   // never over-spawn

            for (int i = 0; i < toSpawn; i++)
            {
                int side;
                if (fivePerSide) side = (i % 2 == 0) ? -1 : 1;        // alternate L/R
                else side = Random.value < 0.5f ? -1 : 1;             // random side
                SpawnZombie(side);
                yield return new WaitForSeconds(delayBetweenSpawns);
            }

            // wait until the whole wave is dead
            while (aliveInWave > 0) yield return null;
            yield return new WaitForSeconds(delayBetweenWaves);
        }
    }

    void SpawnZombie(int side)
    {
        var prefab = zombiePrefabs[Random.Range(0, zombiePrefabs.Length)];
        Vector3 pos = new Vector3(player.position.x + side * spawnDistance,
                                  player.position.y + spawnYOffset,
                                  player.position.z);

        GameObject z = Instantiate(prefab, pos, Quaternion.identity);
        AssignPlayer(z);

        var reporter = z.AddComponent<ZombieDeathReporter>();
        reporter.onDied = OnZombieDied;

        spawned++;
        aliveInWave++;
    }

    void OnZombieDied()
    {
        aliveInWave--;
        kills++;
        UpdateUI();

        if (kills >= killsToEscape)
        {
            StopAllCoroutines();   // no more spawning
            if (exitToEnable != null) exitToEnable.SetActive(true);
            onQuotaReached?.Invoke();
        }
    }

    void UpdateUI()
    {
        if (killCounterText != null)
            killCounterText.text = $"Zombies Killed: {kills} / {killsToEscape}";
    }

    // Sets the "player" Transform field on ZombieAi / ZombieSpitAi at spawn time
    void AssignPlayer(GameObject z)
    {
        foreach (var mb in z.GetComponents<MonoBehaviour>())
        {
            var f = mb.GetType().GetField("player",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (f != null && f.FieldType == typeof(Transform))
                f.SetValue(mb, player);
        }
    }
}

// Reports when a spawned zombie gets destroyed (killed)
public class ZombieDeathReporter : MonoBehaviour
{
    public System.Action onDied;
    bool quitting;

    void OnApplicationQuit() { quitting = true; }

    void OnDestroy()
    {
        // ignore scene changes / stopping play mode
        if (quitting || !gameObject.scene.isLoaded) return;
        onDied?.Invoke();
    }
}