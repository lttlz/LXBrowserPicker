# Changelog

## 总控记录

- `v0.5.2` 图标/品牌化阶段已完成并发布，安装包 SHA256 为 `E8E4BD0547C92BF0DF42C2B98318277B937C26D6D2352ED6330F7686D301C966`。
- `v0.5.1` 已发布到 GitHub Release，安装包 SHA256 为 `773163E721F813C8C54229EFFB1FF994624665EA6113D52B414EA164DCCFA5ED`。
- 后续用户可见 bugfix 或安装包变化应新建补丁版本，不覆盖旧 tag 的 Release 资产。

## 1.0.0

- Promoted LXBrowserPicker to the first stable release version.
- Moved About and voluntary support information into a dedicated settings tab.
- Changed installer output to `LXBrowserPickerSetup-v1.0.0.exe` so older local installers are not overwritten.
- Kept the black-background LX icon and direct support QR code assets from the 0.5.3 preparation build.
- Hardened browser handoff after selection for Fangcloud and other callers by adding a shell-based launch path, direct-start fallback, working directory setup, and diagnostic logging.
- Clarified Settings browser-list buttons so default clearing and manual-entry removal are not confused with automatic browser discovery.
- Replaced the voluntary support QR code with the approved appreciation QR code and updated the free/voluntary support wording.
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
