# LXBrowserPicker 1.1.0

LXBrowserPicker is a multi-browser picker for Windows. It can act as the default browser handler for requests sent to the system default browser, letting you choose which browser opens links, HTML files, and other browser-launch scenarios, with optional per-source rules.

## Included

- New Selected Text Links settings tab.
- New tray-based global selected-text hotkey, defaulting to `Ctrl+Alt+X`.
- Selected text may contain extra content. LXBrowserPicker extracts the first detected link and opens it through the existing browser picker flow.
- Optional common bare-domain recognition, including examples such as `example.com/path` and `example.cn`.
- Optional Windows startup registration for the tray listener.
- Settings now separates OK and Apply, and warns when closing with unapplied changes.
- Refined English Settings layout and button alignment.
- Privacy wording for the selected-text workflow: when the hotkey is pressed, LXBrowserPicker temporarily copies selected text, reads the clipboard, then tries to restore the original clipboard.

## Download

- `LXBrowserPickerSetup-v1.1.0.exe`

## SHA256

```text
55ADEE9570D25A07035E6CA449317FC50876E1789DABFD14B9E37FA5B759F49C
```

## Notes

The tray listener only powers the selected-text hotkey. Exiting the tray does not affect LXBrowserPicker's normal default-browser picker behavior.

Windows does not allow apps to silently set themselves as the default browser. After installation, open Windows Default Apps and set both `http` and `https` to `LXBrowserPicker`.

This installer is not code-signed. Microsoft Defender SmartScreen may still show a warning on first run until the installer or publisher gains reputation.
