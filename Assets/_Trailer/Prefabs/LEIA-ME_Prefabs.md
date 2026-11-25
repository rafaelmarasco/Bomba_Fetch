# Como criar os Prefabs do Modo Trailer

Os prefabs precisam ser criados manualmente no Unity porque contêm componentes
do Cinemachine que o Editor registra com GUIDs internos.
Siga os passos abaixo — cada um leva menos de 1 minuto.

---

## 1. TrailerManager (orquestrador — adicionar na cena de jogo)

1. Hierarchy > Create Empty → renomeie para **TrailerManager**
2. Add Component → `Trailer Mode Controller`
3. Add Component → `Trailer Fly Cam`
4. Add Component → `Trailer Invincibility`
5. No campo **Camera Manager** do `TrailerModeController`, arraste o GameObject
   que tem o `CameraManager` da sua cena.
6. Os campos **Fly Cam** e **Invincibility** são preenchidos automaticamente
   (GetComponentInChildren), mas pode arrastar manualmente se quiser.
7. Arraste o GameObject para `Assets/_Trailer/Prefabs/` para salvar como prefab.

---

## 2. TC_OrbitPlayer (câmera de órbita)

1. Hierarchy > Create Empty → **TC_OrbitPlayer**
2. Add Component → `Cinemachine Camera` (Cinemachine 3.x)
3. Add Component → `Trailer Camera Orbit`
4. No `CinemachineCamera`, defina **Priority = 20** (mais alto que a câmera do jogo)
5. O campo **Target** do `TrailerCameraOrbit` é preenchido automaticamente,
   mas você pode travar num player específico.
6. Ajuste **Orbit Speed** (padrão 8°/s), **Orbit Height** (3 m), **Orbit Radius** (6 m).
7. Desative o GameObject quando não estiver gravando com ele.
8. Salve como prefab em `Assets/_Trailer/Prefabs/`.

---

## 3. TC_PushIn (câmera de aproximação)

1. Hierarchy > Create Empty → **TC_PushIn**
2. Add Component → `Cinemachine Camera`
3. Add Component → `Trailer Camera Push-In`
4. Priority = 20
5. Posicione longe do player (10–15 m) apontando para ele.
6. Ajuste **Approach Speed** (padrão 2), **Stop Distance** (2.5 m).
7. Salve como prefab.

---

## 4. TC_LowAngle (câmera de ângulo baixo)

1. Hierarchy > Create Empty → **TC_LowAngle**
2. Add Component → `Cinemachine Camera`
3. Add Component → `Trailer Camera Low Angle`
4. Priority = 20
5. Offset padrão: (0, 0.4, -4) — fica atrás e bem rente ao chão.
   Ajuste conforme o layout da sua fase.
6. Salve como prefab.

---

## 5. TC_DollyPan (câmera em trilho)

1. Hierarchy > Create Empty → **TC_DollyPan**
2. Add Component → `Cinemachine Camera`
3. Add Component → `Trailer Camera Dolly`
4. Priority = 20
5. Crie GameObjects filhos chamados **WP_0**, **WP_1**, **WP_2**...
   e posicione-os ao longo da panorâmica desejada.
6. (Opcional) Arraste o Transform do player em **Look At Target**
   para a câmera olhar para ele enquanto percorre o trilho.
7. Waypoints aparecem como esferas amarelas e linhas azuis no Scene View.
8. Salve como prefab.

---

## Dica: controlar qual câmera está ativa

- Cada `CinemachineCamera` tem um campo **Priority**.
- A câmera com maior prioridade assume automaticamente o `Cinemachine Brain`
  que está na Main Camera.
- Para alternar entre as câmeras do trailer durante a gravação, basta
  ativar/desativar os GameObjects no Hierarchy (ou ajustar a prioridade).
- A câmera do jogo (`CameraManager`) tem prioridade padrão — coloque as
  câmeras do trailer com **Priority = 20** ou mais para garantir precedência.
