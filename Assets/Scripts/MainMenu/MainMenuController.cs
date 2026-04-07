using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla o Menu Principal do Stellar Fix.
/// Cria toda a UI em código — basta adicionar este script a um GameObject vazio na cena.
/// Requer: Canvas, EventSystem e os sprites em Resources/Sprites/UI/
/// </summary>
public class MainMenuController : MonoBehaviour
{
    // ── Referências (atribuídas em Awake via Resources) ──────────────────────
    private Canvas   _canvas;
    private Image    _logoImg;
    private Button[] _buttons;
    private float    _logoPulseTimer;

    // Configurações visuais
    private static readonly Color COL_TEXT     = new Color(1f, 0.94f, 0.80f, 1f); // #FFF0CC
    private static readonly Color COL_TEXT_DIM = new Color(0.79f, 0.95f, 0.97f, 1f); // #4FC3F7
    private static readonly Color COL_SHADOW   = new Color(0.04f, 0.06f, 0.10f, 0.9f);

    [Header("Scene Names")]
    [SerializeField] private string workshopScene = "Workshop";

    void Awake()
    {
        _canvas = FindObjectOfType<Canvas>();
        if (_canvas == null)
        {
            Debug.LogError("[MainMenu] Canvas não encontrado na cena!");
            return;
        }
        BuildUI();
    }

    void Start()
    {
        AudioManager.Instance?.PlayMainMenuMusic();

        // Anima a entrada do logo e dos botões
        StartCoroutine(IntroAnimation());
    }

    void Update()
    {
        AnimateLogo();
    }

    // ── Construção da UI ─────────────────────────────────────────────────────
    private void BuildUI()
    {
        var rt = _canvas.GetComponent<RectTransform>();

        // Painel central
        var panel = CreatePanel("MainPanel", rt, new Vector2(0, 20), new Vector2(340, 320));

        // Logo
        var logoSprite = Resources.Load<Sprite>("Sprites/UI/logo_stellar_fix");
        var logoGO = CreateUIImage("Logo", panel.transform, logoSprite,
            new Vector2(0, 100), new Vector2(280, 80));
        _logoImg = logoGO.GetComponent<Image>();

        // Subtítulo
        CreateLabel("Subtitle", panel.transform,
            "O MECÂNICO MAIS QUERIDO DA GALÁXIA",
            new Vector2(0, 52), new Vector2(320, 24),
            COL_TEXT_DIM, 11);

        // Divisor
        var div = new GameObject("Divider");
        div.transform.SetParent(panel.transform, false);
        var divImg = div.AddComponent<Image>();
        divImg.color = new Color(0.31f, 0.76f, 0.97f, 0.25f);
        var divRt  = div.GetComponent<RectTransform>();
        divRt.anchoredPosition = new Vector2(0, 28);
        divRt.sizeDelta        = new Vector2(220, 1);

        // Botões
        string[] labels = { "JOGAR", "CONTINUAR", "CONFIGURAÇÕES", "CRÉDITOS" };
        _buttons = new Button[labels.Length];
        for (int i = 0; i < labels.Length; i++)
        {
            float y = 0 - i * 54;
            _buttons[i] = CreateMenuButton(labels[i], panel.transform, new Vector2(0, y), i);
        }

        // Versão
        CreateLabel("Version", rt, "v0.1.0  —  STELLAR FIX",
            new Vector2(0, -rt.sizeDelta.y * 0.5f + 20),
            new Vector2(400, 20),
            new Color(1,1,1,0.2f), 9);
    }

    // ── Botões ────────────────────────────────────────────────────────────────
    private Button CreateMenuButton(string label, Transform parent, Vector2 pos, int index)
    {
        var idleSprite  = Resources.Load<Sprite>("Sprites/UI/btn_idle");
        var hoverSprite = Resources.Load<Sprite>("Sprites/UI/btn_hover");
        var selSprite   = Resources.Load<Sprite>("Sprites/UI/btn_selected");

        var go = new GameObject($"Btn_{label}");
        go.transform.SetParent(parent, false);

        var img = go.AddComponent<Image>();
        img.sprite = idleSprite;
        img.type   = Image.Type.Sliced;

        var rt2 = go.GetComponent<RectTransform>();
        rt2.anchoredPosition = pos;
        rt2.sizeDelta        = new Vector2(200, 36);

        var btn = go.AddComponent<Button>();
        var sb  = btn.spriteState;
        sb.highlightedSprite = hoverSprite;
        sb.pressedSprite     = selSprite;
        sb.selectedSprite    = hoverSprite;
        btn.spriteState      = sb;
        btn.transition       = Selectable.Transition.SpriteSwap;

        // Texto do botão
        CreateLabel("Label", go.transform, label,
            Vector2.zero, new Vector2(180, 36), COL_TEXT, 13);

        // Listener
        int capturedIndex = index;
        btn.onClick.AddListener(() => OnButtonClicked(capturedIndex));

        // SFX no hover (via EventTrigger seria ideal — aqui simulamos via PointerEnter)
        var trigger = go.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        var entry   = new UnityEngine.EventSystems.EventTrigger.Entry
            { eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter };
        entry.callback.AddListener(_ => AudioManager.Instance?.PlayClick());
        trigger.triggers.Add(entry);

        // Começa invisível para a animação de entrada
        var cg = go.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        return btn;
    }

