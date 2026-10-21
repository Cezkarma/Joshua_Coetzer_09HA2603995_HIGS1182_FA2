using UnityEngine;

public static class GameTags
{
    public const string Player = "Player";
    public const string Asteroid = "Asteroid";
    public const string Enemy = "Enemy";
    public const string Scrap = "Scrap";

    public static bool Matches(Collider collider, string tag)
    {
        GameObject owner = collider.attachedRigidbody != null ? collider.attachedRigidbody.gameObject : collider.gameObject;
        return owner.CompareTag(tag);
    }
}
