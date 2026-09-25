/**FileHeader
 * @Author: Federico Marazzi
 * @Date: 9/21/2026, 12:59:47 PM
 * @LastEditors: Federico Marazzi
 * @LastEditTime: 9/21/2026, 1:21:39 PM
 * @Description: 
 * @Copyright: Copyright (©)}) 2026 Federico Marazzi. All rights reserved.
 * @Email: federicoandresmarazzi@gmail.com
 */
using UnityEngine;

// Clase base para cualquier entidad con vida (jugador, enemigos).
// "abstract" = no se usa directo, solo se hereda.
public abstract class Entity : MonoBehaviour, IDamageable
{
    [SerializeField] protected float _maxHealth = 100f;

    protected float _currentHealth;
    protected bool _isDead;

    // Propiedades públicas para que otros scripts puedan leer la vida
    public float CurrentHealth => _currentHealth;
    public bool IsDead => _isDead;

    protected virtual void Awake()
    {
        _currentHealth = _maxHealth;
        _isDead = false;
    }

    // Lógica de daño compartida para todos. Las hijas pueden pisarla con override.
    public virtual void TakeDamage(float amount)
    {
        if (_isDead) return;

        _currentHealth -= amount;
        _currentHealth = Mathf.Max(_currentHealth, 0f); // que no baje de 0

        OnDamaged(amount); // hook para efectos visuales (shake, color, etc.)

        if (_currentHealth <= 0f)
        {
            _isDead = true;
            Die();
        }
    }

    // Cada clase hija define cómo muere: el jugador hace respawn, el enemigo se destruye.
    protected abstract void Die();

    // ver si: sobreescribir en las hijas para agregar efectos al recibir daño
    protected virtual void OnDamaged(float amount) { }
}
