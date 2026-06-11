using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows.Forms;

[assembly: System.Reflection.AssemblyTitle("LXBrowserPicker")]
[assembly: System.Reflection.AssemblyProduct("LXBrowserPicker")]
[assembly: System.Reflection.AssemblyCompany("lttlz")]
[assembly: System.Reflection.AssemblyVersion("1.0.0.0")]
[assembly: System.Reflection.AssemblyFileVersion("1.0.0.0")]

namespace LXBrowserPicker
{
    public class BrowserEntry
    {
        public string name { get; set; }
        public string path { get; set; }
    }

    public class AppRule
    {
        public string process { get; set; }
        public string browserPath { get; set; }
    }

    public class PickerConfig
    {
        public string defaultBrowserPath { get; set; }
        public bool firstRunScanDone { get; set; }
        public bool defaultAppGuideCompleted { get; set; }
        public string language { get; set; }
        public List<BrowserEntry> manualBrowsers { get; set; }
        public List<AppRule> appRules { get; set; }

        public PickerConfig()
        {
            defaultBrowserPath = "";
            firstRunScanDone = false;
            defaultAppGuideCompleted = false;
            language = "auto";
            manualBrowsers = new List<BrowserEntry>();
            appRules = new List<AppRule>();
        }
    }

    internal class BrowserInfo
    {
        public string Name;
        public string Path;
        public bool Manual;

        public override string ToString()
        {
            return Name;
        }
    }

    internal class RuleMatch
    {
        public bool Matched;
        public bool AskEveryTime;
        public BrowserInfo Browser;
    }

    internal enum PickAction
    {
        Once,
        Always
    }

    internal class PickResult
    {
        public PickAction Action;
        public BrowserInfo Browser;
    }

    internal static class I18n
    {
        private static string language = "en-US";

        public static void Configure(string value)
        {
            if (string.Equals(value, "zh-CN", StringComparison.OrdinalIgnoreCase))
            {
                language = "zh-CN";
                return;
            }
            if (string.Equals(value, "en-US", StringComparison.OrdinalIgnoreCase))
            {
                language = "en-US";
                return;
            }
            language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("zh", StringComparison.OrdinalIgnoreCase) ? "zh-CN" : "en-US";
        }

        public static bool IsZh()
        {
            return language == "zh-CN";
        }

