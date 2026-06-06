# LXBrowserPicker

LXBrowserPicker is a lightweight Windows browser picker. Set it as the default app for `http` and `https`, then choose which installed browser opens each link.

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

## Install

Run `LXBrowserPickerSetup.exe`, choose a language and install path, then launch the app from the final setup page.

On first launch, LXBrowserPicker will guide you to Windows default app settings. Set `http` and `https` to `LXBrowserPicker`.

Windows does not allow apps to silently set themselves as the default browser, so this step must be completed manually.

## Usage

- `Once`: open this link only.
- `Always`: save a rule for the source application and open the link.
- `Settings`: manage browsers, rules, global default, and language.

If the source application cannot be detected because Windows launched the picker through a system process, `Always` may save the global default instead.

## Configuration

User configuration is stored next to the executable as `lx-browser-picker.config.json`.

The installer preserves this file during upgrades. `lx-browser-picker.config.example.json` is only a template.

## Legacy BrowserPicker

LXBrowserPicker does not automatically remove old `CodexBrowserPicker` registry entries. If you no longer need the old version, uninstall or clean it separately.
