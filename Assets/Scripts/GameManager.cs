/**FileHeader
 * @Author: Federico Marazzi
 * @Date: 9/29/2026, 3:45:00 PM
 * @LastEditors: Federico Marazzi
 * @LastEditTime: 9/29/2026, 3:45:00 PM
 * @Description: 
 * @Copyright: Copyright (©)}) 2026 Federico Marazzi. All rights reserved.
 * @Email: federicoandresmarazzi@gmail.com
 */

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject _goalZone;

    private int _enemiesAlive;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (_goalZone != null)
            _goalZone.SetActive(false); // Ocultar meta al inicio

        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        _enemiesAlive = enemies.Length;
        Debug.Log("Enemigos totales: " + _enemiesAlive);
    }

    public void OnEnemyDied()
    {
        _enemiesAlive--;
        Debug.Log("Enemigo destruido. Quedan: " + _enemiesAlive);

        if (_enemiesAlive <= 0)
        {
            UnlockGoal();
        }
    }

    private void UnlockGoal()
    {
        Debug.Log("¡Todos derrotados! Ve al puerto.");
        if (_goalZone != null)
            _goalZone.SetActive(true);
    }

    public void WinGame()
    {
        Debug.Log("¡NIVEL COMPLETADO!");
    }
}
