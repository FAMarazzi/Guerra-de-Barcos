/**FileHeader
 * @Author: Federico Marazzi
 * @Date: 9/30/2026, 9:58:00 AM
 * @LastEditors: Federico Marazzi
 * @LastEditTime: 10/1/2026, 11:24:32 AM
 * @Description: Apunta un objeto (ej. una flecha) hacia un objetivo
 * @Copyright: Copyright (©)}) 2026 Federico Marazzi. All rights reserved.
 * @Email: federicoandresmarazzi@gmail.com
 */

using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ObjectivePointer : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform _target;
    
    [Header("Apariencia")]
    [SerializeField] private float _lineLength = 12f;
    [SerializeField] private float _width = 0.4f;
    [SerializeField] private Color _arrowColor = new Color(1f, 0.85f, 0f);
    [SerializeField] private float _heightAboveShip = 5f;

    [Header("Corrección del objetivo")]
    [Tooltip("Ajustá esto si la flecha apunta al pivote equivocado de la isla. X = izq/der, Z = adelante/atrás")]
    [SerializeField] private Vector3 _targetOffset = Vector3.zero;

    private LineRenderer _lr;

    private void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = 5;
        _lr.useWorldSpace = true;
        _lr.material = new Material(Shader.Find("Sprites/Default"));
        _lr.startColor = _arrowColor;
        _lr.endColor = _arrowColor;
        _lr.startWidth = _width;
        _lr.endWidth = _width;
    }

    private void Update()
    {
        if (_target == null) return;

        // Origen fijo en el cielo sobre el barco, en coordenadas de MUNDO (no locales)
        Vector3 origin = new Vector3(
            transform.position.x,
            transform.position.y + _heightAboveShip,
            transform.position.z
        );

        // Destino a la misma altura para que la flecha sea completamente horizontal
        Vector3 destination = new Vector3(
            _target.position.x + _targetOffset.x,
            origin.y,
            _target.position.z + _targetOffset.z
        );

        // Dirección normalizada, puramente en el plano XZ
        Vector3 dir = (destination - origin).normalized;
        if (dir.sqrMagnitude < 0.001f) return;

        // Punta de la flecha
        Vector3 tip = origin + dir * _lineLength;

        // Vector perpendicular horizontal para las alas (no depende de ningún transform)
        Vector3 perp = new Vector3(-dir.z, 0f, dir.x);
        float wingBack  = _lineLength * 0.2f;
        float wingSide  = _lineLength * 0.1f;

        Vector3 leftWing  = tip - dir * wingBack + perp * wingSide;
        Vector3 rightWing = tip - dir * wingBack - perp * wingSide;

        _lr.SetPosition(0, origin);    // cola
        _lr.SetPosition(1, tip);       // punta
        _lr.SetPosition(2, leftWing);  // ala izq
        _lr.SetPosition(3, tip);       // vuelve a la punta
        _lr.SetPosition(4, rightWing); // ala der
    }
}
