# LXBrowserPicker 1.1.1

LXBrowserPicker is a multi-browser picker for Windows. It can act as the default browser handler for requests sent to the system default browser, letting you choose which browser opens links, HTML files, and other browser-launch scenarios, with optional per-source rules.

## Fixed

- Shortened the About tab QR code captions so the English labels for WeChat feedback and voluntary support are no longer truncated.

## Download

- `LXBrowserPickerSetup-v1.1.1.exe`

## SHA256

```text
7DCAE4806896E1DF2FBFE04B0F7F77B703FA6AC36CA9427FB3D983D6CB98E199
```

## Notes

The tray listener only powers the selected-text hotkey. Exiting the tray does not affect LXBrowserPicker's normal default-browser picker behavior.

Windows does not allow apps to silently set themselves as the default browser. After installation, open Windows Default Apps and set both `http` and `https` to `LXBrowserPicker`.

This installer is not code-signed. Microsoft Defender SmartScreen may still show a warning on first run until the installer or publisher gains reputation.
