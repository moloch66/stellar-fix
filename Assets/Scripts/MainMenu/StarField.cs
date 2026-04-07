using UnityEngine;

/// <summary>
/// Gera e anima 3 camadas de estrelas em parallax.
/// Cria os sprites proceduralmente — sem precisar arrastar nada no Inspector.
/// </summary>
public class StarField : MonoBehaviour
{
    [System.Serializable]
    public class StarLayer
    {
        public string  name        = "Stars";
        public int     count       = 60;
        public float   speed       = 0.01f;   // unidades/seg
        public float   minSize     = 0.03f;
        public float   maxSize     = 0.08f;
        [Range(0f,1f)]
        public float   minAlpha    = 0.3f;
        [Range(0f,1f)]
        public float   maxAlpha    = 1f;
        public Color   tint        = Color.white;
    }

    [Header("Layers (far → near)")]
    [SerializeField] private StarLayer[] layers = new StarLayer[]
    {
        new StarLayer { name="Far",    count=80,  speed=0.008f, minSize=0.02f, maxSize=0.05f, minAlpha=0.2f, maxAlpha=0.6f, tint=new Color(0.87f,0.94f,1f) },
        new StarLayer { name="Mid",    count=45,  speed=0.018f, minSize=0.04f, maxSize=0.08f, minAlpha=0.5f, maxAlpha=0.85f, tint=Color.white },
        new StarLayer { name="Near",   count=20,  speed=0.032f, minSize=0.06f, maxSize=0.12f, minAlpha=0.7f, maxAlpha=1f,   tint=new Color(1f,0.97f,0.87f) },
    };

    [Header("Bounds (world units)")]
    [SerializeField] private float boundsW = 20f;
    [SerializeField] private float boundsH = 12f;

    private GameObject[][] _starObjects;
    private Sprite         _starSprite;

    void Awake()
    {
        _starSprite = CreateStarSprite();
        _starObjects = new GameObject[layers.Length][];
        for (int i = 0; i < layers.Length; i++)
            _starObjects[i] = SpawnLayer(i, layers[i]);
    }

    void Update()
    {
        // Drift lento: move cada camada e faz wrap quando sai da tela
        for (int i = 0; i < layers.Length; i++)
        {
            float dx = layers[i].speed * Time.deltaTime;
            foreach (var star in _starObjects[i])
            {
                star.transform.position += Vector3.left * dx;
                var p = star.transform.position;
                if (p.x < -boundsW * 0.5f)
                    star.transform.position = new Vector3(boundsW * 0.5f, p.y, p.z);
            }
        }
    }

    // ── Criação ───────────────────────────────────────────────────────────────
    private GameObject[] SpawnLayer(int layerIdx, StarLayer def)
    {
        var parent = new GameObject(def.name);
        parent.transform.SetParent(transform);

        var stars = new GameObject[def.count];
        for (int j = 0; j < def.count; j++)
        {
            var go = new GameObject($"star_{j}");
            go.transform.SetParent(parent.transform);

            float x = Random.Range(-boundsW * 0.5f, boundsW * 0.5f);
            float y = Random.Range(-boundsH * 0.5f, boundsH * 0.5f);
            go.transform.position = new Vector3(x, y, layerIdx * 0.1f);

            float sz = Random.Range(def.minSize, def.maxSize);
            go.transform.localScale = Vector3.one * sz;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = _starSprite;
            sr.sortingOrder = -10 + layerIdx;

            Color c = def.tint;
            c.a       = Random.Range(def.minAlpha, def.maxAlpha);
            sr.color  = c;

            stars[j] = go;
        }
        return stars;
    }

    /// <summary>Cria sprite de estrela 4×4 em código, sem asset externo.</summary>
    private Sprite CreateStarSprite()
    {
        var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode   = TextureWrapMode.Clamp,
        };

        Color clear = Color.clear;
        Color full  = Color.white;
        Color half  = new Color(1,1,1,0.4f);

        // Padrão de cruz
        Color[] pixels = new Color[16];
        for (int k = 0; k < 16; k++) pixels[k] = clear;
        pixels[5] = full; pixels[6] = full;  // row 1 center
        pixels[9] = full; pixels[10] = full; // row 2 center
        pixels[1] = half; pixels[2] = half;  // top
        pixels[4] = half; pixels[7] = half;  // sides
        pixels[8] = half; pixels[11]= half;
        pixels[13]= half; pixels[14]= half;  // bottom

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0,0,4,4), new Vector2(0.5f,0.5f), 4f);
    }
}
