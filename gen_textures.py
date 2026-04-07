from PIL import Image, ImageDraw, ImageFilter
import random, math, os

BASE = "c:/Users/jande/OneDrive/\u00c1rea de Trabalho/Claude/StellarFix/Assets/Resources/Sprites"
random.seed(42)

# Palette from GDD
BG     = (11,  15,  26, 255)
WARM   = (232, 133,  61, 255)
AMBER  = (245, 200,  66, 255)
NEON   = ( 79, 195, 247, 255)
METAL  = (140, 155, 171, 255)
SPARKS = (255, 240, 204, 255)

W, H = 320, 180

# ── 1. space_bg.png ──────────────────────────────────────────────────────────
img = Image.new('RGBA', (W, H), BG)
px  = img.load()
cx, cy = W//2, H//3
for y in range(H):
    for x in range(W):
        d = math.sqrt((x-cx)**2 + (y-cy)**2) / max(W,H)
        b = int(max(0, 8 - d*30))
        r,g,bl,a = px[x,y]
        px[x,y] = (min(255,r+b), min(255,g+b), min(255,bl+b+2), 255)
# blue-purple nebula (right side)
for _ in range(2000):
    nx, ny = random.randint(160,310), random.randint(10,100)
    alpha = random.randint(4,18)
    col = random.choice([(30,20,60),(15,30,70),(20,50,80)])
    r,g,bl,_ = px[nx,ny]
    px[nx,ny] = (min(255,r+col[0]),min(255,g+col[1]),min(255,bl+col[2]),255)
# warm nebula (left bottom)
for _ in range(800):
    nx, ny = random.randint(0,120), random.randint(80,170)
    r,g,bl,_ = px[nx,ny]
    px[nx,ny] = (min(255,r+random.randint(5,15)),min(255,g+random.randint(0,5)),bl,255)
img.save(f"{BASE}/Background/space_bg.png")
print("OK space_bg.png")

# ── 2. stars_far.png ─────────────────────────────────────────────────────────
img = Image.new('RGBA', (W, H), (0,0,0,0))
px  = img.load()
star_cols = [(255,255,255),(220,230,255),(255,240,200),(180,210,255)]
for _ in range(140):
    sx, sy = random.randint(0,W-1), random.randint(0,H-1)
    col = random.choice(star_cols)
    px[sx,sy] = col + (random.randint(50,160),)
img.save(f"{BASE}/Background/stars_far.png")
print("OK stars_far.png")

