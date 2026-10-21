using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    private enum State { Patrol, Chase }

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 8f;
    [SerializeField] private float chaseSpeed = 18f;
    [Tooltip("How quickly the drone changes direction. Higher is snappier.")]
    [SerializeField] private float turnSpeed = 3f;

    [Header("Patrol")]
    [Tooltip("How far from its patrol centre the drone picks new points to fly to.")]
    [SerializeField] private float patrolRadius = 40f;
    [SerializeField] private float waypointReachedDistance = 4f;

    [Header("Detection")]
    [Tooltip("The drone starts chasing when the player is this close and in line of sight.")]
    [SerializeField] private float detectionRange = 70f;
    [Tooltip("The drone gives up the chase when the player gets this far away.")]
    [SerializeField] private float loseRange = 110f;

    [Header("Feedback")]
    [SerializeField] private Color alertColour = new Color(1f, 0.2f, 0.2f);

    private Rigidbody rb;
    private Transform player;
    private State state = State.Patrol;
    private Vector3 patrolCentre;
    private Vector3 waypoint;
    private float speedMultiplier = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindWithTag(GameTags.Player);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("EnemyAI: no object tagged Player, so this drone will only patrol.", this);
        }

        patrolCentre = transform.position;
        PickNewWaypoint();
    }

    private void Update()
    {
        UpdateState();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    private void UpdateState()
    {
        if (player == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (state == State.Patrol && distance <= detectionRange && CanSeePlayer())
        {
            ChangeState(State.Chase);
        }
        else if (state == State.Chase && distance > loseRange)
        {
            ChangeState(State.Patrol);
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 toPlayer = player.position - transform.position;

        if (!Physics.Raycast(transform.position, toPlayer, out RaycastHit hit, toPlayer.magnitude + 1f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        return GameTags.Matches(hit.collider, GameTags.Player);
    }

    private void ChangeState(State newState)
    {
        state = newState;

        if (state == State.Chase)
        {
            Debug.Log($"Enemy Drone Spotted Player: {name} is now chasing.");
            HUDManager.ShowMessage("Enemy drone spotted you!", alertColour);
        }
        else
        {
            Debug.Log($"Enemy Drone lost the player: {name} is back on patrol.");
            patrolCentre = transform.position;
            PickNewWaypoint();
        }
    }

    private void Move()
    {
        Vector3 target = state == State.Chase && player != null ? player.position : waypoint;
        float speed = (state == State.Chase ? chaseSpeed : patrolSpeed) * speedMultiplier;

        Vector3 direction = (target - rb.position).normalized;
        if (direction == Vector3.zero)
        {
            return;
        }

        float blend = turnSpeed * Time.fixedDeltaTime;
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, direction * speed, blend);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(direction), blend));

        if (state == State.Patrol && Vector3.Distance(rb.position, waypoint) <= waypointReachedDistance)
        {
            PickNewWaypoint();
        }
    }

    private void PickNewWaypoint()
    {
        waypoint = patrolCentre + Random.insideUnitSphere * patrolRadius;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!GameTags.Matches(collision.collider, GameTags.Player))
        {
            return;
        }

        Debug.Log("Game lost: an enemy drone hit the player.");
        GameManager.EndRun("You lost! Destroyed by an enemy drone.");
    }
}
