# Changelog

## 1.1.3

- Fixed the Settings window showing the unapplied-changes prompt even when no settings were changed.
- Improved selected-text link opening responsiveness by removing a redundant hotkey-release wait before clipboard fallback copying.
- Reused compiled URL-matching expressions so complex selected text with surrounding content is recognized with less per-action overhead.

## 1.1.2

- Improved selected-text hotkey responsiveness by removing the pre-action modifier-key wait.
- Added an immediate non-activating "recognizing link" toast near the cursor when the selected-text hotkey is pressed.
- Made clipboard fallback use a fast path with a slower fallback wait for complex Chrome/web selections.
- Fixed stale clipboard reuse when `Ctrl+C` did not produce fresh selected text.
- Wait until hotkey modifier keys are released before clipboard fallback copying, so `Ctrl+Alt+X` does not turn the fallback copy into `Ctrl+Alt+C`.
- Wait for the trigger key itself to be released and retry copying without sending `Esc`, improving reliability in Codex/WebView selection UIs.
- Debounced repeated `WM_HOTKEY` messages so holding the selected-text hotkey only starts one recognition after the key chord is released.
- Limited UI Automation selection reads to a bounded text length to avoid slow reads on very large selections.
- Added opt-in selected-text timing diagnostics through `LXBP_SELECTION_TIMING=1` or `%AppData%\LXBrowserPicker\selection-timing.enabled`.

## 1.1.1

- Fixed truncated About tab QR code labels in English by shortening the WeChat feedback and voluntary support captions.

## 1.1.0

- Added a dedicated Selected Text Links settings tab.
- Added a tray-based global selected-text hotkey, defaulting to `Ctrl+Alt+X`.
- Added URL extraction from selected text, including extra surrounding text and optional common bare-domain recognition such as `example.com/path` and `example.cn`.
- Added optional Windows startup registration for the tray listener.
- Split Settings actions into OK and Apply, with an unapplied-changes prompt when closing.
- Refined English Settings layout and button alignment.
- Documented the clipboard privacy behavior used to copy selected text and restore the previous clipboard.

## 1.0.0

- Promoted LXBrowserPicker to the first stable release version.
- Moved About and voluntary support information into a dedicated settings tab.
- Changed installer output to `LXBrowserPickerSetup-v1.0.0.exe` so older local installers are not overwritten.
- Updated the application icon to the final black-background LX mark.
- Hardened browser handoff after selection for external applications by adding a shell-based launch path, direct-start fallback, working directory setup, and diagnostic logging.
- Clarified Settings browser-list buttons so default clearing and manual-entry removal are not confused with automatic browser discovery.
- Updated the free and voluntary support wording.
- Refined the About tab text spacing for the final 1.0.0 layout.

## 0.5.3

- Updated the application icon to a black-background premium LX routing mark.
- Added About information to the settings window with product description, author, GitHub link, WeChat contact, and voluntary support entry.
- Included About QR code assets in the installer package.
- Updated user-visible version and installer version to `0.5.3`.

## 0.5.2

- Added a dedicated LXBrowserPicker application icon.
- Embedded the icon into the EXE and installer.

## 0.5.1

- Fixed user configuration writes under Program Files by saving settings to `%AppData%\LXBrowserPicker`.
- Fixed overlapping controls in the Settings window.

## 0.5.0

- Renamed the app to LXBrowserPicker / LX浏览器选择器.
- Added built-in English and Simplified Chinese UI.
- Added first-run Windows default app setup guide.
- Added installer plan and Inno Setup script.
- Fixed Settings window ownership when opened from the picker window.
- Preserved browser icons, Once / Always, global default, per-app rules, Ask every time, manual browser paths, and candidate scanning.