        public static string T(string key)
        {
            if (IsZh())
            {
                switch (key)
                {
                    case "AppTitle": return "LX\u6D4F\u89C8\u5668\u9009\u62E9\u5668";
                    case "SettingsTitle": return "LX\u6D4F\u89C8\u5668\u9009\u62E9\u5668\u8BBE\u7F6E";
                    case "OpenLink": return "\u9009\u62E9\u6253\u5F00\u6B64\u94FE\u63A5\u7684\u6D4F\u89C8\u5668\uFF1A";
                    case "OpenLinkSource": return "\u9009\u62E9\u6253\u5F00\u6B64\u94FE\u63A5\u7684\u6D4F\u89C8\u5668\uFF1A  \u6765\u6E90\uFF1A";
                    case "Settings": return "\u8BBE\u7F6E";
                    case "Once": return "\u4EC5\u4E00\u6B21";
                    case "Always": return "\u59CB\u7EC8";
                    case "Cancel": return "\u53D6\u6D88";
                    case "Save": return "\u4FDD\u5B58";
                    case "Clear": return "\u6E05\u9664\u9ED8\u8BA4";
                    case "Browser": return "\u6D4F\u89C8\u5668";
                    case "Path": return "\u8DEF\u5F84";
                    case "AddBrowser": return "\u6DFB\u52A0\u6D4F\u89C8\u5668";
                    case "RemoveManual": return "\u79FB\u9664\u624B\u52A8\u6DFB\u52A0";
                    case "Scan": return "\u626B\u63CF";
                    case "SetDefault": return "\u8BBE\u4E3A\u9ED8\u8BA4";
                    case "ApplicationRules": return "\u5E94\u7528\u89C4\u5219";
                    case "AddRule": return "\u6DFB\u52A0\u89C4\u5219";
                    case "AskRule": return "\u6BCF\u6B21\u8BE2\u95EE";
                    case "Remove": return "\u79FB\u9664";
                    case "AskEveryTime": return "\u6BCF\u6B21\u8BE2\u95EE";
                    case "DefaultSuffix": return "\uFF08\u9ED8\u8BA4\uFF09";
                    case "Language": return "\u8BED\u8A00";
                    case "About": return "\u5173\u4E8E";
                    case "AboutTitle": return "LXBrowserPicker / LX\u6D4F\u89C8\u5668\u9009\u62E9\u5668";
                    case "AboutVersion": return "\u7248\u672C " + Program.AppVersionText;
                    case "AboutDescription": return "\u4E00\u4E2A\u7528\u4E8E Windows \u7684\u591A\u6D4F\u89C8\u5668\u9009\u62E9\u5668\u3002\u5B83\u53EF\u4EE5\u4F5C\u4E3A\u9ED8\u8BA4\u6D4F\u89C8\u5668\u5019\u9009\u63A5\u7BA1\u7CFB\u7EDF\u53D1\u5F80\u9ED8\u8BA4\u6D4F\u89C8\u5668\u7684\u6253\u5F00\u8BF7\u6C42\uFF0C\u8BA9\u4F60\u5728\u6253\u5F00\u7F51\u9875\u94FE\u63A5\u3001HTML \u6587\u4EF6\u6216\u5176\u4ED6\u6D4F\u89C8\u5668\u5524\u8D77\u573A\u666F\u65F6\u9009\u62E9\u5177\u4F53\u6D4F\u89C8\u5668\uFF0C\u5E76\u6309\u6765\u6E90\u4FDD\u5B58\u89C4\u5219\u3002";
                    case "AboutAuthor": return "\u4F5C\u8005\uFF1Alttlz";
                    case "AboutGithub": return "GitHub: https://github.com/lttlz/LXBrowserPicker";
                    case "AboutFree": return "\u672C\u8F6F\u4EF6\u5B8C\u5168\u514D\u8D39\u5F00\u653E\u5168\u90E8\u529F\u80FD\uFF0C\u65E0\u4ED8\u8D39\u9650\u5236\u3001\u65E0\u5F3A\u5236\u6253\u8D4F\u3002";
                    case "AboutSupportNote": return "\u82E5\u60A8\u89C9\u5F97\u5DE5\u5177\u5B9E\u7528\uFF0C\u53EF\u81EA\u613F\u5C0F\u989D\u8D5E\u8D4F\u652F\u6301\u540E\u7EED\u7EF4\u62A4\u66F4\u65B0\uFF0C\u652F\u6301\u4E0E\u5426\u4E0D\u5F71\u54CD\u4EFB\u4F55\u4F7F\u7528\u6743\u9650\u3002";
                    case "WechatContact": return "\u6DFB\u52A0\u5FAE\u4FE1\uFF1A\u53CD\u9988\u95EE\u9898 / \u4EA4\u6D41\u5EFA\u8BAE";
                    case "WechatSupport": return "\u81EA\u613F\u8D5E\u8D4F\uFF1A\u652F\u6301\u540E\u7EED\u7EF4\u62A4\u66F4\u65B0";
                    case "AutoBrowserCannotRemove": return "\u8FD9\u662F\u81EA\u52A8\u626B\u63CF\u5230\u7684\u6D4F\u89C8\u5668\uFF0C\u4E0D\u80FD\u7528\u201C\u79FB\u9664\u624B\u52A8\u6DFB\u52A0\u201D\u5220\u9664\u3002";
                    case "LaunchBrowserFailed": return "\u65E0\u6CD5\u542F\u52A8\u6240\u9009\u6D4F\u89C8\u5668\u3002\r\n\r\n\u6D4F\u89C8\u5668\uFF1A{0}\r\n\u94FE\u63A5\uFF1A{1}\r\n\u9519\u8BEF\uFF1A{2}\r\n\r\n\u5DF2\u5199\u5165\u8BCA\u65AD\u65E5\u5FD7\uFF1A\r\n{3}";
                    case "LangAuto": return "\u81EA\u52A8";
                    case "LangEnglish": return "English";
                    case "LangChinese": return "\u7B80\u4F53\u4E2D\u6587";
                    case "CandidateTitle": return "\u5019\u9009\u6D4F\u89C8\u5668";
                    case "CandidateLabel": return "\u53D1\u73B0\u53EF\u80FD\u9057\u6F0F\u7684\u6D4F\u89C8\u5668\u3002\u9009\u62E9\u8981\u6DFB\u52A0\u7684\u9879\u3002";
                    case "AddSelected": return "\u6DFB\u52A0\u6240\u9009";
                    case "Skip": return "\u8DF3\u8FC7";
                    case "NoBrowser": return "\u672A\u627E\u5230\u6D4F\u89C8\u5668\u3002\u8BF7\u5728\u8BBE\u7F6E\u4E2D\u6DFB\u52A0\u4E00\u4E2A\u3002";
                    case "NoCandidates": return "\u672A\u53D1\u73B0\u989D\u5916\u6D4F\u89C8\u5668\u5019\u9009\u9879\u3002";
                    case "SelectBrowserFirst": return "\u8BF7\u5148\u9009\u62E9\u4E00\u4E2A\u6D4F\u89C8\u5668\u3002";
                    case "AppProcessTitle": return "\u5E94\u7528\u8FDB\u7A0B";
                    case "AppProcessPrompt": return "\u8F93\u5165\u8FDB\u7A0B\u540D\uFF0C\u4F8B\u5982\uFF1ACodex \u6216 Codex.exe";
                    case "OK": return "\u786E\u5B9A";
                    case "GuideTitle": return "\u5B8C\u6210 Windows \u9ED8\u8BA4\u5E94\u7528\u8BBE\u7F6E";
                    case "GuideText": return "\u8981\u8BA9 LX\u6D4F\u89C8\u5668\u9009\u62E9\u5668\u63A5\u7BA1\u94FE\u63A5\uFF0C\u8BF7\u5728 Windows \u9ED8\u8BA4\u5E94\u7528\u4E2D\u628A http \u548C https \u8BBE\u4E3A LXBrowserPicker\u3002";
                    case "OpenDefaultApps": return "\u6253\u5F00\u9ED8\u8BA4\u5E94\u7528\u8BBE\u7F6E";
                    case "GuideDone": return "\u6211\u5DF2\u5B8C\u6210";
                    case "GuideLater": return "\u7A0D\u540E\u63D0\u9192";
                    case "ExeFilter": return "\u7A0B\u5E8F (*.exe)|*.exe";
                    case "SaveConfigFailed": return "\u65E0\u6CD5\u4FDD\u5B58\u8BBE\u7F6E\u5230\uFF1A\r\n{0}\r\n\r\n{1}";
                }
            }

            switch (key)
            {
                case "AppTitle": return "LXBrowserPicker";
                case "SettingsTitle": return "LXBrowserPicker Settings";
                case "OpenLink": return "Open this link with:";
                case "OpenLinkSource": return "Open this link with:  Source: ";
                case "Settings": return "Settings";
                case "Once": return "Once";
                case "Always": return "Always";
                case "Cancel": return "Cancel";
                case "Save": return "Save";
                case "Clear": return "Clear Default";
                case "Browser": return "Browser";
                case "Path": return "Path";
                case "AddBrowser": return "Add Browser";
                case "RemoveManual": return "Remove Manual Entry";
                case "Scan": return "Scan";
                case "SetDefault": return "Set Default";
                case "ApplicationRules": return "Application rules";
                case "AddRule": return "Add Rule";
                case "AskRule": return "Ask Rule";
                case "Remove": return "Remove";
                case "AskEveryTime": return "Ask every time";
                case "DefaultSuffix": return " (default)";
                case "Language": return "Language";
                case "About": return "About";
                case "AboutTitle": return "LXBrowserPicker";
                case "AboutVersion": return "Version " + Program.AppVersionText;
                case "AboutDescription": return "A multi-browser picker for Windows. It can act as the default browser handler for requests sent to the system default browser, letting you choose which browser opens links, HTML files, and other browser-launch scenarios, with optional per-source rules.";
                case "AboutAuthor": return "Author: lttlz";
                case "AboutGithub": return "GitHub: https://github.com/lttlz/LXBrowserPicker";
                case "AboutFree": return "LXBrowserPicker is completely free and all features are available without paid limits or required donations.";
                case "AboutSupportNote": return "If you find it useful, you may optionally make a small appreciation donation to support maintenance updates. Supporting it or not does not affect any usage rights.";
                case "WechatContact": return "WeChat: feedback / suggestions";
                case "WechatSupport": return "Voluntary appreciation: support maintenance updates";
                case "AutoBrowserCannotRemove": return "This browser was found automatically and cannot be removed with Remove Manual Entry.";
                case "LaunchBrowserFailed": return "Could not launch the selected browser.\r\n\r\nBrowser: {0}\r\nLink: {1}\r\nError: {2}\r\n\r\nDiagnostic log written to:\r\n{3}";
                case "LangAuto": return "Auto";
                case "LangEnglish": return "English";
                case "LangChinese": return "Simplified Chinese";
                case "CandidateTitle": return "Browser candidates";
                case "CandidateLabel": return "Candidate browsers were found. Select any browser you want to add.";
                case "AddSelected": return "Add Selected";
                case "Skip": return "Skip";
                case "NoBrowser": return "No browser was found. Add one in Settings.";
                case "NoCandidates": return "No additional browser candidates were found.";
                case "SelectBrowserFirst": return "Select a browser first.";
                case "AppProcessTitle": return "Application process";
                case "AppProcessPrompt": return "Enter process name, for example: Codex or Codex.exe";
                case "OK": return "OK";
                case "GuideTitle": return "Finish Windows default app setup";
                case "GuideText": return "To let LXBrowserPicker handle links, set http and https to LXBrowserPicker in Windows default apps.";
                case "OpenDefaultApps": return "Open Default Apps";
                case "GuideDone": return "I've finished";
                case "GuideLater": return "Remind me later";
                case "ExeFilter": return "Programs (*.exe)|*.exe";
                case "SaveConfigFailed": return "Could not save settings to:\r\n{0}\r\n\r\n{1}";
            }
            return key;
        }
    }

    internal static class Program
    {
        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SetCurrentProcessExplicitAppUserModelID(string appId);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint ExtractIconEx(string file, int index, IntPtr[] largeIcons, IntPtr[] smallIcons, uint icons);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr handle);

        private const int AttachParentProcess = -1;
        private const string AppName = "LXBrowserPicker";
        private const string AppUserModelId = "lttlz.LXBrowserPicker";
        private const string AppVersion = "1.0.0";
        private const string ConfigFileName = "lx-browser-picker.config.json";
        private static readonly string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string InstallConfigPath = Path.Combine(BaseDir, ConfigFileName);
        private static readonly string UserConfigDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
        private static readonly string UserConfigPath = Path.Combine(UserConfigDir, ConfigFileName);
        private static readonly string LaunchLogPath = Path.Combine(UserConfigDir, "launch.log");
        internal static string AppVersionText { get { return AppVersion; } }

