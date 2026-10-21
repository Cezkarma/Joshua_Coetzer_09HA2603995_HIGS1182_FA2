using TMPro;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    [SerializeField] private GameObject scrapPrefab;
    [SerializeField] private int scrapCount = 10;

    [Header("UI")]
    [Tooltip("Shows the running tally as 'Scrap: collected/total'.")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Placement")]
    [Tooltip("Who to scatter the scrap around. Leave empty to use this object's position.")]
    [SerializeField] private Transform player;
    [Tooltip("Scrap is placed between these distances from the player, so none of it starts on top of them.")]
    [SerializeField] private float minDistance = 40f;
    [SerializeField] private float maxDistance = 150f;

    [Header("Feedback")]
    [Tooltip("Colour of the pickup burst and the '+1 Scrap' message.")]
    [SerializeField] private Color pickupColour = new Color(0.3f, 1f, 0.5f);
    [SerializeField] private float pickupBurstSize = 3f;

    private int total;
    private int collected;

    private void Start()
    {
        total = scrapCount;
        GameManager.ReportScrap(collected, total);
        RefreshScoreText();

        if (scrapPrefab == null)
        {
            Debug.LogWarning("CollectibleManager: assign a scrap prefab, otherwise nothing will spawn.", this);
            return;
        }

        Vector3 centre = player != null ? player.position : transform.position;

        for (int i = 0; i < scrapCount; i++)
        {
            Vector3 position = centre + Random.onUnitSphere * Random.Range(minDistance, maxDistance);

            Instantiate(scrapPrefab, position, Random.rotation)
                .AddComponent<ScrapPickup>()
                .Collected += OnScrapCollected;
        }

        Debug.Log($"Generated all {scrapCount} pieces of scrap.");
    }

    private void OnScrapCollected(Vector3 position)
    {
        collected++;
        GameManager.ReportScrap(collected, total);
        RefreshScoreText();
        Debug.Log($"Scrap collected. {total - collected} piece(s) left.");

        Effects.SpawnBurst(position, pickupBurstSize, pickupColour);
        HUDManager.ShowMessage("+1 Scrap", pickupColour);

        if (collected >= total)
        {
            Debug.Log("Game won: all scrap collected.");
            GameManager.EndRun("You won!");
        }
    }

    private void RefreshScoreText()
    {
        if (scoreText == null)
        {
            return;
        }

        scoreText.text = $"Scrap: {collected}/{total}";
    }
}

public class ScrapPickup : MonoBehaviour
{
    public System.Action<Vector3> Collected;

    private void OnTriggerEnter(Collider other)
    {
        if (!GameTags.Matches(other, GameTags.Player))
        {
            return;
        }

        Collected?.Invoke(transform.position);
        Destroy(gameObject);
    }
}
