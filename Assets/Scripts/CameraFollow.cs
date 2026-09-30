/**FileHeader
 * @Author: Federico Marazzi
 * @Date: 9/30/2026, 10:17:00 AM
 * @LastEditors: Federico Marazzi
 * @LastEditTime: 9/30/2026, 10:17:00 AM
 * @Description: Cámara suave que persigue al jugador
 * @Copyright: Copyright (©)}) 2026 Federico Marazzi. All rights reserved.
 * @Email: federicoandresmarazzi@gmail.com
 */

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform _target;

    [Header("Posición")]
    [Tooltip("Altura y distancia de la cámara respecto al barco")]
    [SerializeField] private Vector3 _offset = new Vector3(0f, 22f, -15f);
    [Tooltip("Qué tan suave sigue a la cámara cuando sale de la zona muerta")]
    [SerializeField] private float _smoothSpeed = 3f;

    [Header("Zona Muerta")]
    [Tooltip("El barco puede alejarse esta cantidad de unidades del centro sin que la cámara se mueva")]
    [SerializeField] private float _deadZoneRadius = 4f;

    private Vector3 _currentTarget; // El punto al que la cámara persigue (no el barco directo)

    private void Start()
    {
        if (_target != null)
            _currentTarget = _target.position;
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        // Solo actualizamos el objetivo de la cámara si el barco salió de la zona muerta
        float dist = Vector3.Distance(
            new Vector3(_target.position.x, 0, _target.position.z),
            new Vector3(_currentTarget.x, 0, _currentTarget.z)
        );

        if (dist > _deadZoneRadius)
        {
            // Empujamos el target solo lo suficiente para que el barco quede en el borde de la zona muerta
            Vector3 dir = (_target.position - _currentTarget).normalized;
            _currentTarget = _target.position - dir * _deadZoneRadius;
        }

        // Movemos la cámara suavemente hacia la posición deseada
        Vector3 desiredPos = new Vector3(_currentTarget.x, _target.position.y, _currentTarget.z) + _offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, _smoothSpeed * Time.deltaTime);

        // Siempre mira al barco
        transform.LookAt(_target);
    }
}
