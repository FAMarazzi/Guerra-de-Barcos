/**FileHeader
 * @Author: Federico Marazzi
 * @Date: 10/02/2026, 2:40:00 PM
 * @LastEditors: Federico Marazzi
 * @LastEditTime: 10/02/2026, 2:40:00 PM
 * @Description: Pool de objetos para balas. Evita Instantiate/Destroy en runtime.
 * @Copyright: Copyright (©)}) 2026 Federico Marazzi. All rights reserved.
 * @Email: federicoandresmarazzi@gmail.com
 */

using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private int _poolSize = 30;

    private readonly Queue<BulletController> _available = new Queue<BulletController>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Pre-instanciamos todas las balas y las desactivamos
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject obj = Instantiate(_bulletPrefab, transform);
            obj.SetActive(false);
            BulletController bc = obj.GetComponent<BulletController>();
            _available.Enqueue(bc);
        }
    }

    // Devuelve una bala disponible, la posiciona y la activa
    public BulletController Get(Vector3 position, Quaternion rotation)
    {
        BulletController bc;

        if (_available.Count > 0)
        {
            bc = _available.Dequeue();
        }
        else
        {
            // Si el pool está vacío, creamos una nueva (safety net)
            GameObject obj = Instantiate(_bulletPrefab, transform);
            bc = obj.GetComponent<BulletController>();
        }

        bc.transform.SetPositionAndRotation(position, rotation);
        bc.gameObject.SetActive(true);
        return bc;
    }

    // Desactiva la bala y la devuelve al pool
    public void Return(BulletController bc)
    {
        bc.gameObject.SetActive(false);
        bc.transform.SetParent(transform);
        _available.Enqueue(bc);
    }
}
