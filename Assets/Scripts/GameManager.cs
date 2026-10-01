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
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        _enemiesAlive = enemies.Length;
        Debug.Log("Enemigos totales: " + _enemiesAlive);
    }

    public void OnEnemyDied()
    {
        _enemiesAlive--;
        Debug.Log("Enemigo destruido. Quedan: " + _enemiesAlive);

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateEnemyCount(_enemiesAlive);

        if (_enemiesAlive <= 0)
            Debug.Log("¡Todos derrotados! Ya puedes entrar al puerto.");
    }

    public bool AreAllEnemiesDefeated()
    {
        return _enemiesAlive <= 0;
    }

    public void WinGame()
    {
        Debug.Log("¡NIVEL COMPLETADO!");
        if (UIManager.Instance != null)
            UIManager.Instance.ShowWinScreen();
    }
}
