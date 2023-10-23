using UnityEngine;
using UnityEngine.AI;

public class EnemyBotController : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;
    [SerializeField] private float stoppingDistance = 2.0f;
    [SerializeField] private float chaseRadius = 10.0f;
    [SerializeField] private float wanderRadius = 20.0f;
    [SerializeField] private float wanderTimer = 5.0f;

    private NavMeshAgent _navMeshAgent;
    public bool isOnNavMesh;
    private Rigidbody _playerRigidbody;
    private float _timer;
    public Spawner script;
    private bool isChasing; // Variable added to keep track of chasing state

    private void Start()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _navMeshAgent.updateRotation = false;
        _navMeshAgent.stoppingDistance = stoppingDistance;

        playerObject = GameObject.FindWithTag("Player");
    }

    private void FixedUpdate()
    {
        if (isOnNavMesh)
        {
            Move();
        }

        if (script.Spawn != true)
        {
            isOnNavMesh = true;
        }
    }

    private void Move()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);

        if (distanceToPlayer <= chaseRadius || isChasing) // Modified condition
        {
            isChasing = true; // Set to true when player enters chase radius
            _navMeshAgent.SetDestination(playerObject.transform.position);
        }
        else
        {
            _timer += Time.fixedDeltaTime;
            if (_timer >= wanderTimer)
            {
                RandomWander();
                _timer = 0;
            }
        }

        if (_navMeshAgent.velocity.sqrMagnitude > Mathf.Epsilon) // Check if the agent is moving
        {
            Quaternion targetRotation = Quaternion.LookRotation(_navMeshAgent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime);
        }
    }

    private void RandomWander()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, -1);
        _navMeshAgent.SetDestination(navHit.position);
    }
}
