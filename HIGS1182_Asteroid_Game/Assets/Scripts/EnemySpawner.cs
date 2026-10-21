using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    [Header("Placement")]
    [Tooltip("Who to place enemies around. Leave empty to use this object's position.")]
    [SerializeField] private Transform player;
    [Tooltip("Enemies appear between these distances from the player, so none start on top of them.")]
    [SerializeField] private float minDistance = 90f;
    [SerializeField] private float maxDistance = 160f;

    private float speedMultiplier = 1f;

    private void Awake()
    {
        if (enemyPrefab == null || enemyPrefab.GetComponent<EnemyAI>() == null)
        {
            Debug.LogWarning("EnemySpawner: assign an enemy prefab that has an EnemyAI component, otherwise no enemies will spawn.", this);
            enabled = false;
        }
    }

    public void SpawnEnemies(int count)
    {
        if (!enabled)
        {
            return;
        }

        Vector3 centre = player != null ? player.position : transform.position;

        for (int i = 0; i < count; i++)
        {
            Vector3 position = centre + Random.onUnitSphere * Random.Range(minDistance, maxDistance);

            Instantiate(enemyPrefab, position, Random.rotation)
                .GetComponent<EnemyAI>()
                .SetSpeedMultiplier(speedMultiplier);
        }

        Debug.Log($"Spawned {count} enemy drone(s).");
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;

        foreach (EnemyAI enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            enemy.SetSpeedMultiplier(multiplier);
        }
    }
}
