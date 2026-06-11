# LXBrowserPicker 1.0.0

LXBrowserPicker is a multi-browser picker for Windows. It can act as the default browser handler for requests sent to the system default browser, letting you choose which browser opens links, HTML files, and other browser-launch scenarios, with optional per-source rules.

## Included

- First stable `1.0.0` release.
- New black-background LXBrowserPicker application icon.
- About section moved into a dedicated Settings tab instead of being shown directly on the main settings view.
- Author, GitHub, WeChat contact, and voluntary support entries are available from the About tab.
- About text spacing has been refined for a cleaner final layout.
- Browser handoff after selection is more robust for external applications, with shell-based launch, direct-start fallback, and diagnostic logging on failure.
- Settings browser-list buttons now clarify default clearing versus manual-entry removal.
- Voluntary appreciation remains optional. All features are free, with no paid limits or required donations.

## Download

- `LXBrowserPickerSetup-v1.0.0.exe`

## SHA256

```text
8FFC6826F93D1A33CCECDFF454588BA6690A939E9686AF3FE56E39463B5E4AB4
```

## Notes

Windows does not allow apps to silently set themselves as the default browser. After installation, open Windows Default Apps and set both `http` and `https` to `LXBrowserPicker`.

This installer is not code-signed. Microsoft Defender SmartScreen may still show a warning on first run until the installer or publisher gains reputation.
