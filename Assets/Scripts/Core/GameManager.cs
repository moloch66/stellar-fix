using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton que persiste entre cenas. Guarda estado global do jogo.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ── Estado do jogo ────────────────────────────────────────────────────────
    public int   CurrentDay    { get; private set; } = 1;
    public float Credits       { get; private set; } = 0f;
    public float Reputation    { get; private set; } = 0f;  // 0–100

    // ── Configurações ─────────────────────────────────────────────────────────
    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene  = "MainMenu";
    [SerializeField] private string workshopScene  = "Workshop";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadGame();
    }

    // ── Economia ──────────────────────────────────────────────────────────────
    public void AddCredits(float amount)
    {
        Credits += amount;
        SaveSystem.Save(this);
        Debug.Log($"[GameManager] Créditos: {Credits} cr");
    }

    public bool SpendCredits(float amount)
    {
        if (Credits < amount) return false;
        Credits -= amount;
        SaveSystem.Save(this);
        return true;
    }

    public void AddReputation(float amount)
    {
        Reputation = Mathf.Clamp(Reputation + amount, 0f, 100f);
        SaveSystem.Save(this);
    }

    // ── Progressão de dia ─────────────────────────────────────────────────────
    public void AdvanceDay()
    {
        CurrentDay++;
        SaveSystem.Save(this);
        Debug.Log($"[GameManager] Dia {CurrentDay} começou.");
    }

    // ── Navegação de cena ─────────────────────────────────────────────────────
    public void GoToWorkshop() => SceneManager.LoadScene(workshopScene);
    public void GoToMainMenu() => SceneManager.LoadScene(mainMenuScene);
    public void QuitGame()     => Application.Quit();

    // ── Persistência ─────────────────────────────────────────────────────────
    public void LoadGame()
    {
        var data = SaveSystem.Load();
        if (data == null) return;
        CurrentDay  = data.currentDay;
        Credits     = data.credits;
        Reputation  = data.reputation;
    }

    // Chamado pelo SaveSystem para serializar
    public SaveData ToSaveData() => new SaveData
    {
        currentDay = CurrentDay,
        credits    = Credits,
        reputation = Reputation,
    };
}
