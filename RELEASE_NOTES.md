# LXBrowserPicker 1.1.3

LXBrowserPicker is a multi-browser picker for Windows. It can act as the default browser handler for requests sent to the system default browser, letting you choose which browser opens links, HTML files, and other browser-launch scenarios, with optional per-source rules.

## Fixed

- Fixed the Settings window showing the unapplied-changes prompt even when no settings were changed.
- Improved selected-text link opening responsiveness by removing a redundant hotkey-release wait before clipboard fallback copying.
- Reused compiled URL-matching expressions so complex selected text with surrounding content is recognized with less per-action overhead.

## Download

- `LXBrowserPickerSetup-v1.1.3.exe`

## SHA256

```text
A6E44F2A0E71AB75316F9955B3B004D9D635F1B4E86853C4ABD997FCF428F34F
```

## Notes

The tray listener only powers the selected-text hotkey. Exiting the tray does not affect LXBrowserPicker's normal default-browser picker behavior.

Windows does not allow apps to silently set themselves as the default browser. After installation, open Windows Default Apps and set both `http` and `https` to `LXBrowserPicker`.

This installer is not code-signed. Microsoft Defender SmartScreen may still show a warning on first run until the installer or publisher gains reputation.
