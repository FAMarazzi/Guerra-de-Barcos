using UnityEngine;
using UnityEngine.AI; // necesario para NavMeshAgent

// Hereda de Entity (tiene vida, daño, muerte) y agrega IA de persecución
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : Entity
{
    [Header("IA")]
    [SerializeField] private float _detectionRange = 15f;  // rango para detectar al jugador
    [SerializeField] private float _attackRange = 1.5f;    // rango para hacer daño por contacto
    [SerializeField] private float _contactDamage = 10f;   // daño al tocar al jugador
    [SerializeField] private float _attackCooldown = 1f;   // tiempo entre golpes

    private NavMeshAgent _agent;
    private Transform _player;
    private float _attackTimer;

    protected override void Awake()
    {
        base.Awake(); // inicializa vida desde Entity
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        // Buscamos al jugador por tag. Acordate de tagear el tanque jugador como "Player" en Unity
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
    }

    private void Update()
    {
        if (_isDead || _player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (distanceToPlayer <= _detectionRange)
            ChasePlayer();
        else
            _agent.ResetPath(); // para si el jugador está lejos

        if (distanceToPlayer <= _attackRange)
            TryAttack();

        if (_attackTimer > 0f)
            _attackTimer -= Time.deltaTime;
    }

    private void ChasePlayer()
    {
        _agent.SetDestination(_player.position);
    }

    private void TryAttack()
    {
        if (_attackTimer > 0f) return;

        // Ataque de contacto: busca IDamageable en el jugador y le hace daño
        IDamageable target = _player.GetComponent<IDamageable>();
        if (target != null)
            target.TakeDamage(_contactDamage);

        _attackTimer = _attackCooldown;
    }

    // Override de Entity: el enemigo simplemente se destruye al morir
    protected override void Die()
    {
        // Acá podés instanciar un efecto de explosión si querés
        Destroy(gameObject);
    }
}
