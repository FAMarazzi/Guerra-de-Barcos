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
    
    // Devuelve la altura de la ola en una posición del mundo
    public float GetHeightAt(float worldX, float worldZ)
    {
        float localX = worldX - transform.position.x;
        float localZ = worldZ - transform.position.z;
        
        return Mathf.Sin(Time.time * _waveFrequency + localX * _waveLength)
            * Mathf.Cos(Time.time * _waveFrequency * 0.7f + localZ * _waveLength)
            * _waveHeight;
    }

}
