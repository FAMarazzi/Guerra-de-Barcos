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

[RequireComponent(typeof(Collider))]
public class GoalZone : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.AreAllEnemiesDefeated())
                {
                    GameManager.Instance.WinGame();
                }
                else
                {
                    Debug.Log("¡Aún quedan piratas vivos! Mátalos antes de entrar.");
                }
            }
        }
    }
}
