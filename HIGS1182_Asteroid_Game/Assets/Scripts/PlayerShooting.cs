using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Tooltip("Where shots start from. Left empty, the child named ShootingPoint is used.")]
    [SerializeField] private Transform shootingPoint;
    [Tooltip("How far a shot reaches, in world units.")]
    [SerializeField] private float range = 400f;
    [Tooltip("How far down the camera's line of sight aiming starts, so the ship's own hull is never read as the target.")]
    [SerializeField] private float shipClearance = 15f;

    [Header("Damage")]
    [Tooltip("Layers a shot can hit. Set this to Asteroid and Enemy.")]
    [SerializeField] private LayerMask targetLayers = ~0;
    [SerializeField] private int damage = 1;

    [Header("Feedback")]
    [SerializeField] private Color hitColour = new Color(1f, 0.3f, 0.2f);
    [SerializeField] private float hitBurstSize = 1.5f;

    private Camera cam;

    private void Awake()
    {
        if (shootingPoint == null)
        {
            shootingPoint = transform.Find("ShootingPoint");
        }
    }

    private void Start()
    {
        cam = Camera.main;

        if (cam == null || shootingPoint == null)
        {
            Debug.LogWarning("PlayerShooting: needs a camera tagged MainCamera and a ShootingPoint, so shooting is disabled.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.Locked)
        {
            HandleShooting();
        }
    }

    private void HandleShooting()
    {
        Vector3 origin = shootingPoint.position;
        Vector3 direction = (GetAimPoint() - origin).normalized;

        Debug.DrawRay(origin, direction * range, Color.red, 0.2f);

        if (Raycast(new Ray(origin, direction), out RaycastHit hit))
        {
            ApplyDamage(hit);
        }
    }

    private void ApplyDamage(RaycastHit hit)
    {
        GameObject target = hit.rigidbody != null ? hit.rigidbody.gameObject : hit.collider.gameObject;

        if (!target.CompareTag(GameTags.Asteroid) && !target.CompareTag(GameTags.Enemy))
        {
            return;
        }

        Health health = target.GetComponent<Health>();
        if (health == null)
        {
            Debug.LogWarning($"PlayerShooting: {target.name} is tagged {target.tag} but has no Health component.", target);
            return;
        }

        Debug.Log($"Shot hit {target.tag}: {target.name}.");
        health.ApplyDamage(damage);

        Effects.SpawnBurst(hit.point, hitBurstSize, hitColour);
        HUDManager.ShowMessage(health.IsDead ? $"{target.tag} destroyed" : $"{target.tag} hit ({health.CurrentHealth}/{health.MaxHealth})", hitColour);
    }

    private Vector3 GetAimPoint()
    {
        Ray aim = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        aim.origin = aim.GetPoint(shipClearance);

        return Raycast(aim, out RaycastHit hit) ? hit.point : aim.GetPoint(range);
    }

    private bool Raycast(Ray ray, out RaycastHit hit)
    {
        return Physics.Raycast(ray, out hit, range, targetLayers, QueryTriggerInteraction.Ignore);
    }
}
