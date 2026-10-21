using UnityEngine;

public class BurstEffect : MonoBehaviour
{
    private float size;
    private float duration;
    private float elapsed;

    public void Play(float burstSize, float burstDuration)
    {
        size = burstSize;
        duration = Mathf.Max(burstDuration, 0.01f);
        elapsed = 0f;

        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float progress = elapsed / duration;

        if (progress >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        transform.localScale = Vector3.one * (size * Mathf.Sin(progress * Mathf.PI));
    }
}
