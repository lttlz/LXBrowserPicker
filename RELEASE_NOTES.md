# LXBrowserPicker 1.2.0

LXBrowserPicker is a multi-browser picker for Windows. It can act as the default browser handler for requests sent to the system default browser, letting you choose which browser opens links, HTML files, and other browser-launch scenarios, with optional per-source rules.

## Added

- Added a second selected-text hotkey, defaulting to `Ctrl+Alt+C`, that always asks which browser should open the detected link.
- The manual selected-text picker now opens with one browser-logo click and never saves an "Always" rule.
- Added built-in update checks with Gitee, GitHub Releases, and jsDelivr metadata fallback.
- Added default browser protection for `http` and `https` defaults.

## Download

- `LXBrowserPickerSetup-v1.2.0.exe`

## SHA256

```text
1125B3014093F3CB049C90FF8B1BB94299A2EFE790D0043E9CC8B45A476ED503
```

## Notes

Update checks are enabled by default, but LXBrowserPicker does not automatically download or install updates.

Windows does not allow apps to silently set themselves as the default browser in all cases. If another browser takes over, LXBrowserPicker will try a supported recovery path and then guide you to Windows Default Apps when manual confirmation is required.

This installer is not code-signed. Microsoft Defender SmartScreen may still show a warning on first run until the installer or publisher gains reputation.
