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

// Anima los vértices del plano para simular oleaje
public class WaterWaves : MonoBehaviour
{
    [SerializeField] private float _waveHeight = 0.15f;   // altura de las olas
    [SerializeField] private float _waveFrequency = 0.8f; // velocidad de las olas
    [SerializeField] private float _waveLength = 2f;      // longitud de onda

    private Mesh _mesh;
    private Vector3[] _baseVertices; // posiciones originales de los vértices

    private void Start()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        _baseVertices = _mesh.vertices; // guardamos las posiciones originales
    }

    private void Update()
    {
        Vector3[] vertices = new Vector3[_baseVertices.Length];

        for (int i = 0; i < _baseVertices.Length; i++)
        {
            Vector3 v = _baseVertices[i];
            // Ola en X y Z combinadas para que se vea natural
            v.y = Mathf.Sin(Time.time * _waveFrequency + v.x * _waveLength)
                * Mathf.Cos(Time.time * _waveFrequency * 0.7f + v.z * _waveLength)
                * _waveHeight;
            vertices[i] = v;
        }

        _mesh.vertices = vertices;
        _mesh.RecalculateNormals(); // para que la luz rebote bien en las olas
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
