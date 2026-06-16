# LXBrowserPicker 1.1.2

LXBrowserPicker is a multi-browser picker for Windows. It can act as the default browser handler for requests sent to the system default browser, letting you choose which browser opens links, HTML files, and other browser-launch scenarios, with optional per-source rules.

## Fixed

- Improved selected-text hotkey responsiveness by removing the pre-action modifier-key wait.
- Added an immediate non-activating "recognizing link" toast near the cursor when the selected-text hotkey is pressed.
- Made the clipboard fallback poll every 15 ms and continue as soon as copied text is available, with a 240 ms fast path and a 1000 ms fallback for complex Chrome/web selections.
- Fixed stale clipboard reuse when `Ctrl+C` did not produce fresh selected text.
- Wait until hotkey modifier keys are released before clipboard fallback copying, so `Ctrl+Alt+X` does not turn the fallback copy into `Ctrl+Alt+C`.
- Wait for the trigger key itself to be released and retry copying without sending `Esc`, improving reliability in Codex/WebView selection UIs.
- Debounced repeated `WM_HOTKEY` messages so holding the selected-text hotkey only starts one recognition after the key chord is released.
- Reduced the fallback retry pause after `Esc` from 40 ms to 25 ms.
- Limited UI Automation selected-text reads to 8192 characters to avoid slow reads when a very large block of text is selected.

## Diagnostics

- Added opt-in selected-text timing logs. Set `LXBP_SELECTION_TIMING=1` or create `%AppData%\LXBrowserPicker\selection-timing.enabled`, then reproduce the hotkey action and check `%AppData%\LXBrowserPicker\selection.log` for `TIMING` lines.

## Download

- `LXBrowserPickerSetup-v1.1.2.exe`

## SHA256

```text
2CE910EF1CCE28F468012D081D33ADD8888B32A6CEC7A96A4C69DFF95651A958
```

## Notes

The tray listener only powers the selected-text hotkey. Exiting the tray does not affect LXBrowserPicker's normal default-browser picker behavior.

Windows does not allow apps to silently set themselves as the default browser. After installation, open Windows Default Apps and set both `http` and `https` to `LXBrowserPicker`.

This installer is not code-signed. Microsoft Defender SmartScreen may still show a warning on first run until the installer or publisher gains reputation.
