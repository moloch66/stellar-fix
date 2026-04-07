# Stellar Fix — Unity Project Setup

## Requisitos

- **Unity 2022.3 LTS** (testado com 2022.3.20f1)
- **Universal Render Pipeline** não necessário — usa o pipeline Built-in padrão
- **TextMeshPro** opcional (o código detecta via `#if TMP_PRESENT`)

---

## Como importar no Unity

### 1. Abrir o projeto

1. Abra o **Unity Hub**
2. Clique em **Add → Add project from disk**
3. Selecione a pasta `StellarFix/`
4. Aguarde a importação (primeira vez pode demorar alguns minutos)

### 2. Configurar a cena MainMenu

1. No **Project panel**, vá em `Assets/Scenes/`
2. Abra `MainMenu.unity` com duplo clique

A cena já contém os seguintes GameObjects prontos:

| GameObject | Script |
|---|---|
| Main Camera | Camera orthográfica (ortho size 5) |
| EventSystem | StandaloneInputModule |
| Canvas | CanvasScaler (1920×1080, ScaleWithScreen) |
| GameManager | `GameManager.cs` — singleton, DontDestroyOnLoad |
| AudioManager | `AudioManager.cs` — música + SFX com crossfade |
| StarField | `StarField.cs` — 3 camadas de estrelas procedurais |
| BackgroundCompositor | `BackgroundCompositor.cs` — monta layers do fundo via Resources |
| MainMenuController | `MainMenuController.cs` — constrói toda a UI em código |

### 3. Atribuir os scripts aos GameObjects (se necessário)

Se o Unity não reconhecer os scripts automaticamente (GUIDs divergentes), atribua manualmente:

- **GameManager** → adicione `Scripts/Core/GameManager.cs`
- **AudioManager** → adicione `Scripts/Core/AudioManager.cs`
- **StarField** → adicione `Scripts/MainMenu/StarField.cs`
- **BackgroundCompositor** → adicione `Scripts/MainMenu/BackgroundCompositor.cs`
- **MainMenuController** → adicione `Scripts/MainMenu/MainMenuController.cs`

### 4. Assets de áudio (opcional)

O `AudioManager` tem slots para:
- `Music Main Menu` — trilha ambiente do menu (loop)
- `Music Workshop` — trilha da oficina (loop)
- `Sfx Button Click` — clique de botão
- `Sfx Sparkle` — sparkle/brilho
- `Sfx Repair Complete` — reparo concluído

Importe arquivos `.ogg` ou `.wav` e arraste para os campos no Inspector do `AudioManager`.

### 5. TextMeshPro (recomendado)

Para ativar textos com TMP:

1. **Window → TextMeshPro → Import TMP Essential Resources**
2. No menu **Project Settings → Player → Scripting Define Symbols** adicione:
   ```
   TMP_PRESENT
   ```

Sem isso, os textos usam o `UnityEngine.UI.Text` legado (funcional, mas menos bonito).

---

## Estrutura de pastas

```
StellarFix/
├── Assets/
│   ├── Resources/
│   │   └── Sprites/
│   │       ├── Background/
│   │       │   ├── space_bg.png          ← fundo espacial (320×180)
│   │       │   ├── stars_far.png         ← estrelas distantes (parallax)
│   │       │   ├── stars_near.png        ← estrelas próximas (parallax)
│   │       │   └── workshop_exterior.png ← estação espacial (320×180)
│   │       ├── Ships/
│   │       │   └── ship_shuttle.png      ← nave shuttle (128×128)
│   │       └── UI/
│   │           ├── logo_stellar_fix.png  ← logo do jogo
│   │           ├── btn_idle.png          ← botão estado normal
│   │           ├── btn_hover.png         ← botão hover
│   │           ├── btn_selected.png      ← botão pressionado
│   │           ├── ui_panel.png          ← painel de UI
│   │           ├── particle_spark.png    ← partícula faísca
│   │           └── star_sprite.png       ← sprite de estrela
│   ├── Scenes/
│   │   └── MainMenu.unity
│   └── Scripts/
│       ├── Core/
│       │   ├── GameManager.cs
│       │   ├── AudioManager.cs
│       │   └── SaveSystem.cs
│       └── MainMenu/
│           ├── BackgroundCompositor.cs
│           ├── MainMenuController.cs
│           └── StarField.cs
└── ProjectSettings/
    ├── ProjectVersion.txt      ← Unity 2022.3.20f1
    ├── ProjectSettings.asset
    ├── EditorBuildSettings.asset
    └── InputManager.asset
```

---

## Como funciona o Menu Principal

1. **BackgroundCompositor** carrega os sprites via `Resources.Load<Sprite>()` e cria GameObjects com SpriteRenderer em runtime — sem necessidade de arrastar nada no Inspector
2. **StarField** gera 145 estrelas procedurais (80 distantes + 45 médias + 20 próximas) com drift horizontal e wrap automático
3. **MainMenuController** encontra o Canvas na cena e constrói toda a UI (logo, botões, divisor, versão) em código puro no `Awake()`
4. A animação de entrada (`IntroAnimation`) desliza o logo de cima e os botões da esquerda usando coroutines com easing quadrático

---

## Próximas cenas a criar

- `Workshop` — oficina principal (tilemap pixel art, mecânica de reparo)
- `Dialogue` — sistema de diálogo com clientes
- `Inventory` — inventário de peças e ferramentas
