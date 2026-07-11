import os

import pygame
import random
import math
from PIL import Image
from time import time

RECORD_GIF = True
GIF_LENGTH = 60          # кадров записать
GIF_NAME = "smoke.gif"

frames = []

# ==========================
# CONFIG
# ==========================

WIDTH = 512
HEIGHT = 512
FPS = 15

LIFETIME = 40
SPAWN_COUNT = 50

EMIT_POSITION = (260, 380)

# ==========================
# RANDOMNESS
# ==========================

RANDOM_RADIUS = 0.80      # ±20%
RANDOM_SPEED = 0.3
RANDOM_DIRECTION = 30     # градусов
RANDOM_ALPHA = 0.05
RANDOM_LIFETIME = 0.4

EMISSION_PER_FRAME = 1    # сколько пузырей рождается каждый кадр

# ==========================
# CURVE
# ==========================

class Curve:

    def __init__(self, *keys):
        self.keys = sorted(keys)

    def __call__(self, t):

        t = max(0.0, min(1.0, t))

        if t <= self.keys[0][0]:
            return self.keys[0][1]

        for i in range(len(self.keys)-1):

            ta, va = self.keys[i]
            tb, vb = self.keys[i+1]

            if ta <= t <= tb:

                k = (t-ta)/(tb-ta)
                return va + (vb-va)*k

        return self.keys[-1][1]

# ==========================
# ANIMATION CURVES
# ==========================

radiusCurve = Curve(
    (0.00, 7),
    (0.1, 9),
    (.35, 15),
    (.5, 18),
    (.65, 15),
    (.8, 6),
    (1.00, 0),
)

speedCurve = Curve(
    (0.00, 7),
    (0.25, 5),
    (0.60, 2),
    (1.00, 0)
)

directionCurve = Curve(
    (0.00, 170),
    (0.25, 170),
    (0.30, 170),
    (0.35, 190),
    (0.60, 220),
    (1.00, 270),
)

spreadCurve = Curve(
    (0.00, 5),
    (0.50, 15),
    (1.00, 35)
)

# ==========================
# PARTICLE
# ==========================

class Bubble:

    _circle_cache = {}

    def __init__(self):

        self.life = 0

        self.maxLife = int(
            LIFETIME *
            random.uniform(
                1 - RANDOM_LIFETIME,
                1 + RANDOM_LIFETIME
            )
        )

        self.x = EMIT_POSITION[0]
        self.y = EMIT_POSITION[1]

        self.radiusMul = random.uniform(
            1 - RANDOM_RADIUS,
            1 + RANDOM_RADIUS
        )

        self.speedMul = random.uniform(
            1 - RANDOM_SPEED,
            1 + RANDOM_SPEED
        )

        self.alphaMul = random.uniform(
            1 - RANDOM_ALPHA,
            1 + RANDOM_ALPHA
        )

        self.directionOffset = random.uniform(
            0,
            RANDOM_DIRECTION
        )

    def update(self):

        self.life += 1

        t = self.life / self.maxLife

        direction = directionCurve(t) + self.directionOffset
        speed = speedCurve(t) * self.speedMul

        rad = math.radians(direction)

        self.x += math.cos(rad) * speed
        self.y += math.sin(rad) * speed

    def draw(self, surface):
        t = self.life / self.maxLife

        radius = max(1, int(radiusCurve(t) * self.radiusMul))

        key = (radius, 255)

        img = Bubble._circle_cache.get(key)

        if img is None:
            size = radius * 2 + 2

            img = pygame.Surface((size, size), pygame.SRCALPHA)

            pygame.draw.circle(
                img,
                (255, 255, 255, 255),
                (size // 2, size // 2),
                radius
            )

            Bubble._circle_cache[key] = img

        surface.blit(
            img,
            (
                int(self.x - img.get_width() / 2),
                int(self.y - img.get_height() / 2)
            )
        )

    @property
    def dead(self):
        return self.life >= self.maxLife


# ==========================
# MAIN
# ==========================

pygame.init()

screen = pygame.display.set_mode((WIDTH, HEIGHT))
clock = pygame.time.Clock()

particles_to_spawn = 0
particles = []

running = True

start = time()
for frame_i in range(0, 60*60):
    frame_sin = math.sin(frame_i)
    random.seed(start * frame_sin)

    particles_to_spawn += EMISSION_PER_FRAME

    clock.tick(FPS)

    for _ in range(int(particles_to_spawn)):
        particles.append(Bubble())

    particles_to_spawn -= int(particles_to_spawn)

    screen.fill((0, 0, 0))

    for p in particles[:]:

        p.update()
        p.draw(screen)

        if p.dead:
            particles.remove(p)

    pygame.display.flip()

    if RECORD_GIF and len(frames) < GIF_LENGTH and frame_i % 10 == 0:

        frame = pygame.image.tostring(screen, "RGB")

        img = Image.frombytes(
            "RGB",
            screen.get_size(),
            frame
        )

        frames.append(img)

        if len(frames) == GIF_LENGTH:
            frames[0].save(
                GIF_NAME,
                save_all=True,
                append_images=frames[1:],
                duration=1,
                loop=1
            )

            print("GIF saved.")

            break

pygame.quit()
