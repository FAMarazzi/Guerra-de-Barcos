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

[RequireComponent(typeof(Rigidbody))]
public class BulletController : MonoBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _damage = 25f;
    [SerializeField] private float _lifeTime = 5f; // se autodestruye si no choca con nada

    private Rigidbody _rb;

    private string _shooterTag;

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

    // Se llama desde PlayerController o EnemyController al disparar
    public void Launch(Vector3 direction, string shooterTag)
    {
        _shooterTag = shooterTag;
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
        // Si la bala choca contra quien la disparó (o alguien de su bando), ignoramos
        if (!string.IsNullOrEmpty(_shooterTag) && target.CompareTag(_shooterTag)) 
            return;

        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.TakeDamage(_damage);

        Destroy(gameObject);
    }
}