        [STAThread]
        private static void Main(string[] args)
        {
            SetAppUserModelId();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            PickerConfig config = LoadConfig();
            I18n.Configure(config.language);

            if (args.Length > 0 && string.Equals(args[0], "--list", StringComparison.OrdinalIgnoreCase))
            {
                AttachConsoleForOutput();
                foreach (BrowserInfo browser in FindBrowsers(config))
                {
                    Console.WriteLine(browser.Name + "\t" + browser.Path);
                }
                return;
            }

            if (args.Length > 0 && string.Equals(args[0], "--candidates", StringComparison.OrdinalIgnoreCase))
            {
                AttachConsoleForOutput();
                foreach (BrowserInfo browser in FindCandidateBrowsers(FindBrowsers(config)))
                {
                    Console.WriteLine(browser.Name + "\t" + browser.Path);
                }
                return;
            }

            if (args.Length > 0 && string.Equals(args[0], "--settings", StringComparison.OrdinalIgnoreCase))
            {
                ShowSettings(config);
                return;
            }

            if (args.Length == 0 && !config.defaultAppGuideCompleted)
            {
                GuideForm guide = new GuideForm();
                DialogResult guideResult = guide.ShowDialog();
                if (guideResult == DialogResult.OK)
                {
                    config.defaultAppGuideCompleted = true;
                    SaveConfig(config);
                }
            }

            if (!config.firstRunScanDone)
            {
                List<BrowserInfo> browsers = FindBrowsers(config);
                List<BrowserInfo> candidates = FindCandidateBrowsers(browsers);
                config.firstRunScanDone = true;
                if (candidates.Count > 0)
                {
                    CandidateForm candidateForm = new CandidateForm(candidates);
                    if (candidateForm.ShowDialog() == DialogResult.OK)
                    {
                        foreach (BrowserInfo browser in candidateForm.SelectedBrowsers)
                        {
                            AddManualBrowser(config, browser.Name, browser.Path);
                        }
                    }
                }
                SaveConfig(config);
            }

            string url = args.Length > 0 ? string.Join(" ", args).Trim('"') : "";
            if (string.IsNullOrWhiteSpace(url))
            {
                ShowSettings(config);
                return;
            }

            List<BrowserInfo> available = FindBrowsers(config);
            if (available.Count == 0)
            {
                MessageBox.Show(I18n.T("NoBrowser"), I18n.T("AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ShowSettings(config);
                return;
            }

            string parentProcess = GetParentProcessName();
            RuleMatch ruleMatch = FindRuleMatch(config, available, parentProcess);
            if (ruleMatch.Matched && ruleMatch.Browser != null)
            {
                OpenBrowser(ruleMatch.Browser, url, parentProcess);
                return;
            }

            if (!ruleMatch.AskEveryTime)
            {
                BrowserInfo defaultBrowser = FindByPath(available, config.defaultBrowserPath);
                if (defaultBrowser != null)
                {
                    OpenBrowser(defaultBrowser, url, parentProcess);
                    return;
                }
            }

            PickResult pick = PickerForm.Pick(available, url, parentProcess, delegate(Form owner)
            {
                ShowSettings(config, owner);
                config = LoadConfig();
                I18n.Configure(config.language);
                available = FindBrowsers(config);
            });

            if (pick != null && pick.Browser != null)
            {
                if (pick.Action == PickAction.Always)
                {
                    SaveAlwaysRule(config, parentProcess, pick.Browser);
                }
                OpenBrowser(pick.Browser, url, parentProcess);
            }
        }

        private static void SetAppUserModelId()
        {
            try
            {
                SetCurrentProcessExplicitAppUserModelID(AppUserModelId);
            }
            catch
            {
            }
        }

        private static void AttachConsoleForOutput()
        {
            AttachConsole(AttachParentProcess);
            StreamWriter writer = new StreamWriter(Console.OpenStandardOutput(), Encoding.UTF8);
            writer.AutoFlush = true;
            Console.SetOut(writer);
        }

        private static PickerConfig LoadConfig()
        {
            if (File.Exists(UserConfigPath))
            {
                return LoadConfigFromPath(UserConfigPath);
            }

            if (File.Exists(InstallConfigPath))
            {
                return LoadConfigFromPath(InstallConfigPath);
            }

            return new PickerConfig();
        }

        private static PickerConfig LoadConfigFromPath(string path)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                PickerConfig config = serializer.Deserialize<PickerConfig>(File.ReadAllText(path));
                if (config == null)
                {
                    return new PickerConfig();
                }

                return NormalizeConfig(config);
            }
            catch
            {
                return new PickerConfig();
            }
        }

        private static PickerConfig NormalizeConfig(PickerConfig config)
        {
            if (config.manualBrowsers == null) config.manualBrowsers = new List<BrowserEntry>();
            if (config.appRules == null) config.appRules = new List<AppRule>();
            if (config.defaultBrowserPath == null) config.defaultBrowserPath = "";
            if (string.IsNullOrWhiteSpace(config.language)) config.language = "auto";
            return config;
        }

        private static bool SaveConfig(PickerConfig config)
        {
            try
            {
                Directory.CreateDirectory(UserConfigDir);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(config);
                File.WriteAllText(UserConfigPath, PrettyJson(json));
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowSaveConfigError(ex);
                return false;
            }
            catch (IOException ex)
            {
                ShowSaveConfigError(ex);
                return false;
            }
        }

