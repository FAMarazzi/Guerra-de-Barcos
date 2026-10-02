/**FileHeader
 * @Author: Federico Marazzi
 * @Date: 9/21/2026, 1:06:19 PM
 * @LastEditors: Federico Marazzi
 * @LastEditTime: 10/2/2026, 2:48:26 PM
 * @Description: 
 * @Copyright: Copyright (©)}) 2026 Federico Marazzi. All rights reserved.
 * @Email: federicoandresmarazzi@gmail.com
 */

using System.Collections;
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

    // OnEnable se llama cada vez que el pool activa la bala (no solo al crear)
    private void OnEnable()
    {
        _shooterTag = string.Empty;
        _rb.linearVelocity = Vector3.zero;
        StartCoroutine(ReturnAfterLifetime());
    }

    private IEnumerator ReturnAfterLifetime()
    {
        yield return new WaitForSeconds(_lifeTime);
        ReturnToPool();
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
        if (!string.IsNullOrEmpty(_shooterTag) && target.CompareTag(_shooterTag))
            return;

        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.TakeDamage(_damage);

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        StopAllCoroutines();
        if (BulletPool.Instance != null)
            BulletPool.Instance.Return(this);
        else
            Destroy(gameObject); // fallback si no hay pool en escena
    }
}