    private void OnButtonClicked(int index)
    {
        AudioManager.Instance?.PlayClick();
        switch (index)
        {
            case 0: StartCoroutine(LoadScene(workshopScene)); break; // Jogar
            case 1: // Continuar (verifica save)
                if (SaveSystem.HasSave())
                    StartCoroutine(LoadScene(workshopScene));
                else
                    FlashButton(index, "SEM SAVE");
                break;
            case 2: Debug.Log("TODO: Configurações"); break;
            case 3: Debug.Log("TODO: Créditos"); break;
        }
    }

    // ── Animações ─────────────────────────────────────────────────────────────
    private IEnumerator IntroAnimation()
    {
        yield return new WaitForSeconds(0.3f);

        // Logo entra de cima
        if (_logoImg != null)
        {
            var logoRt = _logoImg.GetComponent<RectTransform>();
            Vector2 finalPos = logoRt.anchoredPosition;
            logoRt.anchoredPosition = finalPos + Vector2.up * 40f;
            var logoCg = _logoImg.GetComponent<CanvasGroup>() ?? _logoImg.gameObject.AddComponent<CanvasGroup>();
            logoCg.alpha = 0f;
            float t = 0f;
            while (t < 0.6f)
            {
                t += Time.deltaTime;
                float p = t / 0.6f;
                logoCg.alpha = p;
                logoRt.anchoredPosition = Vector2.Lerp(finalPos + Vector2.up * 40f, finalPos, EaseOut(p));
                yield return null;
            }
            logoCg.alpha = 1f;
        }

        yield return new WaitForSeconds(0.15f);

        // Botões entram um a um
        for (int i = 0; i < _buttons.Length; i++)
        {
            var cg = _buttons[i].GetComponent<CanvasGroup>();
            if (cg == null) continue;
            var rt = _buttons[i].GetComponent<RectTransform>();
            Vector2 finalPos = rt.anchoredPosition;
            rt.anchoredPosition = finalPos + Vector2.left * 30f;
            float t = 0f;
            while (t < 0.4f)
            {
                t += Time.deltaTime;
                float p = t / 0.4f;
                cg.alpha = p;
                rt.anchoredPosition = Vector2.Lerp(finalPos + Vector2.left * 30f, finalPos, EaseOut(p));
                yield return null;
            }
            cg.alpha = 1f;
            rt.anchoredPosition = finalPos;
            yield return new WaitForSeconds(0.07f);
        }
    }

    private void AnimateLogo()
    {
        if (_logoImg == null) return;
        _logoPulseTimer += Time.deltaTime;
        // Pulsa suavemente a escala do logo
        float scale = 1f + Mathf.Sin(_logoPulseTimer * 1.2f) * 0.008f;
        _logoImg.transform.localScale = Vector3.one * scale;
    }

    private IEnumerator LoadScene(string sceneName)
    {
        // Fade out simples
        var fadePanel = CreateFadePanel();
        var cg = fadePanel.GetComponent<CanvasGroup>();
        float t = 0f;
        while (t < 0.5f) { t += Time.deltaTime; cg.alpha = t / 0.5f; yield return null; }
        SceneManager.LoadScene(sceneName);
    }

    private void FlashButton(int index, string msg)
    {
        // TODO: mostrar mensagem temporária
        Debug.Log($"[MainMenu] {msg}");
    }

    // ── Helpers de UI ─────────────────────────────────────────────────────────
    private RectTransform CreatePanel(string name, RectTransform parent, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
        return rt;
    }

    private GameObject CreateUIImage(string name, Transform parent, Sprite sprite, Vector2 pos, Vector2 size)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;
        var rt  = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
        return go;
    }

    private GameObject CreateLabel(string name, Transform parent, string text, Vector2 pos, Vector2 size, Color col, int fontSize)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        // Usa TextMeshPro se disponível, senão Unity UI Text legado
#if TMP_PRESENT
        var tmp = go.AddComponent<TMPro.TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = fontSize;
        tmp.color     = col;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
#else
        var txt = go.AddComponent<Text>();
        txt.text      = text;
        txt.fontSize  = fontSize;
        txt.color     = col;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
#endif
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
        return go;
    }

    private GameObject CreateFadePanel()
    {
        var go  = new GameObject("FadePanel");
        go.transform.SetParent(_canvas.transform, false);
        var img = go.AddComponent<Image>();
        img.color = Color.black;
        var rt  = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var cg = go.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        return go;
    }

    private static float EaseOut(float t) => 1f - (1f - t) * (1f - t);
}
