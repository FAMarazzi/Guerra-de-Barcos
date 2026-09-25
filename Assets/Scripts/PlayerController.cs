/**FileHeader
 * @Author: Federico Marazzi
 * @Date: 9/21/2026, 1:06:19 PM
 * @LastEditors: Federico Marazzi
 * @LastEditTime: 9/25/2026, 11:36:58 AM
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

    [Header("Agua")]
    [SerializeField] private float _bobAmplitude = 0.05f; // altura del bamboleo en Y
    [SerializeField] private float _bobFrequency = 1.2f;  // velocidad del bamboleo

    [Header("Disparo")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private float _shootCooldown = 0.5f;

    [Header("Agua")]
    [SerializeField] private WaterWaves _water; //referencia al script de olas

    [Header("Respawn")]
    [SerializeField] private Transform _spawnPoint;

    private Rigidbody _rb;
    private float _shootTimer;
    private Vector3 _currentVelocity; // velocidad actual (permite inercia)
    private Quaternion _lastTargetRotation;   // último giro que estaba haciendo
    private float _currentAngularVelocity;    // velocidad de giro actual (va decayendo)
    private float _baseY;             // posición Y base del barco

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();
        _baseY = transform.position.y; // guardamos el Y inicial del barco
        _water = FindObjectOfType<WaterWaves>();
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

        // Movimiento con olas
        float waveY = _water != null ? _water.GetHeightAt(_rb.position.x, _rb.position.z) : 0f;
        Vector3 newPosition = _rb.position + _currentVelocity * Time.fixedDeltaTime;
        newPosition.y = _baseY + waveY;
        _rb.MovePosition(newPosition);

    }


    private void HandleShooting()
    {
        // Bajamos el timer cada frame
        if (_shootTimer > 0f)
            _shootTimer -= Time.deltaTime;

        // Disparo con click izquierdo o Space, si ya pasó el cooldown
        if (Input.GetButtonDown("Fire1") && _shootTimer <= 0f)
        {
            Shoot();
            _shootTimer = _shootCooldown;
        }
    }

    private void Shoot()
    {
        if (_bulletPrefab == null || _shootPoint == null) return;

        GameObject bullet = Instantiate(_bulletPrefab, _shootPoint.position, _shootPoint.rotation);
        BulletController bc = bullet.GetComponent<BulletController>();
        if (bc != null)
            bc.Launch(_shootPoint.forward); // le pasamos la dirección hacia donde mira el cañón
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
