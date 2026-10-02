/**FileHeader
 * @Author: Federico Marazzi
 * @Date: 9/21/2026, 1:06:19 PM
 * @LastEditors: Federico Marazzi
 * @LastEditTime: 10/2/2026, 9:21:32 AM
 * @Description: 
 * @Copyright: Copyright (©)}) 2026 Federico Marazzi. All rights reserved.
 * @Email: federicoandresmarazzi@gmail.com
 */

using UnityEngine;

// Anima los vértices del plano para simular oleaje
public class WaterWaves : MonoBehaviour
{
    [SerializeField] private float _waveHeight = 0.15f;   // altura de las olas
    [SerializeField] private float _waveFrequency = 0.8f; // velocidad de las olas
    [SerializeField] private float _waveLength = 2f;      // longitud de onda
    [SerializeField] private float _calmSpeed = 0.3f;     // velocidad a la que el mar se calma

    private Mesh _mesh;
    private Vector3[] _baseVertices;
    private float _targetWaveHeight;
    private float _originalWaveHeight; // guardamos el valor inicial del Inspector

    private void Start()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        _baseVertices = _mesh.vertices;
        _originalWaveHeight = _waveHeight; // leemos lo que puso el usuario en el Inspector
        _targetWaveHeight = _waveHeight;
    }

    private void Update()
    {
        // Interpolamos suavemente la altura hacia el objetivo (normal o calmada)
        _waveHeight = Mathf.Lerp(_waveHeight, _targetWaveHeight, _calmSpeed * Time.deltaTime);

        Vector3[] vertices = new Vector3[_baseVertices.Length];

        for (int i = 0; i < _baseVertices.Length; i++)
        {
            Vector3 v = _baseVertices[i];
            v.y = Mathf.Sin(Time.time * _waveFrequency + v.x * _waveLength)
                * Mathf.Cos(Time.time * _waveFrequency * 0.7f + v.z * _waveLength)
                * _waveHeight;
            vertices[i] = v;
        }

        _mesh.vertices = vertices;
        _mesh.RecalculateNormals();
    }

    // Llamado desde GameManager cuando todos los enemigos mueren
    public void CalmDown()
    {
        // Baja a la mitad de lo que el usuario tenga configurado en el Inspector
        _targetWaveHeight = _originalWaveHeight * 0.5f;
    }
    
    // Devuelve la altura de la ola en una posición del mundo.
    // Uso InverseTransformPoint para convertir correctamente a espacio local,
    // teniendo en cuenta posición, rotación Y escala del plano de agua.
    // (La resta simple worldX - position.x ignoraba la escala y causaba desfasaje)
    public float GetHeightAt(float worldX, float worldZ)
    {
        Vector3 localPos = transform.InverseTransformPoint(worldX, 0f, worldZ);

        return Mathf.Sin(Time.time * _waveFrequency + localPos.x * _waveLength)
            * Mathf.Cos(Time.time * _waveFrequency * 0.7f + localPos.z * _waveLength)
            * _waveHeight;
    }

}
