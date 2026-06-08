# LX浏览器选择器

LX浏览器选择器是一个轻量的 Windows 浏览器选择器。把它设为 `http` 和 `https` 的默认应用后，每次打开外部链接时都可以选择用哪个已安装浏览器打开。

## 功能

- 带浏览器图标的选择窗口
- 仅一次 / 始终使用
- 全局默认浏览器
- 按来源应用保存规则
- 每次询问规则
- 手动管理浏览器路径
- 扫描候选浏览器
- 内置英文和简体中文界面
- 首次启动引导 Windows 默认应用设置

## 安装

运行 `LXBrowserPickerSetup.exe`，选择语言和安装路径，然后在安装完成页启动程序。

首次启动时，程序会引导你打开 Windows 默认应用设置。请把 `http` 和 `https` 设置为 `LXBrowserPicker`。

Windows 不允许应用静默把自己设为默认浏览器，所以这一步必须手动完成。

## 使用

- `仅一次`：只打开本次链接。
- `始终`：保存当前来源应用的浏览器规则，并打开链接。
- `设置`：管理浏览器、规则、全局默认浏览器和语言。

如果 Windows 通过系统进程中转启动选择器，程序可能无法识别真实来源应用；这种情况下，`始终` 可能保存为全局默认。

## 配置

用户配置保存在 `%AppData%\LXBrowserPicker\lx-browser-picker.config.json`。

如果用户配置不存在，LXBrowserPicker 会读取程序同目录的 `lx-browser-picker.config.json` 作为安装默认模板。`lx-browser-picker.config.example.json` 只是模板。

## 旧版 BrowserPicker

LX浏览器选择器不会自动删除旧的 `CodexBrowserPicker` 注册表项。如果不再需要旧版，请单独卸载或清理。
