# LXBrowserPicker

LXBrowserPicker is a multi-browser picker for Windows. It can act as the default browser handler for requests sent to the system default browser, letting you choose which browser opens links, HTML files, and other browser-launch scenarios, with optional per-source rules.

## Features

- Browser selection window with browser icons
- Once / Always actions
- Global default browser
- Per-application rules
- Ask every time rules
- Manual browser path management
- Candidate browser scan
- Built-in English and Simplified Chinese UI
- First-run guide for Windows default app setup
- Global selected-text hotkey for opening links from any app through the tray listener
- About tab with project information and optional support details

## Install

Run `LXBrowserPickerSetup.exe`, choose a language and install path, then launch the app from the final setup page.

On first launch, LXBrowserPicker will guide you to Windows default app settings. Set `http` and `https` to `LXBrowserPicker`.

Windows does not allow apps to silently set themselves as the default browser, so this step must be completed manually.

## Usage

- `Once`: open this link only.
- `Always`: save a rule for the source application and open the link.
- `Settings`: manage browsers, rules, global default, and language.

Selected-text link opening can be enabled from Settings -> Selected Text Links. It uses a tray listener and a customizable global hotkey, defaulting to `Ctrl+Alt+X`. Select text in any app, press the hotkey, and LXBrowserPicker extracts the first link from the selected text. Extra surrounding text is allowed. Non-link text is not searched.

When bare-domain recognition is enabled, common domains such as `example.com/path` and `example.cn` are treated as links and opened as `https://...`.

Privacy note: when the hotkey is pressed, LXBrowserPicker temporarily copies the selected text, reads the clipboard, then tries to restore the original clipboard.

If the source application cannot be detected because Windows launched the picker through a system process, `Always` may save the global default instead.

## Configuration

User configuration is stored at `%AppData%\LXBrowserPicker\lx-browser-picker.config.json`.

If the user configuration does not exist, LXBrowserPicker reads `lx-browser-picker.config.json` next to the executable as the installed default template. `lx-browser-picker.config.example.json` is only a template.

## Legacy BrowserPicker

LXBrowserPicker does not automatically remove old `CodexBrowserPicker` registry entries. If you no longer need the old version, uninstall or clean it separately.

## Support

LXBrowserPicker is free to use. Optional support information is available in the About tab. Support is voluntary and does not unlock paid features, limits, subscriptions, or priority service.
