using UnityEngine;

/// <summary>
/// Monta o fundo do menu principal em código:
///   Layer 0 — space_bg (estático)
///   Layer 1 — stars_far  (parallax lento)
///   Layer 2 — stars_near (parallax rápido)
///   Layer 3 — workshop_exterior (posicionado no centro-baixo, levemente oscilando)
/// Basta adicionar à câmera principal ou a um GameObject vazio.
/// </summary>
[DefaultExecutionOrder(-100)]
public class BackgroundCompositor : MonoBehaviour
{
    [Header("Parallax")]
    [SerializeField] private float starsFarSpeed  = 0.006f;
    [SerializeField] private float starsNearSpeed = 0.016f;

    [Header("Workshop Float")]
    [SerializeField] private float floatAmplitude = 0.06f;
    [SerializeField] private float floatFrequency = 0.4f;

    private SpriteRenderer _srBg;
    private SpriteRenderer _srFar;
    private SpriteRenderer _srNear;
    private SpriteRenderer _srWorkshop;

    private float _workshopBaseY;
    private float _time;

    void Awake()
    {
        _srBg       = SpawnBg("BG_Space",    "Sprites/Background/space_bg",           -5,  new Vector2(0, 0),     new Vector3(10f, 5.625f, 1f));
        _srFar      = SpawnBg("BG_StarsFar", "Sprites/Background/stars_far",          -4,  new Vector2(0, 0),     new Vector3(10f, 5.625f, 1f));
        _srNear     = SpawnBg("BG_StarsNear","Sprites/Background/stars_near",         -3,  new Vector2(0, 0),     new Vector3(10f, 5.625f, 1f));
        _srWorkshop = SpawnBg("Workshop_Ext","Sprites/Background/workshop_exterior",  -2,  new Vector2(0, -0.5f), new Vector3(10f, 5.625f, 1f));

        if (_srWorkshop != null)
            _workshopBaseY = _srWorkshop.transform.position.y;
    }

    void Update()
    {
        _time += Time.deltaTime;

        // Parallax das estrelas (loop horizontal simples)
        ShiftX(_srFar,  starsFarSpeed  * Time.deltaTime);
        ShiftX(_srNear, starsNearSpeed * Time.deltaTime);

        // Float suave da oficina
        if (_srWorkshop != null)
        {
            float newY = _workshopBaseY + Mathf.Sin(_time * floatFrequency * Mathf.PI * 2f) * floatAmplitude;
            var p = _srWorkshop.transform.position;
            _srWorkshop.transform.position = new Vector3(p.x, newY, p.z);
        }
    }

    private void ShiftX(SpriteRenderer sr, float dx)
    {
        if (sr == null) return;
        var p = sr.transform.position;
        float newX = p.x - dx;
        // Wrap: quando shift excede metade da largura, reinicia (loop visual)
        if (newX < -0.5f) newX += 0.5f;
        sr.transform.position = new Vector3(newX, p.y, p.z);
    }

    private SpriteRenderer SpawnBg(string goName, string spritePath, int sortOrder, Vector2 offset, Vector3 scale)
    {
        var sprite = Resources.Load<Sprite>(spritePath);
        if (sprite == null)
        {
            Debug.LogWarning($"[BackgroundCompositor] Sprite não encontrado: {spritePath}");
            return null;
        }

        var go = new GameObject(goName);
        go.transform.SetParent(transform);
        go.transform.localPosition = new Vector3(offset.x, offset.y, sortOrder * 0.1f);
        go.transform.localScale    = scale;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite       = sprite;
        sr.sortingOrder = sortOrder;

        return sr;
    }
}