        private static void ShowSaveConfigError(Exception ex)
        {
            MessageBox.Show(
                string.Format(I18n.T("SaveConfigFailed"), UserConfigPath, ex.Message),
                I18n.T("AppTitle"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static void SaveAlwaysRule(PickerConfig config, string parentProcess, BrowserInfo browser)
        {
            if (!string.IsNullOrWhiteSpace(parentProcess))
            {
                for (int i = config.appRules.Count - 1; i >= 0; i--)
                {
                    if (string.Equals(NormalizeProcess(config.appRules[i].process), NormalizeProcess(parentProcess), StringComparison.OrdinalIgnoreCase))
                    {
                        config.appRules.RemoveAt(i);
                    }
                }
                config.appRules.Add(new AppRule { process = parentProcess.Trim(), browserPath = browser.Path });
            }
            else
            {
                config.defaultBrowserPath = browser.Path;
            }
            SaveConfig(config);
        }

        private static string PrettyJson(string json)
        {
            int indent = 0;
            bool quoted = false;
            string result = "";
            for (int i = 0; i < json.Length; i++)
            {
                char ch = json[i];
                if (ch == '"' && (i == 0 || json[i - 1] != '\\')) quoted = !quoted;
                if (!quoted && (ch == '{' || ch == '['))
                {
                    result += ch + Environment.NewLine + new string(' ', ++indent * 2);
                }
                else if (!quoted && (ch == '}' || ch == ']'))
                {
                    result += Environment.NewLine + new string(' ', --indent * 2) + ch;
                }
                else if (!quoted && ch == ',')
                {
                    result += ch + Environment.NewLine + new string(' ', indent * 2);
                }
                else if (!quoted && ch == ':')
                {
                    result += ": ";
                }
                else
                {
                    result += ch;
                }
            }
            return result;
        }

        private static List<BrowserInfo> FindBrowsers(PickerConfig config)
        {
            Dictionary<string, BrowserInfo> byPath = new Dictionary<string, BrowserInfo>(StringComparer.OrdinalIgnoreCase);

            AddRegistryBrowsers(byPath, Registry.LocalMachine, @"Software\Clients\StartMenuInternet");
            AddRegistryBrowsers(byPath, Registry.CurrentUser, @"Software\Clients\StartMenuInternet");
            AddRegistryBrowsers(byPath, Registry.LocalMachine, @"Software\WOW6432Node\Clients\StartMenuInternet");
            AddAppPathBrowser(byPath, Registry.LocalMachine, @"Software\Microsoft\Windows\CurrentVersion\App Paths\QQBrowser.exe", "QQ Browser");
            AddAppPathBrowser(byPath, Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\App Paths\QQBrowser.exe", "QQ Browser");
            AddAppPathBrowser(byPath, Registry.LocalMachine, @"Software\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\QQBrowser.exe", "QQ Browser");

            AddKnownBrowser(byPath, "Chrome", Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\Google\Chrome\Application\chrome.exe"), false);
            AddKnownBrowser(byPath, "Chrome", Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\Google\Chrome\Application\chrome.exe"), false);
            AddKnownBrowser(byPath, "Chrome", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Google\Chrome\Application\chrome.exe"), false);
            AddKnownBrowser(byPath, "Firefox", Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\Mozilla Firefox\firefox.exe"), false);
            AddKnownBrowser(byPath, "Firefox", Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\Mozilla Firefox\firefox.exe"), false);
            AddKnownBrowser(byPath, "Brave", Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\BraveSoftware\Brave-Browser\Application\brave.exe"), false);
            AddKnownBrowser(byPath, "Brave", Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\BraveSoftware\Brave-Browser\Application\brave.exe"), false);
            AddKnownBrowser(byPath, "Brave", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"BraveSoftware\Brave-Browser\Application\brave.exe"), false);
            AddKnownBrowser(byPath, "QQ Browser", Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\Tencent\QQBrowser\QQBrowser.exe"), false);
            AddKnownBrowser(byPath, "QQ Browser", Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\Tencent\QQBrowser\QQBrowser.exe"), false);
            AddKnownBrowser(byPath, "QQ Browser", Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\TSoft\QQSrow\QQBrowser.exe"), false);
            AddKnownBrowser(byPath, "QQ Browser", Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\TSoft\QQSrow\QQBrowser.exe"), false);
            AddKnownBrowser(byPath, "QQ Browser", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Tencent\QQBrowser\QQBrowser.exe"), false);
            AddKnownBrowser(byPath, "QQ Browser", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Tencent\QQBrowser\QQBrowser.exe"), false);

            foreach (BrowserEntry entry in config.manualBrowsers)
            {
                AddKnownBrowser(byPath, entry.name, entry.path, true);
            }

            List<BrowserInfo> result = new List<BrowserInfo>(byPath.Values);
            result.Sort(delegate (BrowserInfo a, BrowserInfo b)
            {
                return string.Compare(a.Name, b.Name, StringComparison.CurrentCultureIgnoreCase);
            });
            return result;
        }

        private static void AddRegistryBrowsers(Dictionary<string, BrowserInfo> byPath, RegistryKey root, string subkey)
        {
            using (RegistryKey key = root.OpenSubKey(subkey))
            {
                if (key == null) return;

                foreach (string childName in key.GetSubKeyNames())
                {
                    if (childName.IndexOf("BrowserPicker", StringComparison.OrdinalIgnoreCase) >= 0) continue;

                    using (RegistryKey child = key.OpenSubKey(childName))
                    using (RegistryKey commandKey = child == null ? null : child.OpenSubKey(@"shell\open\command"))
                    {
                        string command = commandKey == null ? "" : Convert.ToString(commandKey.GetValue(""));
                        string exePath = ExtractExecutablePath(command);
                        if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath)) continue;

                        string displayName = CleanName(childName);
                        using (RegistryKey cap = child.OpenSubKey("Capabilities"))
                        {
                            if (cap != null)
                            {
                                string appName = Convert.ToString(cap.GetValue("ApplicationName"));
                                if (!string.IsNullOrWhiteSpace(appName)) displayName = appName;
                            }
                        }

                        AddKnownBrowser(byPath, displayName, exePath, false);
                    }
                }
            }
        }

        private static void AddAppPathBrowser(Dictionary<string, BrowserInfo> byPath, RegistryKey root, string subkey, string name)
        {
            using (RegistryKey key = root.OpenSubKey(subkey))
            {
                if (key == null) return;

                string exePath = Convert.ToString(key.GetValue(""));
                if (!string.IsNullOrWhiteSpace(exePath))
                {
                    AddKnownBrowser(byPath, name, Environment.ExpandEnvironmentVariables(exePath), false);
                }

                string folderPath = Convert.ToString(key.GetValue("Path"));
                if (!string.IsNullOrWhiteSpace(folderPath))
                {
                    AddKnownBrowser(byPath, name, Path.Combine(Environment.ExpandEnvironmentVariables(folderPath), "QQBrowser.exe"), false);
                }
            }
        }

        private static void AddKnownBrowser(Dictionary<string, BrowserInfo> byPath, string name, string path, bool manual)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            path = Environment.ExpandEnvironmentVariables(path);
            if (!File.Exists(path)) return;

            string fullPath = Path.GetFullPath(path);
            if (fullPath.IndexOf("BrowserPicker", StringComparison.OrdinalIgnoreCase) >= 0) return;

            if (!byPath.ContainsKey(fullPath))
            {
                byPath[fullPath] = new BrowserInfo { Name = CleanName(name), Path = fullPath, Manual = manual };
            }
        }

        private static List<BrowserInfo> FindCandidateBrowsers(List<BrowserInfo> known)
        {
            Dictionary<string, BrowserInfo> candidates = new Dictionary<string, BrowserInfo>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> knownPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (BrowserInfo browser in known) knownPaths.Add(browser.Path);

            string[] roots = new string[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"D:\Program Files",
                @"D:\Program Files (x86)"
            };

            foreach (string root in roots)
            {
                ScanCandidates(root, 0, 4, knownPaths, candidates);
            }

            List<BrowserInfo> result = new List<BrowserInfo>(candidates.Values);
            result.Sort(delegate (BrowserInfo a, BrowserInfo b)
            {
                return string.Compare(a.Name, b.Name, StringComparison.CurrentCultureIgnoreCase);
            });
            return result;
        }

        private static void ScanCandidates(string folder, int depth, int maxDepth, HashSet<string> knownPaths, Dictionary<string, BrowserInfo> candidates)
        {
            if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder) || depth > maxDepth) return;

            string folderName = Path.GetFileName(folder) ?? "";
            bool interestingFolder = IsBrowserFolder(folderName) || depth <= 1;

            try
            {
                if (interestingFolder)
                {
                    foreach (string file in Directory.GetFiles(folder, "*.exe"))
                    {
                        string fileName = Path.GetFileName(file);
                        if (!IsBrowserExecutable(fileName, file)) continue;
                        if (knownPaths.Contains(file)) continue;
                        if (file.IndexOf("BrowserPicker", StringComparison.OrdinalIgnoreCase) >= 0) continue;

                        candidates[file] = new BrowserInfo { Name = GetFileDisplayName(file), Path = file, Manual = true };
                    }
                }

                foreach (string dir in Directory.GetDirectories(folder))
                {
                    string name = Path.GetFileName(dir) ?? "";
                    if (depth < 1 || IsBrowserFolder(name) || IsKnownVendorFolder(name))
                    {
                        ScanCandidates(dir, depth + 1, maxDepth, knownPaths, candidates);
                    }
                }
            }
            catch
            {
            }
        }

        private static bool IsBrowserFolder(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            string value = text.ToLowerInvariant();
            return value.Contains("browser") || value.Contains("chrome") || value.Contains("firefox") ||
                   value.Contains("brave") || value.Contains("opera") || value.Contains("vivaldi") ||
                   value.Contains("ucbrowser") || value.Contains("liebao") || value.Contains("dcbrowser") ||
                   value.Contains("qqbrowser") || value.Contains("qqsrow") || value.Contains("360se") ||
                   value.Contains("360chrome") || value.Contains("sogouexplorer");
        }

        private static bool IsKnownVendorFolder(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            string value = text.ToLowerInvariant();
            return value == "tencent" || value == "tsoft" || value == "google" || value == "mozilla" ||
                   value == "bravesoftware" || value == "microsoft" || value == "opera software" ||
                   value == "360" || value == "sogou";
        }

        private static bool IsBrowserExecutable(string fileName, string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;

            string name = fileName.ToLowerInvariant();
            if (fullPath.IndexOf(@"\temp\", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            if (IsNonBrowserHelper(name)) return false;

            if (name == "chrome.exe" || name == "msedge.exe" || name == "firefox.exe" ||
                name == "brave.exe" || name == "opera.exe" ||
                name == "vivaldi.exe" || name == "qqbrowser.exe" || name == "360se.exe" ||
                name == "360chrome.exe" || name == "sogouexplorer.exe" || name == "ucbrowser.exe" ||
                name == "liebao.exe" || name == "dcbrowser.exe")
            {
                return true;
            }

            if (name == "launcher.exe" && fullPath.IndexOf("Opera", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            if (name.EndsWith("browser.exe", StringComparison.OrdinalIgnoreCase) &&
                !name.Contains("update") && !name.Contains("service") && !name.Contains("helper"))
            {
                return true;
            }

            string productName = "";
            string description = "";
            try
            {
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(fullPath);
                productName = (info.ProductName ?? "").ToLowerInvariant();
                description = (info.FileDescription ?? "").ToLowerInvariant();
            }
            catch
            {
            }

            string combined = productName + " " + description;
            if (IsNonBrowserHelper(combined)) return false;

            return combined.Contains("web browser") || combined.Contains("internet browser") ||
                   combined.Contains("浏览器") || combined.Contains("chrome") ||
                   combined.Contains("firefox") || combined.Contains("brave") ||
                   combined.Contains("vivaldi") || combined.Contains("opera browser");
        }

        private static bool IsNonBrowserHelper(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            string value = text.ToLowerInvariant();
            return value.Contains("update") || value.Contains("updater") || value.Contains("install") ||
                   value.Contains("uninstall") || value.Contains("uninst") || value.Contains("setup") || value.Contains("service") ||
                   value.Contains("crash") || value.Contains("helper") || value.Contains("sandbox") ||
                   value.Contains("repair") || value.Contains("liveup") || value.Contains("loader") ||
                   value.Contains("download") || value.Contains("zip") || value.Contains("music") ||
                   value.Contains("svr") || value.Contains("module") || value.Contains("bug") ||
                   value.Contains("report") || value.Contains("proxy");
        }

        private static string GetFileDisplayName(string path)
        {
            try
            {
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(path);
                if (!string.IsNullOrWhiteSpace(info.ProductName)) return info.ProductName;
                if (!string.IsNullOrWhiteSpace(info.FileDescription)) return info.FileDescription;
            }
            catch
            {
            }
            return Path.GetFileNameWithoutExtension(path);
        }

        private static string ExtractExecutablePath(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return "";
            command = Environment.ExpandEnvironmentVariables(command.Trim());
            Match quoted = Regex.Match(command, "^\"([^\"]+\\.exe)\"", RegexOptions.IgnoreCase);
            if (quoted.Success) return quoted.Groups[1].Value;
            Match bare = Regex.Match(command, "^([^\\s]+\\.exe)", RegexOptions.IgnoreCase);
            return bare.Success ? bare.Groups[1].Value : "";
        }

        private static string CleanName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Browser";
            return name.Replace(".EXE", "").Replace(".exe", "").Trim();
        }

        private static BrowserInfo FindByPath(List<BrowserInfo> browsers, string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;
            foreach (BrowserInfo browser in browsers)
            {
                if (string.Equals(browser.Path, path, StringComparison.OrdinalIgnoreCase)) return browser;
            }
            return null;
        }

        private static RuleMatch FindRuleMatch(PickerConfig config, List<BrowserInfo> browsers, string parentProcess)
        {
            RuleMatch result = new RuleMatch();
            if (string.IsNullOrWhiteSpace(parentProcess)) return result;
            string normalizedParent = NormalizeProcess(parentProcess);

            foreach (AppRule rule in config.appRules)
            {
                if (string.Equals(NormalizeProcess(rule.process), normalizedParent, StringComparison.OrdinalIgnoreCase))
                {
                    result.Matched = true;
                    if (string.Equals(rule.browserPath, "ASK", StringComparison.OrdinalIgnoreCase))
                    {
                        result.AskEveryTime = true;
                        return result;
                    }
                    result.Browser = FindByPath(browsers, rule.browserPath);
                    return result;
                }
            }
            return result;
        }

        private static string NormalizeProcess(string process)
        {
            if (string.IsNullOrWhiteSpace(process)) return "";
            process = process.Trim();
            if (process.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) process = process.Substring(0, process.Length - 4);
            return process;
        }

        private static string GetParentProcessName()
        {
            try
            {
                using (ManagementObject current = new ManagementObject("win32_process.handle='" + Process.GetCurrentProcess().Id + "'"))
                {
                    current.Get();
                    object parentPid = current["ParentProcessId"];
                    if (parentPid == null) return "";
                    using (ManagementObject parent = new ManagementObject("win32_process.handle='" + parentPid + "'"))
                    {
                        parent.Get();
                        return Path.GetFileNameWithoutExtension(Convert.ToString(parent["Name"]));
                    }
                }
            }
            catch
            {
                return "";
            }
        }

        private static void AddManualBrowser(PickerConfig config, string name, string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return;
            foreach (BrowserEntry entry in config.manualBrowsers)
            {
                if (string.Equals(entry.path, path, StringComparison.OrdinalIgnoreCase)) return;
            }
            config.manualBrowsers.Add(new BrowserEntry { name = CleanName(name), path = Path.GetFullPath(path) });
        }

        private static void OpenBrowser(BrowserInfo browser, string url, string sourceProcess)
        {
            Exception shellError = TryStartBrowser(browser, url, true);
            if (shellError == null) return;

            Exception directError = TryStartBrowser(browser, url, false);
            if (directError == null) return;

            LogLaunchFailure(browser, url, sourceProcess, shellError, directError);
            ShowLaunchFailure(browser, url, shellError, directError);
        }

        private static Exception TryStartBrowser(BrowserInfo browser, string url, bool useShellExecute)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = browser.Path;
                info.Arguments = QuoteArgument(url);
                info.UseShellExecute = useShellExecute;

                string workingDirectory = Path.GetDirectoryName(browser.Path);
                if (!string.IsNullOrWhiteSpace(workingDirectory) && Directory.Exists(workingDirectory))
                {
                    info.WorkingDirectory = workingDirectory;
                }

                Process process = Process.Start(info);
                if (process == null) return new InvalidOperationException("Process.Start returned null.");
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        private static string QuoteArgument(string value)
        {
            if (value == null) return "\"\"";

            StringBuilder builder = new StringBuilder();
            builder.Append('"');
            int backslashes = 0;
            foreach (char ch in value)
            {
                if (ch == '\\')
                {
                    backslashes++;
                }
                else if (ch == '"')
                {
                    builder.Append('\\', backslashes * 2 + 1);
                    builder.Append('"');
                    backslashes = 0;
                }
                else
                {
                    if (backslashes > 0)
                    {
                        builder.Append('\\', backslashes);
                        backslashes = 0;
                    }
                    builder.Append(ch);
                }
            }
            if (backslashes > 0)
            {
                builder.Append('\\', backslashes * 2);
            }
            builder.Append('"');
            return builder.ToString();
        }

        private static void ShowLaunchFailure(BrowserInfo browser, string url, Exception shellError, Exception directError)
        {
            MessageBox.Show(
                string.Format(
                    I18n.T("LaunchBrowserFailed"),
                    browser.Name + " - " + browser.Path,
                    url,
                    BuildLaunchErrorSummary(shellError, directError),
                    LaunchLogPath),
                I18n.T("AppTitle"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static string BuildLaunchErrorSummary(Exception shellError, Exception directError)
        {
            return "ShellExecute: " + DescribeException(shellError) + "; Direct: " + DescribeException(directError);
        }

        private static string DescribeException(Exception ex)
        {
            if (ex == null) return "";
            return ex.GetType().Name + ": " + ex.Message;
        }

        private static void LogLaunchFailure(BrowserInfo browser, string url, string sourceProcess, Exception shellError, Exception directError)
        {
            try
            {
                Directory.CreateDirectory(UserConfigDir);
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "]");
                builder.AppendLine("Source: " + sourceProcess);
                builder.AppendLine("BrowserName: " + browser.Name);
                builder.AppendLine("BrowserPath: " + browser.Path);
                builder.AppendLine("Url: " + url);
                builder.AppendLine("ShellExecuteError: " + DescribeException(shellError));
                builder.AppendLine("DirectError: " + DescribeException(directError));
                builder.AppendLine();
                File.AppendAllText(LaunchLogPath, builder.ToString(), Encoding.UTF8);
            }
            catch
            {
            }
        }

        private static void ShowSettings(PickerConfig config)
        {
            ShowSettings(config, null);
        }

        private static void ShowSettings(PickerConfig config, IWin32Window owner)
        {
            SettingsForm form = new SettingsForm(config);
            DialogResult result = owner == null ? form.ShowDialog() : form.ShowDialog(owner);
            if (result == DialogResult.OK)
            {
                if (SaveConfig(form.Config))
                {
                    I18n.Configure(form.Config.language);
                }
            }
        }

        internal static ImageList BuildImageList(List<BrowserInfo> browsers, int size)
        {
            ImageList images = new ImageList();
            images.ImageSize = new Size(size, size);
            images.ColorDepth = ColorDepth.Depth32Bit;
            foreach (BrowserInfo browser in browsers)
            {
                images.Images.Add(browser.Path, ExtractIcon(browser.Path, size));
            }
            return images;
        }

        private static Icon ExtractIcon(string path, int size)
        {
            try
            {
                Icon icon = Icon.ExtractAssociatedIcon(path);
                if (icon != null) return new Icon(icon, size, size);
            }
            catch
            {
            }
            return SystemIcons.Application;
        }

        internal static Image ExtractIconImage(string path, int size)
        {
            return ExtractIcon(path, size).ToBitmap();
        }

        internal static Icon GetAppIcon()
        {
            Icon icon = ExtractAppIcon(0);
            if (icon != null) return icon;

            try
            {
                icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                if (icon != null) return new Icon(icon, 32, 32);
            }
            catch
            {
            }
            return SystemIcons.Application;
        }

        private static Icon ExtractAppIcon(int index)
        {
            IntPtr[] largeIcons = new IntPtr[1];
            try
            {
                if (ExtractIconEx(Application.ExecutablePath, index, largeIcons, null, 1) > 0 && largeIcons[0] != IntPtr.Zero)
                {
                    Icon icon = (Icon)Icon.FromHandle(largeIcons[0]).Clone();
                    return new Icon(icon, 32, 32);
                }
            }
            catch
            {
            }
            finally
            {
                if (largeIcons[0] != IntPtr.Zero)
                {
                    DestroyIcon(largeIcons[0]);
                }
            }
            return null;
        }

        internal static Image LoadAboutImage(string fileName, int size)
        {
            string imagePath = Path.Combine(BaseDir, "assets", "about", fileName);
            if (!File.Exists(imagePath))
            {
                imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            }
            if (!File.Exists(imagePath)) return null;

            using (Image original = Image.FromFile(imagePath))
            {
                Bitmap bitmap = new Bitmap(size, size);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.White);
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.DrawImage(original, 0, 0, size, size);
                }
                return bitmap;
            }
        }

        internal static List<BrowserInfo> PublicFindBrowsers(PickerConfig config)
        {
            return FindBrowsers(config);
        }

        internal static void PublicAddManualBrowser(PickerConfig config, string name, string path)
        {
            AddManualBrowser(config, name, path);
        }

        internal static List<BrowserInfo> PublicFindCandidateBrowsers(List<BrowserInfo> known)
        {
            return FindCandidateBrowsers(known);
        }
    }

    internal class GuideForm : Form
    {
        public GuideForm()
        {
            Text = I18n.T("GuideTitle");
            Icon = Program.GetAppIcon();
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(560, 210);

            Label label = new Label();
            label.Text = I18n.T("GuideText");
            label.Location = new Point(18, 18);
            label.Size = new Size(520, 76);

            Label step = new Label();
            step.Text = "http / https -> LXBrowserPicker";
            step.Location = new Point(18, 96);
            step.Size = new Size(520, 24);
            step.Font = new Font(step.Font, FontStyle.Bold);

            Button openSettings = new Button();
            openSettings.Text = I18n.T("OpenDefaultApps");
            openSettings.Location = new Point(18, 150);
            openSettings.Size = new Size(170, 30);
            openSettings.Click += delegate
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "ms-settings:defaultapps";
                info.UseShellExecute = true;
                Process.Start(info);
            };

            Button done = new Button();
            done.Text = I18n.T("GuideDone");
            done.Location = new Point(312, 150);
            done.Size = new Size(104, 30);
            done.DialogResult = DialogResult.OK;

            Button later = new Button();
            later.Text = I18n.T("GuideLater");
            later.Location = new Point(428, 150);
            later.Size = new Size(110, 30);
            later.DialogResult = DialogResult.Cancel;

            Controls.Add(label);
            Controls.Add(step);
            Controls.Add(openSettings);
            Controls.Add(done);
            Controls.Add(later);
            AcceptButton = done;
            CancelButton = later;
        }
    }

    internal class PickerForm : Form
    {
        private readonly FlowLayoutPanel browserPanel;
        private readonly ToolTip tips = new ToolTip();
        private BrowserInfo selectedBrowser;
        private PickAction pickAction = PickAction.Once;

        private PickerForm(List<BrowserInfo> browsers, string url, string parentProcess, Action<Form> settingsAction)
        {
            Text = I18n.T("AppTitle");
            Icon = Program.GetAppIcon();
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(560, 330);
            TopMost = true;

            Label label = new Label();
            label.Text = string.IsNullOrWhiteSpace(parentProcess) ? I18n.T("OpenLink") : I18n.T("OpenLinkSource") + parentProcess;
            label.Location = new Point(18, 16);
            label.Size = new Size(520, 22);

            TextBox urlBox = new TextBox();
            urlBox.Text = url;
            urlBox.ReadOnly = true;
            urlBox.Location = new Point(18, 42);
            urlBox.Size = new Size(524, 24);

            browserPanel = new FlowLayoutPanel();
            browserPanel.Location = new Point(18, 78);
            browserPanel.Size = new Size(524, 190);
            browserPanel.AutoScroll = true;
            browserPanel.WrapContents = true;
            browserPanel.FlowDirection = FlowDirection.LeftToRight;
            browserPanel.BackColor = SystemColors.Window;
            browserPanel.BorderStyle = BorderStyle.FixedSingle;
            browserPanel.Padding = new Padding(8);

            foreach (BrowserInfo browser in browsers)
            {
                RadioButton option = CreateBrowserOption(browser);
                browserPanel.Controls.Add(option);
                if (selectedBrowser == null)
                {
                    option.Checked = true;
                    selectedBrowser = browser;
                }
            }

            Button settings = new Button();
            settings.Text = I18n.T("Settings");
            settings.Location = new Point(196, 286);
            settings.Size = new Size(82, 28);
            settings.Click += delegate
            {
                bool oldTopMost = TopMost;
                TopMost = false;
                settingsAction(this);
                TopMost = oldTopMost;
                DialogResult = DialogResult.Retry;
                Close();
            };

            Button once = new Button();
            once.Text = I18n.T("Once");
            once.Location = new Point(292, 286);
            once.Size = new Size(74, 28);
            once.Click += delegate
            {
                pickAction = PickAction.Once;
                DialogResult = DialogResult.OK;
                Close();
            };
            AcceptButton = once;

            Button always = new Button();
            always.Text = I18n.T("Always");
            always.Location = new Point(376, 286);
            always.Size = new Size(74, 28);
            always.Click += delegate
            {
                pickAction = PickAction.Always;
                DialogResult = DialogResult.OK;
                Close();
            };

            Button cancel = new Button();
            cancel.Text = I18n.T("Cancel");
            cancel.Location = new Point(468, 286);
            cancel.Size = new Size(74, 28);
            cancel.DialogResult = DialogResult.Cancel;
            CancelButton = cancel;

            Controls.Add(label);
            Controls.Add(urlBox);
            Controls.Add(browserPanel);
            Controls.Add(settings);
            Controls.Add(once);
            Controls.Add(always);
            Controls.Add(cancel);
        }

        private RadioButton CreateBrowserOption(BrowserInfo browser)
        {
            RadioButton option = new RadioButton();
            option.Appearance = Appearance.Button;
            option.Size = new Size(76, 72);
            option.Margin = new Padding(4);
            option.Text = browser.Name;
            option.Tag = browser;
            option.Image = Program.ExtractIconImage(browser.Path, 32);
            option.TextImageRelation = TextImageRelation.ImageAboveText;
            option.ImageAlign = ContentAlignment.TopCenter;
            option.TextAlign = ContentAlignment.BottomCenter;
            option.AutoEllipsis = true;
            tips.SetToolTip(option, browser.Name + Environment.NewLine + browser.Path);
            option.CheckedChanged += delegate
            {
                if (option.Checked)
                {
                    selectedBrowser = option.Tag as BrowserInfo;
                }
            };
            option.DoubleClick += delegate
            {
                option.Checked = true;
                selectedBrowser = option.Tag as BrowserInfo;
                pickAction = PickAction.Once;
                DialogResult = DialogResult.OK;
                Close();
            };
            return option;
        }

        internal static PickResult Pick(List<BrowserInfo> browsers, string url, string parentProcess, Action<Form> settingsAction)
        {
            while (true)
            {
                PickerForm form = new PickerForm(browsers, url, parentProcess, settingsAction);
                DialogResult result = form.ShowDialog();
                if (result == DialogResult.Retry) continue;
                if (result != DialogResult.OK) return null;
                if (form.selectedBrowser == null) return null;
                return new PickResult
                {
                    Action = form.pickAction,
                    Browser = form.selectedBrowser
                };
            }
        }
    }

    internal class CandidateForm : Form
    {
        private readonly CheckedListBox list;
        private readonly List<BrowserInfo> candidates;
        public List<BrowserInfo> SelectedBrowsers { get; private set; }

        public CandidateForm(List<BrowserInfo> candidates)
        {
            this.candidates = candidates;
            SelectedBrowsers = new List<BrowserInfo>();
            Text = I18n.T("CandidateTitle");
            Icon = Program.GetAppIcon();
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(620, 360);

            Label label = new Label();
            label.Text = I18n.T("CandidateLabel");
            label.Location = new Point(18, 16);
            label.Size = new Size(580, 24);

            list = new CheckedListBox();
            list.Location = new Point(18, 48);
            list.Size = new Size(584, 250);
            list.CheckOnClick = true;
            foreach (BrowserInfo browser in candidates)
            {
                list.Items.Add(browser.Name + " - " + browser.Path, false);
            }

            Button add = new Button();
            add.Text = I18n.T("AddSelected");
            add.Location = new Point(394, 314);
            add.Size = new Size(102, 28);
            add.DialogResult = DialogResult.OK;

            Button skip = new Button();
            skip.Text = I18n.T("Skip");
            skip.Location = new Point(508, 314);
            skip.Size = new Size(94, 28);
            skip.DialogResult = DialogResult.Cancel;

            AcceptButton = add;
            CancelButton = skip;
            Controls.Add(label);
            Controls.Add(list);
            Controls.Add(add);
            Controls.Add(skip);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SelectedBrowsers.Clear();
            for (int i = 0; i < list.CheckedIndices.Count; i++)
            {
                SelectedBrowsers.Add(candidates[list.CheckedIndices[i]]);
            }
            base.OnFormClosing(e);
        }
    }

    internal class SettingsForm : Form
    {
        public PickerConfig Config { get; private set; }
        private ListView browserList;
        private ListBox ruleList;
        private ComboBox languageCombo;
        private List<BrowserInfo> browsers;

        public SettingsForm(PickerConfig config)
        {
            Config = config;
            Text = I18n.T("SettingsTitle");
            Icon = Program.GetAppIcon();
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(800, 540);
            MinimumSize = new Size(800, 540);

            TabControl tabs = new TabControl();
            tabs.Location = new Point(10, 10);
            tabs.Size = new Size(780, 470);

            TabPage settingsTab = new TabPage(I18n.T("Settings"));
            TabPage aboutTab = new TabPage(I18n.T("About"));
            tabs.TabPages.Add(settingsTab);
            tabs.TabPages.Add(aboutTab);

            browserList = new ListView();
            browserList.Location = new Point(18, 18);
            browserList.Size = new Size(440, 372);
            browserList.View = View.Details;
            browserList.FullRowSelect = true;
            browserList.Columns.Add(I18n.T("Browser"), 150);
            browserList.Columns.Add(I18n.T("Path"), 260);

            Button addBrowser = new Button();
            addBrowser.Text = I18n.T("AddBrowser");
            addBrowser.Location = new Point(18, 404);
            addBrowser.Size = new Size(96, 28);
            addBrowser.Click += delegate { AddBrowser(); };

            Button removeBrowser = new Button();
            removeBrowser.Text = I18n.T("RemoveManual");
            removeBrowser.Location = new Point(124, 404);
            removeBrowser.Size = new Size(112, 28);
            removeBrowser.Click += delegate { RemoveManualBrowser(); };

            Button scan = new Button();
            scan.Text = I18n.T("Scan");
            scan.Location = new Point(246, 404);
            scan.Size = new Size(72, 28);
            scan.Click += delegate { ScanCandidates(); };

            Button setDefault = new Button();
            setDefault.Text = I18n.T("SetDefault");
            setDefault.Location = new Point(328, 404);
            setDefault.Size = new Size(88, 28);
            setDefault.Click += delegate { SetDefault(); };

            Button clearDefault = new Button();
            clearDefault.Text = I18n.T("Clear");
            clearDefault.Location = new Point(426, 404);
            clearDefault.Size = new Size(64, 28);
            clearDefault.Click += delegate { Config.defaultBrowserPath = ""; RefreshBrowsers(); };

            Label rulesLabel = new Label();
            rulesLabel.Text = I18n.T("ApplicationRules");
            rulesLabel.Location = new Point(510, 18);
            rulesLabel.Size = new Size(240, 22);

            ruleList = new ListBox();
            ruleList.Location = new Point(510, 46);
            ruleList.Size = new Size(272, 300);

            Button addRule = new Button();
            addRule.Text = I18n.T("AddRule");
            addRule.Location = new Point(510, 358);
            addRule.Size = new Size(84, 28);
            addRule.Click += delegate { AddRule(false); };

            Button askRule = new Button();
            askRule.Text = I18n.T("AskRule");
            askRule.Location = new Point(604, 358);
            askRule.Size = new Size(84, 28);
            askRule.Click += delegate { AddRule(true); };

            Button removeRule = new Button();
            removeRule.Text = I18n.T("Remove");
            removeRule.Location = new Point(698, 358);
            removeRule.Size = new Size(72, 28);
            removeRule.Click += delegate { RemoveRule(); };

            Label languageLabel = new Label();
            languageLabel.Text = I18n.T("Language");
            languageLabel.Location = new Point(510, 406);
            languageLabel.Size = new Size(80, 22);

            languageCombo = new ComboBox();
            languageCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            languageCombo.Location = new Point(596, 402);
            languageCombo.Size = new Size(174, 24);
            languageCombo.Items.Add(I18n.T("LangAuto"));
            languageCombo.Items.Add(I18n.T("LangEnglish"));
            languageCombo.Items.Add(I18n.T("LangChinese"));
            languageCombo.SelectedIndex = LanguageToIndex(Config.language);
            languageCombo.SelectedIndexChanged += delegate
            {
                Config.language = IndexToLanguage(languageCombo.SelectedIndex);
            };

            Button ok = new Button();
            ok.Text = I18n.T("Save");
            ok.Location = new Point(606, 498);
            ok.Size = new Size(74, 28);
            ok.Click += delegate { Config.language = IndexToLanguage(languageCombo.SelectedIndex); };
            ok.DialogResult = DialogResult.OK;

            Button cancel = new Button();
            cancel.Text = I18n.T("Cancel");
            cancel.Location = new Point(688, 498);
            cancel.Size = new Size(74, 28);
            cancel.DialogResult = DialogResult.Cancel;

            settingsTab.Controls.Add(browserList);
            settingsTab.Controls.Add(addBrowser);
            settingsTab.Controls.Add(removeBrowser);
            settingsTab.Controls.Add(scan);
            settingsTab.Controls.Add(setDefault);
            settingsTab.Controls.Add(clearDefault);
            settingsTab.Controls.Add(rulesLabel);
            settingsTab.Controls.Add(ruleList);
            settingsTab.Controls.Add(addRule);
            settingsTab.Controls.Add(askRule);
            settingsTab.Controls.Add(removeRule);
            settingsTab.Controls.Add(languageLabel);
            settingsTab.Controls.Add(languageCombo);
            aboutTab.Controls.Add(CreateAboutGroup());
            Controls.Add(tabs);
            Controls.Add(ok);
            Controls.Add(cancel);

            AcceptButton = ok;
            CancelButton = cancel;
            RefreshBrowsers();
            RefreshRules();
        }

        private GroupBox CreateAboutGroup()
        {
            GroupBox group = new GroupBox();
            group.Text = I18n.T("About");
            group.Location = new Point(14, 14);
            group.Size = new Size(738, 405);

            Label title = new Label();
            title.Text = I18n.T("AboutTitle") + Environment.NewLine + I18n.T("AboutVersion");
            title.Font = new Font(Font, FontStyle.Bold);
            title.Location = new Point(14, 24);
            title.Size = new Size(430, 42);

            RichTextBox description = CreateInfoText(I18n.T("AboutDescription"), new Point(14, 68), new Size(430, 92));

            LinkLabel github = new LinkLabel();
            github.Text = I18n.T("AboutGithub");
            github.Location = new Point(14, 168);
            github.Size = new Size(430, 20);
            github.LinkClicked += delegate { Process.Start("https://github.com/lttlz/LXBrowserPicker"); };

            Label author = new Label();
            author.Text = I18n.T("AboutAuthor");
            author.Location = new Point(14, 194);
            author.Size = new Size(430, 18);

            AddQrBlock(group, "wechat-contact.png", I18n.T("WechatContact"), 470);
            AddQrBlock(group, "wechat-support.png", I18n.T("WechatSupport"), 600);

            RichTextBox supportNote = CreateInfoText(I18n.T("AboutFree") + " " + I18n.T("AboutSupportNote"), new Point(14, 242), new Size(430, 76));

            group.Controls.Add(title);
            group.Controls.Add(description);
            group.Controls.Add(github);
            group.Controls.Add(author);
            group.Controls.Add(supportNote);
            return group;
        }

        private RichTextBox CreateInfoText(string text, Point location, Size size)
        {
            RichTextBox box = new RichTextBox();
            box.Location = location;
            box.Size = size;
            box.BorderStyle = BorderStyle.None;
            box.BackColor = SystemColors.Control;
            box.ReadOnly = true;
            box.TabStop = false;
            box.ScrollBars = RichTextBoxScrollBars.None;
            box.DetectUrls = false;
            box.ShortcutsEnabled = false;
            box.Rtf = BuildInfoRtf(text);
            return box;
        }

        private string BuildInfoRtf(string text)
        {
            return "{\\rtf1\\ansi\\deff0{\\fonttbl{\\f0 Microsoft YaHei UI;}}\\uc1\\pard\\f0\\fs18\\sl300\\slmult1 " + EscapeRtf(text) + "\\par}";
        }

        private string EscapeRtf(string text)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char ch in text)
            {
                if (ch == '\\')
                {
                    builder.Append("\\\\");
                }
                else if (ch == '{')
                {
                    builder.Append("\\{");
                }
                else if (ch == '}')
                {
                    builder.Append("\\}");
                }
                else if (ch == '\r')
                {
                }
                else if (ch == '\n')
                {
                    builder.Append("\\par ");
                }
                else if (ch <= 0x7f)
                {
                    builder.Append(ch);
                }
                else
                {
                    short value = unchecked((short)ch);
                    builder.Append("\\u");
                    builder.Append(value);
                    builder.Append("?");
                }
            }
            return builder.ToString();
        }

        private void AddQrBlock(Control parent, string fileName, string labelText, int x)
        {
            PictureBox picture = new PictureBox();
            picture.Location = new Point(x, 24);
            picture.Size = new Size(112, 112);
            picture.BorderStyle = BorderStyle.FixedSingle;
            picture.SizeMode = PictureBoxSizeMode.StretchImage;
            picture.Image = Program.LoadAboutImage(fileName, 112);

            Label label = new Label();
            label.Text = labelText;
            label.Location = new Point(x - 4, 142);
            label.Size = new Size(124, 24);
            label.TextAlign = ContentAlignment.MiddleCenter;

            parent.Controls.Add(picture);
            parent.Controls.Add(label);
        }

        private void RefreshBrowsers()
        {
            browsers = Program.PublicFindBrowsers(Config);
            browserList.Items.Clear();
            browserList.SmallImageList = Program.BuildImageList(browsers, 16);
            foreach (BrowserInfo browser in browsers)
            {
                string name = browser.Name;
                if (string.Equals(browser.Path, Config.defaultBrowserPath, StringComparison.OrdinalIgnoreCase))
                {
                    name += I18n.T("DefaultSuffix");
                }
                ListViewItem item = new ListViewItem(name);
                item.ImageKey = browser.Path;
                item.SubItems.Add(browser.Path);
                item.Tag = browser;
                browserList.Items.Add(item);
            }
        }

        private BrowserInfo SelectedBrowser()
        {
            if (browserList.SelectedItems.Count == 0) return null;
            return browserList.SelectedItems[0].Tag as BrowserInfo;
        }

        private void AddBrowser()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = I18n.T("ExeFilter");
            dialog.Title = I18n.T("AddBrowser");
            if (dialog.ShowDialog() != DialogResult.OK) return;
            string name = Program.PublicFindBrowsers(Config).Count.ToString();
            name = Path.GetFileNameWithoutExtension(dialog.FileName);
            Program.PublicAddManualBrowser(Config, name, dialog.FileName);
            RefreshBrowsers();
        }

        private void RemoveManualBrowser()
        {
            BrowserInfo selected = SelectedBrowser();
            if (selected == null) return;
            if (!selected.Manual)
            {
                MessageBox.Show(I18n.T("AutoBrowserCannotRemove"), Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            for (int i = Config.manualBrowsers.Count - 1; i >= 0; i--)
            {
                if (string.Equals(Config.manualBrowsers[i].path, selected.Path, StringComparison.OrdinalIgnoreCase))
                {
                    Config.manualBrowsers.RemoveAt(i);
                }
            }
            if (string.Equals(Config.defaultBrowserPath, selected.Path, StringComparison.OrdinalIgnoreCase)) Config.defaultBrowserPath = "";
            RefreshBrowsers();
        }

        private void ScanCandidates()
        {
            List<BrowserInfo> candidates = Program.PublicFindCandidateBrowsers(browsers);
            if (candidates.Count == 0)
            {
                MessageBox.Show(I18n.T("NoCandidates"), Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            CandidateForm form = new CandidateForm(candidates);
            if (form.ShowDialog() == DialogResult.OK)
            {
                foreach (BrowserInfo browser in form.SelectedBrowsers)
                {
                    Program.PublicAddManualBrowser(Config, browser.Name, browser.Path);
                }
                RefreshBrowsers();
            }
        }

        private void SetDefault()
        {
            BrowserInfo selected = SelectedBrowser();
            if (selected == null) return;
            Config.defaultBrowserPath = selected.Path;
            RefreshBrowsers();
        }

        private void RefreshRules()
        {
            ruleList.Items.Clear();
            foreach (AppRule rule in Config.appRules)
            {
                BrowserInfo browser = null;
                List<BrowserInfo> current = Program.PublicFindBrowsers(Config);
                string browserName = I18n.T("AskEveryTime");
                if (!string.Equals(rule.browserPath, "ASK", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (BrowserInfo item in current)
                    {
                        if (string.Equals(item.Path, rule.browserPath, StringComparison.OrdinalIgnoreCase)) browser = item;
                    }
                    browserName = browser == null ? rule.browserPath : browser.Name;
                }
                ruleList.Items.Add(rule.process + " -> " + browserName);
            }
        }

        private void AddRule(bool askEveryTime)
        {
            BrowserInfo selected = SelectedBrowser();
            if (!askEveryTime && selected == null)
            {
                MessageBox.Show(I18n.T("SelectBrowserFirst"), Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string process = PromptForm.Ask(I18n.T("AppProcessTitle"), I18n.T("AppProcessPrompt"));
            if (string.IsNullOrWhiteSpace(process)) return;

            for (int i = Config.appRules.Count - 1; i >= 0; i--)
            {
                if (string.Equals(Config.appRules[i].process, process, StringComparison.OrdinalIgnoreCase))
                {
                    Config.appRules.RemoveAt(i);
                }
            }
            Config.appRules.Add(new AppRule { process = process.Trim(), browserPath = askEveryTime ? "ASK" : selected.Path });
            RefreshRules();
        }

        private void RemoveRule()
        {
            if (ruleList.SelectedIndex < 0) return;
            Config.appRules.RemoveAt(ruleList.SelectedIndex);
            RefreshRules();
        }

        private int LanguageToIndex(string language)
        {
            if (string.Equals(language, "en-US", StringComparison.OrdinalIgnoreCase)) return 1;
            if (string.Equals(language, "zh-CN", StringComparison.OrdinalIgnoreCase)) return 2;
            return 0;
        }

        private string IndexToLanguage(int index)
        {
            if (index == 1) return "en-US";
            if (index == 2) return "zh-CN";
            return "auto";
        }
    }

    internal class PromptForm : Form
    {
        private TextBox input;

        public static string Ask(string title, string prompt)
        {
            PromptForm form = new PromptForm(title, prompt);
            return form.ShowDialog() == DialogResult.OK ? form.input.Text : "";
        }

        private PromptForm(string title, string prompt)
        {
            Text = title;
            Icon = Program.GetAppIcon();
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(420, 130);
            Label label = new Label();
            label.Text = prompt;
            label.Location = new Point(14, 14);
            label.Size = new Size(390, 24);
            input = new TextBox();
            input.Location = new Point(14, 46);
            input.Size = new Size(390, 24);
            Button ok = new Button();
            ok.Text = I18n.T("OK");
            ok.Location = new Point(248, 86);
            ok.Size = new Size(74, 28);
            ok.DialogResult = DialogResult.OK;
            Button cancel = new Button();
            cancel.Text = I18n.T("Cancel");
            cancel.Location = new Point(330, 86);
            cancel.Size = new Size(74, 28);
            cancel.DialogResult = DialogResult.Cancel;
            Controls.Add(label);
            Controls.Add(input);
            Controls.Add(ok);
            Controls.Add(cancel);
            AcceptButton = ok;
            CancelButton = cancel;
        }
    }
}
