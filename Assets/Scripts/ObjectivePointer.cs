/**FileHeader
 * @Author: Federico Marazzi
 * @Date: 9/30/2026, 9:58:00 AM
 * @LastEditors: Federico Marazzi
 * @LastEditTime: 9/30/2026, 10:21:44 AM
 * @Description: Apunta un objeto (ej. una flecha) hacia un objetivo
 * @Copyright: Copyright (©)}) 2026 Federico Marazzi. All rights reserved.
 * @Email: federicoandresmarazzi@gmail.com
 */

using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ObjectivePointer : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _lineLength = 12f;
    [SerializeField] private float _width = 0.4f;
    [SerializeField] private Color _arrowColor = new Color(1f, 0.85f, 0f); // amarillo dorado

    [Header("Desplazamiento (respecto al barco)")]
    [Tooltip("X: Lados, Y: Altura, Z: Adelante")]
    [SerializeField] private Vector3 _startOffset = new Vector3(0f, 5f, 0f);

    [Header("Corrección de dirección")]
    [Tooltip("Girá este valor si la flecha apunta torcida. Negativo = izquierda, Positivo = derecha")]
    [SerializeField] private float _directionOffsetDegrees = 0f;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 5;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = _arrowColor;
        _lineRenderer.endColor = _arrowColor;
        _lineRenderer.startWidth = _width;
        _lineRenderer.endWidth = _width;
    }

    private void Update()
    {
        if (_target == null) return;

        Vector3 startPos = transform.position;
        startPos.y += _startOffset.y;

        Vector3 targetPos = _target.position;
        targetPos.y = startPos.y;

        Vector3 dir = (targetPos - startPos).normalized;
        if (dir == Vector3.zero) return;

        // Aplicamos el offset de corrección rotando la dirección sobre el eje Y
        dir = Quaternion.AngleAxis(_directionOffsetDegrees, Vector3.up) * dir;

        Vector3 tipPos = startPos + dir * _lineLength;
        Vector3 right = new Vector3(-dir.z, 0f, dir.x);
        float wingLen = _lineLength * 0.18f;
        float wingWidth = _lineLength * 0.12f;

        Vector3 leftWing  = tipPos - dir * wingLen + right * wingWidth;
        Vector3 rightWing = tipPos - dir * wingLen - right * wingWidth;

        _lineRenderer.SetPosition(0, startPos);
        _lineRenderer.SetPosition(1, tipPos);
        _lineRenderer.SetPosition(2, leftWing);
        _lineRenderer.SetPosition(3, tipPos);
        _lineRenderer.SetPosition(4, rightWing);
    }
}
