/**FileHeader
 * @Author: Federico Marazzi
 * @Date: 9/21/2026, 1:06:19 PM
 * @LastEditors: Federico Marazzi
 * @LastEditTime: 9/28/2026, 11:24:29 AM
 * @Description: 
 * @Copyright: Copyright (©)}) 2026 Federico Marazzi. All rights reserved.
 * @Email: federicoandresmarazzi@gmail.com
 */
 
using UnityEngine;

// Hereda de Entity (tiene vida, daño, muerte) y agrega movimiento + disparo del jugador
public class PlayerController : Entity
{
    [Header("Movimiento")]
    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _rotationSpeed = 200f;
    [SerializeField] private float _drag = 3f; // qué tan rápido frena la inercia al soltar

    [Header("Disparo")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform[] _leftShootPoints;
    [SerializeField] private Transform[] _rightShootPoints;
    [SerializeField] private float _shootCooldown = 1f;

    [Header("Agua")]
    [SerializeField] private WaterWaves _water;           // referencia al script de olas
    [SerializeField] private float _submersionDepth = 0.06f; // cuánto queda siempre hundido bajo la superficie
    [SerializeField] private float _bobAmplitude = 0.03f;    // variación del hundimiento (debe ser < submersionDepth)
    [SerializeField] private float _bobFrequency = 0.8f;     // velocidad del bamboleo sutil

    [Header("Respawn")]
    [SerializeField] private Transform _spawnPoint;

    private Rigidbody _rb;
    private float _leftShootTimer;
    private float _rightShootTimer;
    private Vector3 _currentVelocity; // velocidad actual (permite inercia)
    private Quaternion _lastTargetRotation;   // último giro que estaba haciendo
    private float _currentAngularVelocity;    // velocidad de giro actual (va decayendo)
    private float _waterSurfaceY;     // Y del plano del agua (constante)

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();
        _water = FindObjectOfType<WaterWaves>();
        // Usamos la Y del plano del agua, no la del barco en el spawn
        _waterSurfaceY = _water != null ? _water.transform.position.y : 0f;
    }

    private void Update()
    {
        HandleShooting();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude > 0.1f)
        {
            _currentVelocity = direction * _speed;
            _lastTargetRotation = Quaternion.LookRotation(direction);
            _currentAngularVelocity = _rotationSpeed; // giro a velocidad máxima
        }
        else
        {
            // Inercia lineal: va frenando
            _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, _drag * Time.fixedDeltaTime);
            // Inercia angular: el giro también va frenando
            _currentAngularVelocity = Mathf.Lerp(_currentAngularVelocity, 0f, _drag * Time.fixedDeltaTime);
        }

        // Aplicamos la rotación con la velocidad actual (con o sin input, va decayendo)
        _rb.MoveRotation(Quaternion.RotateTowards(
            _rb.rotation, _lastTargetRotation, _currentAngularVelocity * Time.fixedDeltaTime));

        // Movimiento con olas: calculo la nueva posición XZ primero,
        // y evalúo la ola EN ESA posición para no tener desfasaje de un frame
        Vector3 newPosition = _rb.position + _currentVelocity * Time.fixedDeltaTime;
        float waveY = _water != null ? _water.GetHeightAt(newPosition.x, newPosition.z) : 0f;
        // Bob sutil: varía cuánto se hunde, pero submersionDepth > bobAmplitude
        // así el barco siempre queda al menos un poco bajo la superficie
        float bob = Mathf.Sin(Time.time * _bobFrequency) * _bobAmplitude;
        newPosition.y = _waterSurfaceY + waveY - _submersionDepth + bob;
        _rb.MovePosition(newPosition);

    }


    private void HandleShooting()
    {
        // Bajamos el timer cada frame
        if (_leftShootTimer > 0f) _leftShootTimer -= Time.deltaTime;
        if (_rightShootTimer > 0f) _rightShootTimer -= Time.deltaTime;

        // Click Izquierdo (Fire1) para disparar babor (izquierda)
        if (Input.GetButtonDown("Fire1") && _leftShootTimer <= 0f)
        {
            ShootSide(_leftShootPoints);
            _leftShootTimer = _shootCooldown;
        }

        // Click Derecho (Fire2) para disparar estribor (derecha)
        if (Input.GetButtonDown("Fire2") && _rightShootTimer <= 0f)
        {
            ShootSide(_rightShootPoints);
            _rightShootTimer = _shootCooldown;
        }
    }

    private void ShootSide(Transform[] points)
    {
        if (_bulletPrefab == null || points == null) return;

        foreach (Transform pt in points)
        {
            if (pt == null) continue;
            GameObject bullet = Instantiate(_bulletPrefab, pt.position, pt.rotation);
            BulletController bc = bullet.GetComponent<BulletController>();
            if (bc != null)
                bc.Launch(pt.forward); // le pasamos la dirección hacia donde mira el cañón
        }
    }

    // Override de Entity: el jugador no se destruye, hace respawn
    protected override void Die()
    {
        Debug.Log("Jugador muerto - respawneando");
        Respawn();
    }

    private void Respawn()
    {
        // Resetea vida y estado
        _isDead = false;
        _currentHealth = _maxHealth;

        // Lo manda al punto de spawn
        if (_spawnPoint != null)
            transform.position = _spawnPoint.position;
        else
            transform.position = Vector3.zero;
    }
}
