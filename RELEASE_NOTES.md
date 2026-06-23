# LXBrowserPicker 1.2.1

LXBrowserPicker is a multi-browser picker for Windows. It can act as the default browser handler for requests sent to the system default browser, letting you choose which browser opens links, HTML files, and other browser-launch scenarios, with optional per-source rules.

## Fixed

- Fixed missing installed application icons by embedding `assets/LXBrowserPicker.ico` into `LXBrowserPicker.exe`.
- This restores the expected icon for shortcuts, the tray, windows, and Windows default-app registration entries that point to `LXBrowserPicker.exe,0`.

## Download

- `LXBrowserPickerSetup-v1.2.1.exe`

## SHA256

```text
1776AB8B1766AD09217C576BFD94B899D29C572CFD9C66F84626775CF90BC59B
```

## Notes

This release only fixes the application icon packaging issue from v1.2.0. It does not change browser rules, selected-text hotkeys, update checks, or default browser protection behavior.

This installer is not code-signed. Microsoft Defender SmartScreen may still show a warning on first run until the installer or publisher gains reputation.
