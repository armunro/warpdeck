# warpdeck

A lightweight layer and macro management system and WPF Studio for Elgato Stream Deck with dynamic context switching, FontAwesome icon support, custom button themes, and interactive layer management.

## WPF Studio GUI

Warpdeck features a modern WPF Studio desktop application allowing you to:
- **Stream Deck Brightness Control**: Real-time slider to adjust connected Stream Deck display brightness (0 - 100%).
- **Create, Duplicate, and Remove Layers**: Add new layers, duplicate existing ones, and reorder layer priorities.
- **Configure Layer Properties**: Customize layer names, themes (`cyber`, `synthwave`, `korry`, `default`), default caption font, empty button display style (theme empty button vs black background), background/font colors, and dynamic window activation rules (process name, window title regex).
- **Interactive 32-Key Matrix Grid**: Visual 4x8 Stream Deck matrix with real-time Skia preview rendering of keys.
- **Delete Buttons with Delete Key**: Delete or clear any selected button instantly using the **Delete** key or the Delete button in the inspector.
- **Full Undo / Redo Support**: Seamlessly undo (**Ctrl+Z**) and redo (**Ctrl+Y** / **Ctrl+Shift+Z**) changes across layer modifications, button deletions, property edits, macro changes, and drag/drop actions.
- **Custom Caption Fonts**: Configure custom font families (e.g. Arial, Segoe UI, Consolas, Impact, Verdana, etc.) globally per layer or overridden per individual button.
- **Drag & Drop Organization**: Drag and drop buttons across the matrix to move or swap button positions; hold **Shift** while dragging to copy/duplicate buttons.
- **Copy Buttons Across Layers**: Easily copy and paste buttons between layers using **Ctrl+C** and **Ctrl+V**, right-click context menu ("Copy to Layer"), the Button Inspector cross-layer copy selector, or by dragging a button from the matrix directly onto a layer in the sidebar layer list.
- **Button Inspector**: Customize individual button captions, fonts, SVG icon paths (with built-in file browser), theme overrides, colors with an expanded quick palette and interactive custom color picker, and action macros / brightness triggers.
- **Expanded Color Palette & Custom Color Picker**: Choose from an expanded palette of categorized presets (neutrals, vibrant cyber neons, classic and deep shades) or open the interactive HSV/RGB Custom Color Picker dialog to select any custom color for buttons and layers.
- **Keystroke Recorder & Text Macros**: Record live keyboard shortcuts (e.g. `Ctrl+Alt+S`, `Win+Shift+S`, `F5`) directly from your keyboard into button macros, or type and edit macro text manually. Supports append/replace modes, backspace, and quick shortcut presets.
- **Macro Shortcuts & Presets**: Easily configure hotkeys and macros (e.g. `{{ctrl+c}}`, `{{win+left}}`, etc.).
- **Multi-Layer Priority Composition**: Active layers automatically compose on top of the default base layer by priority; undefined button slots gracefully fall through to show the underlying layer buttons and dispatch their actions.
- **Locked Buttons & Layer Protection**: Lock essential buttons (such as global window controls on the Default layer) with `locked: true` so they persist across all screens and cannot be blocked or covered by overlying layers.
- **Live Device Synchronization**: Save changes directly to `config.yaml` with instant device update and configuration reloading.

## Configuration & Brightness

Warpdeck supports global brightness configuration (0 - 100%) and SVG icons (such as FontAwesome) with configurable root directories. You can specify `brightness` and `iconRoot` at the root configuration level:

```yaml
iconRoot: "C:\\Tools\\fontawesome-pro-7.3.1-desktop\\svgs"
brightness: 100
```

## Button Themes

Warpdeck supports themes configurable at both the layer level and per button:

- **`default` / `classic`**: The modern pro-deck console style with sleek graphite/slate vertical gradient, specular glass highlight, ambient accent glow, studio accent bar, and high-readability typography.
- **`korry`**: An authentic aviation-style square pushbutton annunciator modeled after Korry 389 switches found on Airbus and Boeing flight decks. Features a dark matte cockpit bezel, split annunciator display windows, subtle glowing backlighting, and crisp uppercase legends.
- **`cyber` / `neon` / `hud` / `scifi`**: A futuristic Cyberpunk / Sci-Fi tactical HUD theme featuring deep obsidian surfaces, fine horizontal tech scanlines, glowing neon HUD corner brackets, tactical LED status ticks, neon bloom icon backlighting, and energized kinetic overdrive press reactions.
- **`synthwave` / `retro` / `outrun` / `arcade`**: An 80s Retro Synthwave / Outrun aesthetic featuring deep cosmic midnight violet gradients, radiant retro sunset horizon glow, perspective 3D ground wireframe gridlines, glowing duo-tone neon border frames (Hot Pink & Electric Cyan), and radiant backlit neon bloom icons.

### YAML Configuration Example

```yaml
iconRoot: "C:\\Tools\\fontawesome-pro-7.3.1-desktop\\svgs"

layers:
  - name: Cockpit
    priority: 0
    theme: "korry" # Sets default theme for the entire layer
    showEmptyButtons: true # Set to true to show theme empty button graphics, or false for black background
    buttons:
      - index: 0
        color: "#0070F3"
        caption: "Master Caution"
        icon: "solid/triangle-exclamation.svg"
        theme: "korry" # Can also be specified per-button
```