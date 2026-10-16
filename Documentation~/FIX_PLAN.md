# IMG2ETC2 — аудит и план исправлений

Дата аудита: 2026-10-16 · Ревизия: `f58d974` (main, тег `v1.7.0`)

> Файл лежит в `Documentation~/` — Unity игнорирует папки с `~` на конце, поэтому `.meta` не нужен и в пакет он не попадает.

Приоритеты: 🔴 критично (портит проект пользователя) · 🟠 баг · 🟡 документация/упаковка · 🟢 улучшение.

---

## Этап 1 — Безопасность данных 🔴

### 1.1. Сохранение GUID при конвертации JPG → PNG
- **Где:** `Editor/ImageProcessor.cs:37-47` (`SaveAsPng`)
- **Проблема:** `File.Delete(model.FilePath)` удаляет `.jpg`, но `.jpg.meta` остаётся. Unity создаёт новый `.meta` для `.png` с новым GUID → рвутся ссылки из материалов, префабов, сцен; теряются настройки импорта (Sprite mode, slicing, compression).
- **Решение:** перед записью PNG переименовать ассет через `AssetDatabase.MoveAsset(old, new)` (GUID и meta сохраняются), затем перезаписать содержимое и вызвать `AssetDatabase.ImportAsset`. Нужен хелпер конвертации абсолютного пути в `Assets/...`.
- **Критерий готовности:** после конвертации JPG GUID в `.png.meta` совпадает с исходным, ссылки на текстуру в тестовой сцене сохраняются.

### 1.2. Padding вместо растягивания
- **Где:** `Editor/ImageUtils.cs:37-46` (`ResizeTexture`), `Editor/ImageProcessor.cs:24-35`
- **Проблема:** `Graphics.Blit` масштабирует изображение с билинейной фильтрацией: размытие даже при +1..3 px, смещение sprite rects (Multiple), 9-slice borders и пиксельных pivot. README при этом обещает «transparent padding».
- **Решение:**
  - Режим **Up**: копировать исходные пиксели в новую текстуру через `GetPixels32`/`SetPixels32` без ресемплинга, оставшиеся пиксели — `Color32(0,0,0,0)`. Якорь — левый нижний угол (система координат Unity), тогда sprite rects остаются валидными.
  - Режим **Down**: обрезка (crop) вместо сжатия; с предупреждением, если sprite rects выходят за новые границы.
  - Опционально: выбор якоря (Bottom-Left / Center) с пересчётом pivot/rects в `TextureImporter.spritesheet` (или `ISpriteEditorDataProvider`).
- **Критерий готовности:** пиксели исходной области байт-в-байт совпадают с оригиналом; спрайтшит после обработки нарезан корректно.

### 1.3. Ограничение путей проектом
- **Где:** `Editor/IMG2ETC2Window.cs:35, 103-111`
- **Проблема:** можно выбрать любую папку на диске — инструмент перезапишет файлы вне проекта; также `Packages/` из кэша (read-only).
- **Решение:** принимать только пути внутри `Application.dataPath`; при выборе другого пути показывать ошибку (`EditorGUILayout.HelpBox`) и блокировать кнопку Resize.

### 1.4. Подтверждение перед изменением
- **Где:** `Editor/IMG2ETC2Window.cs:51-52`
- **Решение:** `EditorUtility.DisplayDialog` с количеством файлов и напоминанием о бэкапе/коммите. Опционально — галочка «Создать бэкап» (копия в `Library/IMG2ETC2Backup` или рядом с `~`).

---

## Этап 2 — Баги 🟠

| # | Проблема | Где | Решение |
|---|----------|-----|---------|
| 2.1 | Полный ре-скан и декодирование на каждое нажатие клавиши в поле пути; на неполном пути — `DirectoryNotFoundException` в консоль | `IMG2ETC2Window.cs:34-39` | Вынести `TextField` из `ChangeCheck` или использовать `EditorGUILayout.DelayedTextField`; проверять `Directory.Exists` перед сканом |
| 2.2 | При открытии окна список пуст; состояние теряется после domain reload | `IMG2ETC2Window.cs:13-23` | `[SerializeField]` на настройки, сохранение в `EditorPrefs`, `LoadImages()` в `OnEnable` при валидном пути |
| 2.3 | Down на текстурах 1–3 px → размер 0 → исключение | `ImageUtils.cs:30-35` | `Math.Max(4, …)` для Down или пропуск файла с предупреждением |
| 2.4 | Нет `try/finally`: исключение оставляет висящий progress bar и утёкшие текстуры; один битый файл роняет весь скан | `ImageProcessor.cs:10-35`, `ImageLoader.cs:14-19` | `try/finally` для `ClearProgress` и `DestroyImmediate`; `try/catch` на каждый файл с `Debug.LogWarning` |
| 2.5 | Progress bar: путь файла передаётся как заголовок, счётчик отстаёт на 1, нет отмены | `ImageProcessor.cs:18`, `GUIUtils.cs:24-29` | Заголовок = `UtilityName`, info = путь, `i + 1`; `DisplayCancelableProgressBar` |
| 2.6 | Нет реимпорта после записи — размеры в инспекторе устаревают | `ImageProcessor.cs` | `AssetDatabase.StartAssetEditing()` / `StopAssetEditing()` + `ImportAsset` по каждому файлу (или `Refresh` в конце) |
| 2.7 | Регистрозависимая проверка расширения: `.PNG`, `.JPG` пропускаются | `ImageLoader.cs:24-25` | `Path.GetExtension(path).ToLowerInvariant()` + `HashSet` |
| 2.8 | Потери при перекодировании: 16-bit PNG → 8-bit, теряется цветовой профиль; возможны гамма-сдвиги в Linear-проектах через RenderTexture | `ImageUtils.cs:37-53` | Уходит вместе с 1.2 (без RenderTexture); для 16-bit — предупреждать или пропускать |

