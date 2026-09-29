/**FileHeader
 * @Author: Federico Marazzi
 * @Date: 9/21/2026, 1:06:19 PM
 * @LastEditors: Federico Marazzi
 * @LastEditTime: 9/29/2026, 2:46:00 PM
 * @Description: 
 * @Copyright: Copyright (©)}) 2026 Federico Marazzi. All rights reserved.
 * @Email: federicoandresmarazzi@gmail.com
 */

using UnityEngine;
using UnityEngine.AI;

// Hereda de Entity (tiene vida, daño, muerte) y agrega IA naval
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : Entity
{
    [Header("IA Naval")]
    [SerializeField] private float _detectionRange = 30f;
    [SerializeField] private float _attackRange = 15f;
    [SerializeField] private float _attackCooldown = 2f;

    [Header("Disparo")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform[] _leftShootPoints;
    [SerializeField] private Transform[] _rightShootPoints;

    [Header("Agua")]
    [SerializeField] private WaterWaves _water;           // referencia al script de olas
    [SerializeField] private float _submersionDepth = 0.06f; // cuánto queda hundido bajo la superficie
    [SerializeField] private float _bobAmplitude = 0.03f;    // variación del hundimiento
    [SerializeField] private float _bobFrequency = 0.8f;     // velocidad del bamboleo sutil

    private NavMeshAgent _agent;
    private Transform _player;
    private float _attackTimer;
    private float _waterSurfaceY;

    protected override void Awake()
    {
        base.Awake(); // inicializa vida desde Entity
        _agent = GetComponent<NavMeshAgent>();
        // Frenamos un poco antes del rango máximo para tener margen
        _agent.stoppingDistance = _attackRange * 0.8f; 
        
        _water = FindObjectOfType<WaterWaves>();
        _waterSurfaceY = _water != null ? _water.transform.position.y : 0f;
    }

    private void Start()
    {
        // Buscamos al jugador por tag.
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
    }

    private void Update()
    {
        HandleWaterBob();

        if (_isDead || _player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        // Si está en rango visual, decide qué hacer
        if (distanceToPlayer <= _detectionRange)
        {
            ChaseAndPosition();
        }
        else
        {
            _agent.ResetPath(); // El jugador se escapó
        }

        if (_attackTimer > 0f)
            _attackTimer -= Time.deltaTime;
    }

    private void HandleWaterBob()
    {
        if (_water != null && _agent != null)
        {
            // Calculamos la altura de la ola en la posición del enemigo
            float waveY = _water.GetHeightAt(transform.position.x, transform.position.z);
            float bob = Mathf.Sin(Time.time * _bobFrequency) * _bobAmplitude;
            
            // Ajustamos el "desfase" del modelo respecto a la malla de navegación (NavMesh)
            float newYOffset = _waterSurfaceY + waveY - _submersionDepth + bob - transform.position.y + _agent.baseOffset;
            
            // Lo aplicamos directo sin Lerp para que no arranquen flotando en el aire
            _agent.baseOffset = newYOffset;
        }
    }

    private void ChaseAndPosition()
    {
        float distance = Vector3.Distance(transform.position, _player.position);

        if (distance > _attackRange)
        {
            // Aún está lejos, nos acercamos normal de frente
            _agent.isStopped = false;
            _agent.SetDestination(_player.position);
        }
        else
        {
            // Ya está a rango de disparo. Frenamos y nos ponemos de costado.
            _agent.isStopped = true;
            AlignBroadside();
        }
    }

    private void AlignBroadside()
    {
        // Vector que apunta hacia el jugador
        Vector3 dirToPlayer = (_player.position - transform.position).normalized;
        dirToPlayer.y = 0f;

        // Vector3.Dot devuelve > 0 si el jugador está a nuestra derecha, < 0 si está a la izquierda
        float dotRight = Vector3.Dot(transform.right, dirToPlayer);

        bool useRightCannons = dotRight > 0;
        Vector3 targetForward;

        if (useRightCannons)
        {
            // Queremos que nuestro estribor (derecha) apunte al jugador
            targetForward = Vector3.Cross(dirToPlayer, Vector3.up);
        }
        else
        {
            // Queremos que nuestro babor (izquierda) apunte al jugador
            targetForward = Vector3.Cross(Vector3.up, dirToPlayer);
        }

        // Girar el barco suavemente hacia la posición de disparo
        Quaternion targetRotation = Quaternion.LookRotation(targetForward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);

        // Si ya estamos más o menos paralelos (el Dot casi en 1 o -1), ¡Fuego!
        if (Mathf.Abs(dotRight) > 0.85f)
        {
            TryShoot(useRightCannons ? _rightShootPoints : _leftShootPoints);
        }
    }

    private void TryShoot(Transform[] points)
    {
        if (_attackTimer > 0f || _bulletPrefab == null) return;

        foreach (Transform pt in points)
        {
            if (pt == null) continue;
            GameObject bullet = Instantiate(_bulletPrefab, pt.position, pt.rotation);
            BulletController bc = bullet.GetComponent<BulletController>();
            if (bc != null)
                bc.Launch(pt.forward);
        }

        _attackTimer = _attackCooldown;
    }

    protected override void Die()
    {
        Debug.Log("Enemigo destruido");
        Destroy(gameObject);
    }
}
