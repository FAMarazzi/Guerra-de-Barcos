using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletController : MonoBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _damage = 25f;
    [SerializeField] private float _lifeTime = 5f; // se autodestruye si no choca con nada

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;                                          // no cae
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // no traspasa
    }

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    // Se llama desde PlayerController al disparar
    public void Launch(Vector3 direction)
    {
        // BUG del profe: multiplicaba por Time.fixedDeltaTime → bala lentísima
        // La velocidad del Rigidbody ya es en unidades/segundo, no necesita delta time
        _rb.linearVelocity = direction.normalized * _speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Hit(collision.gameObject);
    }

    // El prefab de shell del proyecto Tanks usa Trigger, así que cubrimos los dos casos
    private void OnTriggerEnter(Collider other)
    {
        Hit(other.gameObject);
    }

    private void Hit(GameObject target)
    {
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.TakeDamage(_damage);

        Destroy(gameObject);
    }
}
