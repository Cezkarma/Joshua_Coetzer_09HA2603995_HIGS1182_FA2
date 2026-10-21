using UnityEngine;
using UnityEngine.Rendering;

public static class Effects
{
    public static void SpawnBurst(Vector3 position, float size, Color colour, float duration = 0.35f)
    {
        GameObject burst = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        burst.name = "Burst";
        burst.transform.position = position;

        Collider burstCollider = burst.GetComponent<Collider>();
        burstCollider.enabled = false;
        Object.Destroy(burstCollider);

        Renderer burstRenderer = burst.GetComponent<Renderer>();
        burstRenderer.material.color = colour;
        burstRenderer.shadowCastingMode = ShadowCastingMode.Off;

        burst.AddComponent<BurstEffect>().Play(size, duration);
    }
}