# ── 3. stars_near.png ────────────────────────────────────────────────────────
img = Image.new('RGBA', (W, H), (0,0,0,0))
px  = img.load()
for _ in range(45):
    sx, sy = random.randint(0,W-1), random.randint(0,H-1)
    col = random.choice(star_cols)
    a2  = random.randint(140,240)
    px[sx,sy] = col + (a2,)
    if random.random() < 0.3:
        for dd in [(-1,0),(1,0),(0,-1),(0,1)]:
            xx, yy = sx+dd[0], sy+dd[1]
            if 0<=xx<W and 0<=yy<H:
                px[xx,yy] = col + (a2//3,)
img.save(f"{BASE}/Background/stars_near.png")
print("OK stars_near.png")

# ── 4. workshop_exterior.png ─────────────────────────────────────────────────
img  = Image.new('RGBA', (W, H), (0,0,0,0))
draw = ImageDraw.Draw(img)
METAL_D  = ( 90,100,115,255)
METAL_L  = (160,172,188,255)
RIVET    = ( 60, 68, 80,255)
PANEL_MID= (110,122,138,255)
bx, by, bw, bh = 60, 60, 200, 100
draw.rectangle([bx, by, bx+bw, by+bh], fill=METAL_D)
draw.rectangle([bx+4, by-8, bx+bw-4, by+4], fill=METAL_L)
for i in range(4):
    px_x = bx + 8 + i*48
    draw.rectangle([px_x, by+6, px_x+40, by+bh-6], fill=PANEL_MID)
for i in range(8):
    rx = bx + 10 + i*24
    draw.ellipse([rx-2, by+2, rx+2, by+6], fill=RIVET)
# window glow
wx, wy = bx+80, by+35
for r2,col,a3 in [(28,(255,140,40),50),(18,(255,170,80),90),(10,(255,200,120),150),(5,(255,230,160),210)]:
    layer = Image.new('RGBA',(W,H),(0,0,0,0))
    ImageDraw.Draw(layer).ellipse([wx-r2,wy-r2,wx+r2,wy+r2], fill=col+(a3,))
    img = Image.alpha_composite(img, layer)
# portholes
draw = ImageDraw.Draw(img)
for wx2, wy2 in [(bx+18,by+38),(bx+bw-22,by+38),(bx+18,by+62),(bx+bw-22,by+65)]:
    draw.ellipse([wx2-7,wy2-5,wx2+7,wy2+5], fill=METAL_D)
    draw.ellipse([wx2-4,wy2-3,wx2+4,wy2+3], fill=(80,50,20,200))
    draw.ellipse([wx2-2,wy2-1,wx2+2,wy2+1], fill=(255,180,60,200))
# antenna
ax = bx+bw-22
draw.rectangle([ax,by-26,ax+2,by-8], fill=METAL_L)
draw.ellipse([ax-3,by-30,ax+5,by-24], fill=NEON[:3]+(200,))
# struts
for sx2 in [bx+14, bx+bw-18]:
    draw.rectangle([sx2-3,by+bh,sx2+3,by+bh+14], fill=METAL_D)
    draw.rectangle([sx2-6,by+bh+12,sx2+6,by+bh+16], fill=METAL_L)
img.save(f"{BASE}/Background/workshop_exterior.png")
print("OK workshop_exterior.png")

# ── 5. ui_panel.png  192×60 ──────────────────────────────────────────────────
PW, PH = 192, 60
img  = Image.new('RGBA', (PW, PH), (0,0,0,0))
draw = ImageDraw.Draw(img)
draw.rectangle([2,2,PW-3,PH-3], fill=(26,32,48,210))
for i in range(PW):
    img.putpixel((i,0),     NEON[:3]+(160,))
    img.putpixel((i,PH-1),  NEON[:3]+(60,))
for i in range(PH):
    img.putpixel((0,i),     NEON[:3]+(160,))
    img.putpixel((PW-1,i),  NEON[:3]+(60,))
img.save(f"{BASE}/UI/ui_panel.png")
print("OK ui_panel.png")

# ── 6. btn_idle.png  128×28 ──────────────────────────────────────────────────
BW2, BH2 = 128, 28
img  = Image.new('RGBA', (BW2, BH2), (0,0,0,0))
draw = ImageDraw.Draw(img)
draw.rectangle([1,1,BW2-2,BH2-2], fill=(20,28,46,230))
for i in range(BW2):
    img.putpixel((i,0),    NEON[:3]+(120,))
    img.putpixel((i,BH2-1),NEON[:3]+(50,))
for i in range(BH2):
    img.putpixel((0,i),    NEON[:3]+(100,))
    img.putpixel((BW2-1,i),NEON[:3]+(50,))
img.save(f"{BASE}/UI/btn_idle.png")
print("OK btn_idle.png")

# ── 7. btn_hover.png  128×28 ─────────────────────────────────────────────────
img  = Image.new('RGBA', (BW2, BH2), (0,0,0,0))
draw = ImageDraw.Draw(img)
draw.rectangle([1,1,BW2-2,BH2-2], fill=(35,50,80,240))
draw.rectangle([2,2,BW2-3,BH2-3], fill=AMBER[:3]+(20,))
for i in range(BW2):
    img.putpixel((i,0),    AMBER[:3]+(180,))
    img.putpixel((i,BH2-1),AMBER[:3]+(80,))
for i in range(BH2):
    img.putpixel((0,i),    AMBER[:3]+(160,))
    img.putpixel((BW2-1,i),AMBER[:3]+(80,))
img.save(f"{BASE}/UI/btn_hover.png")
print("OK btn_hover.png")

# ── 8. btn_selected.png  128×28 ──────────────────────────────────────────────
img  = Image.new('RGBA', (BW2, BH2), (0,0,0,0))
draw = ImageDraw.Draw(img)
draw.rectangle([1,1,BW2-2,BH2-2], fill=(50,38,10,250))
draw.rectangle([2,2,BW2-3,BH2-3], fill=WARM[:3]+(30,))
for i in range(BW2):
    img.putpixel((i,0),    WARM[:3]+(200,))
    img.putpixel((i,BH2-1),WARM[:3]+(100,))
for i in range(BH2):
    img.putpixel((0,i),    WARM[:3]+(180,))
    img.putpixel((BW2-1,i),WARM[:3]+(100,))
img.save(f"{BASE}/UI/btn_selected.png")
print("OK btn_selected.png")

# ── 9. logo_stellar_fix.png ──────────────────────────────────────────────────
FONT = {
'S':[[0,1,1,1,0],[1,0,0,0,0],[1,0,0,0,0],[0,1,1,1,0],[0,0,0,0,1],[0,0,0,0,1],[0,1,1,1,0]],
'T':[[1,1,1,1,1],[0,0,1,0,0],[0,0,1,0,0],[0,0,1,0,0],[0,0,1,0,0],[0,0,1,0,0],[0,0,1,0,0]],
'E':[[1,1,1,1,1],[1,0,0,0,0],[1,0,0,0,0],[1,1,1,1,0],[1,0,0,0,0],[1,0,0,0,0],[1,1,1,1,1]],
'L':[[1,0,0,0,0],[1,0,0,0,0],[1,0,0,0,0],[1,0,0,0,0],[1,0,0,0,0],[1,0,0,0,0],[1,1,1,1,1]],
'A':[[0,0,1,0,0],[0,1,0,1,0],[1,0,0,0,1],[1,0,0,0,1],[1,1,1,1,1],[1,0,0,0,1],[1,0,0,0,1]],
'R':[[1,1,1,1,0],[1,0,0,0,1],[1,0,0,0,1],[1,1,1,1,0],[1,0,1,0,0],[1,0,0,1,0],[1,0,0,0,1]],
'F':[[1,1,1,1,1],[1,0,0,0,0],[1,0,0,0,0],[1,1,1,1,0],[1,0,0,0,0],[1,0,0,0,0],[1,0,0,0,0]],
'I':[[0,1,1,1,0],[0,0,1,0,0],[0,0,1,0,0],[0,0,1,0,0],[0,0,1,0,0],[0,0,1,0,0],[0,1,1,1,0]],
'X':[[1,0,0,0,1],[0,1,0,1,0],[0,0,1,0,0],[0,0,1,0,0],[0,0,1,0,0],[0,1,0,1,0],[1,0,0,0,1]],
' ':[[0]*5]*7,
}
SC  = 4
GAP = 2
t1, t2 = "STELLAR", "FIX"
cw = 5*SC + GAP
def tw(t): return len(t)*cw - GAP
LW  = max(tw(t1), tw(t2)) + 20
LH  = 7*SC*2 + 10*SC
img  = Image.new('RGBA', (LW, LH), (0,0,0,0))
draw = ImageDraw.Draw(img)

def draw_text(t, ox, oy, color, shadow=True):
    for ci, ch in enumerate(t):
        bitmap = FONT.get(ch, FONT[' '])
        for row, line in enumerate(bitmap):
            for col, bit in enumerate(line):
                if bit:
                    rx = ox + ci*cw + col*SC
                    ry = oy + row*SC
                    if shadow:
                        draw.rectangle([rx+2,ry+2,rx+SC,ry+SC], fill=(10,15,30,160))
                    draw.rectangle([rx,ry,rx+SC-1,ry+SC-1], fill=color)

ox1 = (LW - tw(t1))//2
ox2 = (LW - tw(t2))//2
draw_text(t1, ox1, 4, SPARKS[:3]+(255,))
draw_text(t2, ox2, 4 + 7*SC + 6*SC, NEON[:3]+(255,))

glow = img.copy().filter(ImageFilter.GaussianBlur(3))
final = Image.alpha_composite(glow, img)
final.save(f"{BASE}/UI/logo_stellar_fix.png")
print("OK logo_stellar_fix.png")

# ── 10. ship_shuttle.png  128×128 ────────────────────────────────────────────
SW = SH = 128
img  = Image.new('RGBA', (SW, SH), (0,0,0,0))
draw = ImageDraw.Draw(img)
draw.rounded_rectangle([30,44,98,90], radius=10, fill=METAL[:3]+(255,))
draw.ellipse([18,52,48,80], fill=(160,172,188,255))
draw.ellipse([22,56,42,76], fill=(40,60,90,240))
draw.ellipse([24,58,36,68], fill=(100,180,240,200))
draw.rectangle([88,52,106,84], fill=(100,112,128,255))
draw.rectangle([104,56,116,64], fill=(80,90,105,255))
draw.rectangle([104,68,116,76], fill=(80,90,105,255))
draw.polygon([(56,44),(72,44),(66,28),(62,28)], fill=(120,132,148,255))
draw.polygon([(56,90),(72,90),(66,106),(62,106)], fill=(120,132,148,255))
draw.line([(36,44),(36,90)], fill=(100,110,125,200), width=1)
draw.line([(60,44),(60,90)], fill=(100,110,125,200), width=1)
draw.line([(82,44),(82,90)], fill=(100,110,125,200), width=1)
draw.rectangle([36,62,82,66], fill=AMBER[:3]+(180,))
for r2,a2 in [(14,30),(10,50),(6,90),(3,160)]:
    draw.ellipse([114-r2,68-r2+2,114+r2,68+r2+2], fill=(232,133,61,a2))
draw.ellipse([60,32,68,40], fill=NEON[:3]+(200,))
draw.ellipse([62,34,66,38], fill=(200,240,255,255))
img.save(f"{BASE}/Ships/ship_shuttle.png")
print("OK ship_shuttle.png")

# ── 11. particle_spark.png  8×8 ──────────────────────────────────────────────
img = Image.new('RGBA', (8,8), (0,0,0,0))
for dx,dy in [(3,0),(4,0),(3,1),(4,1),(2,2),(5,2),(1,3),(6,3),(1,4),(6,4),(2,5),(5,5),(3,6),(4,6)]:
    img.putpixel((dx,dy), SPARKS)
for dx,dy in [(3,3),(4,3),(3,4),(4,4)]:
    img.putpixel((dx,dy), (255,255,255,255))
img.save(f"{BASE}/UI/particle_spark.png")
print("OK particle_spark.png")

# ── 12. star_sprite.png  4×4 ─────────────────────────────────────────────────
img = Image.new('RGBA', (4,4), (0,0,0,0))
img.putpixel((1,1),(255,255,255,255)); img.putpixel((2,1),(255,255,255,255))
img.putpixel((1,2),(255,255,255,255)); img.putpixel((2,2),(255,255,255,255))
for dx,dy in [(1,0),(2,0),(0,1),(3,1),(0,2),(3,2),(1,3),(2,3)]:
    img.putpixel((dx,dy),(255,255,255,100))
img.save(f"{BASE}/UI/star_sprite.png")
print("OK star_sprite.png")

print("\nAll textures generated!")
