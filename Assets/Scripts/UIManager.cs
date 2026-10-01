/**FileHeader
 * @Author: Federico Marazzi
 * @Date: 10/01/2026, 10:06:00 AM
 * @LastEditors: Federico Marazzi
 * @LastEditTime: 10/1/2026, 10:24:42 AM
 * @Description: Maneja todo el HUD y las pantallas del juego
 * @Copyright: Copyright (©)}) 2026 Federico Marazzi. All rights reserved.
 * @Email: federicoandresmarazzi@gmail.com
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD (en juego)")]
    [SerializeField] private GameObject _hudPanel;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private TMP_Text _enemiesLeftText;

    [Header("Pantalla de Inicio")]
    [SerializeField] private GameObject _startPanel;

    [Header("Pantalla de Victoria")]
    [SerializeField] private GameObject _winPanel;

    [Header("Pantalla de Derrota")]
    [SerializeField] private GameObject _losePanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Desactivamos TODOS los paneles inmediatamente en Awake (antes del primer frame)
        // así nunca se ve el parpadeo de los paneles al arrancar
        SetAllPanels(false);
    }

    private void Start()
    {
        // El Slider NO debe ser interactable (es solo visual, no un control del jugador)
        if (_healthSlider != null)
        {
            _healthSlider.interactable = false;
            _healthSlider.value = 1f; // Arranca con vida llena
        }

        if (_enemiesLeftText != null)
        {
            // Buscamos cuántos enemigos hay en la escena para mostrar el número correcto
            int total = FindObjectsOfType<EnemyController>().Length;
            _enemiesLeftText.text = "Piratas: " + total;
        }

        ShowStartScreen();
    }

    // ──────────────────────────────────────────────
    // Pantallas
    // ──────────────────────────────────────────────

    public void ShowStartScreen()
    {
        SetAllPanels(false);
        if (_startPanel != null) _startPanel.SetActive(true);
        Time.timeScale = 0f; // Pausamos el juego en el menú
    }

    public void StartGame()
    {
        SetAllPanels(false);
        if (_hudPanel != null) _hudPanel.SetActive(true);
        Time.timeScale = 1f; // Reanudamos el juego
    }

    public void ShowWinScreen()
    {
        SetAllPanels(false);
        if (_winPanel != null) _winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowLoseScreen()
    {
        SetAllPanels(false);
        if (_losePanel != null) _losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // ──────────────────────────────────────────────
    // HUD Updates
    // ──────────────────────────────────────────────

    // Llamado desde PlayerController cuando cambia la vida
    public void UpdateHealthBar(float current, float max)
    {
        if (_healthSlider != null)
            _healthSlider.value = current / max;
    }

    // Llamado desde GameManager cuando muere un enemigo
    public void UpdateEnemyCount(int remaining)
    {
        if (_enemiesLeftText != null)
            _enemiesLeftText.text = "Piratas: " + remaining;
    }

    // ──────────────────────────────────────────────
    // Botones (asignar en el Inspector de cada botón)
    // ──────────────────────────────────────────────

    public void OnPlayButtonPressed()
    {
        StartGame();
    }

    public void OnRestartButtonPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ──────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────

    private void SetAllPanels(bool active)
    {
        if (_hudPanel != null)    _hudPanel.SetActive(active);
        if (_startPanel != null)  _startPanel.SetActive(active);
        if (_winPanel != null)    _winPanel.SetActive(active);
        if (_losePanel != null)   _losePanel.SetActive(active);
    }
}
