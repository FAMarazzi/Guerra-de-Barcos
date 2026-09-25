/**FileHeader
 * @Author: Federico Marazzi
 * @Date: 9/21/2026, 12:59:28 PM
 * @LastEditors: Federico Marazzi
 * @LastEditTime: 9/21/2026, 1:21:40 PM
 * @Description: 
 * @Copyright: Copyright (©)}) 2026 Federico Marazzi. All rights reserved.
 * @Email: federicoandresmarazzi@gmail.com
 */

// Contrato: cualquier cosa que pueda recibir daño implementa esto
// (jugador, enemigos, y lo que quieras agregar después)
public interface IDamageable
{
    void TakeDamage(float amount); // recibe daño
    float CurrentHealth { get; }  // vida actual (solo lectura desde afuera)
    bool IsDead { get; }          // está muerto?
}
