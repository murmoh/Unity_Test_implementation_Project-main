using UnityEngine;
using UnityEngine.AI;

namespace UnityTutorial.EnemyBotControl
{
    public class EnemyBotController : MonoBehaviour
    {
        [SerializeField] private GameObject playerObject;
        [SerializeField] private float stoppingDistance = 2.0f;
        [SerializeField] private float chaseRadius = 10.0f;
        [SerializeField] private float wanderRadius = 20.0f;
        [SerializeField] private float AnimBlendSpeed = 8.9f;
        [SerializeField] private float wanderTimer = 5.0f;

        private NavMeshAgent _navMeshAgent;
        public bool isOnNavMesh;
        private Rigidbody _playerRigidbody;
        private Animator _animator;
        private bool _hasAnimator;
        private int _xVelHash;
        private int _yVelHash;
        private Vector2 _currentVelocity;
        private float _timer;
        public Spawner script;

        private void Start()
        {
            _hasAnimator = TryGetComponent<Animator>(out _animator);
            _playerRigidbody = GetComponent<Rigidbody>();
            _navMeshAgent = GetComponent<NavMeshAgent>();

            _navMeshAgent.updateRotation = false;
            _navMeshAgent.stoppingDistance = stoppingDistance;

            _xVelHash = Animator.StringToHash("angularspeed");
            _yVelHash = Animator.StringToHash("speed");

            playerObject = GameObject.FindWithTag("Player");
        }

        private void FixedUpdate()
        {
            if (isOnNavMesh)
            {
                Move();
            }

            if(script.Spawn != true)
            {
                isOnNavMesh = true;
            }
        }

        private void Move()
        {
            if (!_hasAnimator) return;

            float distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);

            if (distanceToPlayer <= chaseRadius)
            {
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

            _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, _navMeshAgent.velocity.x, AnimBlendSpeed * Time.fixedDeltaTime);
            _currentVelocity.y = Mathf.Lerp(_currentVelocity.y, _navMeshAgent.velocity.z, AnimBlendSpeed * Time.fixedDeltaTime);

            if (_navMeshAgent.velocity.sqrMagnitude > Mathf.Epsilon) // Check if the agent is moving
            {
                Quaternion targetRotation = Quaternion.LookRotation(_navMeshAgent.velocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * AnimBlendSpeed);
            }

            _animator.SetFloat("angularspeed", _currentVelocity.x);
            _animator.SetFloat("speed", _currentVelocity.y);
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
}