---

## Этап 3 — Упаковка и релиз 🟡

### 3.1. Asmdef
- **Где:** `Editor/LLarean.IMG2ETC2.Editor.asmdef`
- **Проблема:** `"name": "LLarean.IMG2ETC2.Runtime"` при имени файла `.Editor`; нет `includePlatforms` → сборка попадает в player build.
- **Решение:**
  ```json
  {
    "name": "LLarean.IMG2ETC2.Editor",
    "rootNamespace": "LLarean.IMG2ETC2",
    "includePlatforms": ["Editor"]
  }
  ```
  После этого убрать все `#if UNITY_EDITOR` из файлов в `Editor/`.
- ⚠️ Смена имени сборки — breaking для тех, кто ссылается на неё по имени (маловероятно). Отметить в changelog.

### 3.2. `build_unitypackage.py`
- `pathname` пишется относительно корня репо (`Editor/...`) — Unity ожидает `Assets/...`. Использовать префикс `Assets/IMG2ETC2/`.
- Добавлять записи папок (`asset.meta` + `pathname` без `asset`) для `Editor.meta`.
- Исключить `package.json`, `Preview.png` (или осознанно оставить) — проверить, нужны ли они в `.unitypackage`.
- Вынести `import shutil` наверх.
- **Проверка:** импорт собранного пакета в чистый проект Unity — файлы оказываются в `Assets/IMG2ETC2/Editor/`, ошибок нет.

### 3.3. Версии и теги
- Тег `v1.7.0` стоит на коммите, где `package.json` = `1.6.0` — привести в соответствие.
- Старые теги `v.1.0.0`, `v.1.5.1`, `v.1.6.0` записаны в другом формате, чем `v1.7.0` (с точкой после `v`) — зафиксировать формат `vX.Y.Z`, чтобы имена артефактов и сверка версий были единообразны.
- В `release.yml` добавить шаг: сверка `github.ref_name` с `version` из `package.json`, падение при расхождении.

### 3.4. `package.json`
- Добавить `"unity": "2021.2"` (код использует C# 9: target-typed `new()`).
- `licensesUrl`: `blob/master` → `blob/main`.
- Добавить `CHANGELOG.md` (+ `.meta`) или убрать `changelogUrl`, если достаточно Releases.

---

## Этап 4 — README 🟡

- Пункт меню: `Tools > UnityIMG2ETC2` → `Tools > IMG2ETC2`.
- «There are 4 ways» → 5 (или пересчитать).
- Ссылка `archive/master.zip` → `archive/refs/heads/main.zip`.
- Описание padding привести в соответствие с реализацией из 1.2.
- Убрать предупреждение про «resolution may not be updated» после 2.6.
- Ссылка на документацию Unity без версии (`/Manual/...`), блог без `/ru/`.

---

## Этап 5 — Качество кода и улучшения 🟢

- **Быстрое чтение размеров:** `TextureImporter.GetSourceTextureWidthAndHeight` (через `AssetImporter.GetAtPath`) вместо полного декодирования. Даёт поддержку TGA/PSD/BMP/TIF/… для анализа (сама обработка — только для форматов, которые можно перезаписать).
- **Смена Rounding Mode** — пересчёт только целевых размеров, без ре-скана.
- **Проверка итогового импортированного размера:** `Max Size` / NPOT scale могут дать некратный 4 результат даже при корректном исходнике — показывать отдельным статусом.
- **Модель:** `ImageModel` хранит разрешения как строки → `Vector2Int`, форматирование только в UI.
- **UI:** пути относительно `Assets/`; клик по строке — `EditorGUIUtility.PingObject`; чекбоксы исключения файлов; виртуализация списка (рисовать только видимые строки) для тысяч файлов.
- **Стиль:** `GlobalStrings` → `static class`; единая кодировка (без BOM), убрать пустую первую строку в `RoundingMode.cs`.
- **Тесты:** EditMode-тесты (`Tests/Editor`, отдельный asmdef) на `RoundToMultipleOfFour`, padding/crop, сохранение GUID.
- **Фичи (по запросу):** режим POT; batch-запуск через `-executeMethod` для CI.
- **Репозиторий:** удалить устаревшую ветку `origin/features` (полностью влита в `main`).

---

## Порядок работы

1. Этап 1 (одна ветка `fix/data-safety`) — самое важное, затем релиз патча.
2. Этап 2 + 3.1 (`fix/editor-bugs`).
3. Этап 3.2–3.4 + 4 (`chore/packaging-and-docs`) → релиз `v1.8.0` (или `v2.0.0`, если смена имени asmdef и поведения resize считается breaking).
4. Этап 5 — по мере необходимости.
