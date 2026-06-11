# LXBrowserPicker 1.0.0

LXBrowserPicker is a lightweight Windows browser picker. Install it, set `http` and `https` to `LXBrowserPicker` in Windows Default Apps, then choose which installed browser opens each link.

## Included

- First stable `1.0.0` release.
- New black-background LXBrowserPicker icon generated from the approved image master.
- About section moved into a dedicated Settings tab instead of being shown directly on the main settings view.
- Author, GitHub, WeChat contact, and voluntary support entries are available from the About tab.
- About text spacing has been refined for a cleaner final layout.
- Browser handoff after selection is more robust for Fangcloud and other callers, with shell-based launch, direct-start fallback, and diagnostic logging on failure.
- Settings browser-list buttons now clarify default clearing versus manual-entry removal.
- Voluntary appreciation remains optional. All features are free, with no paid limits or required donations.
- QR code assets packaged under `assets/about/`.

## Download

- `LXBrowserPickerSetup-v1.0.0.exe`

## SHA256

```text
8FFC6826F93D1A33CCECDFF454588BA6690A939E9686AF3FE56E39463B5E4AB4
```

## Notes

Windows does not allow apps to silently set themselves as the default browser. After installation, open Windows Default Apps and set both `http` and `https` to `LXBrowserPicker`.

QR code scanability should be confirmed with a real WeChat scan before publishing the release.

This installer is not code-signed. Microsoft Defender SmartScreen may still show a warning on first run until the installer or publisher gains reputation.
