# warpdeck

A lightweight layer and macro management system for Elgato Stream Deck with dynamic context switching, FontAwesome icon support, and custom button themes.

## Icon Configuration

Warpdeck supports SVG icons (such as FontAwesome) with configurable root directories. You can specify `iconRoot` at the root configuration level, layer level, or per-button level in `config.yaml`:

```yaml
iconRoot: "C:\\Tools\\fontawesome-pro-7.3.1-desktop\\svgs"
```

## Button Themes

Warpdeck supports themes configurable at both the layer level and per button:

- **`default` / `classic`**: The modern pro-deck console style with sleek graphite/slate vertical gradient, specular glass highlight, ambient accent glow, studio accent bar, and high-readability typography.
- **`korry`**: An authentic aviation-style square pushbutton annunciator modeled after Korry 389 switches found on Airbus and Boeing flight decks. Features a dark matte cockpit bezel, split annunciator display windows, subtle glowing backlighting, and crisp uppercase legends.
- **`cyber` / `neon` / `hud` / `scifi`**: A futuristic Cyberpunk / Sci-Fi tactical HUD theme featuring deep obsidian surfaces, fine horizontal tech scanlines, glowing neon HUD corner brackets, tactical LED status ticks, neon bloom icon backlighting, and energized kinetic overdrive press reactions.

### YAML Configuration Example

```yaml
iconRoot: "C:\\Tools\\fontawesome-pro-7.3.1-desktop\\svgs"

layers:
  - name: Cockpit
    priority: 0
    theme: "korry" # Sets default theme for the entire layer
    buttons:
      - index: 0
        color: "#0070F3"
        caption: "Master Caution"
        icon: "solid/triangle-exclamation.svg"
        theme: "korry" # Can also be specified per-button
```