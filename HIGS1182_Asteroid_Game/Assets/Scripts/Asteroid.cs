using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Asteroid : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float minSpeed = 14f;
    [SerializeField] private float maxSpeed = 24f;

    [Header("Lifetime")]
    [Tooltip("Seconds the asteroid stays alive before despawning.")]
    [SerializeField] private float lifetime = 45f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void Launch(Vector3 direction)
    {
        rb.linearVelocity = direction.normalized * Random.Range(minSpeed, maxSpeed);

        Invoke(nameof(Despawn), lifetime);
    }

    private void Despawn()
    {
        Debug.Log("Asteroid destroyed: lifetime ran out.");
        Destroy(gameObject);
    }
}
