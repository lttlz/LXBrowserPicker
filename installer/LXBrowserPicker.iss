#define MyAppName "LXBrowserPicker"
#define MyAppVersion "0.5.2"
#define MyAppPublisher "LX"
#define MyAppExeName "LXBrowserPicker.exe"

[Setup]
AppId={{7D3F6F53-1BAE-4BA3-A27C-98A7CF4E5D6D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputDir=..
OutputBaseFilename=LXBrowserPickerSetup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
SetupIconFile=..\assets\LXBrowserPicker.ico
PrivilegesRequired=admin
ChangesAssociations=yes
UninstallDisplayIcon={app}\{#MyAppExeName}
CloseApplications=yes

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "zh"; MessagesFile: "ChineseSimplified.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\LXBrowserPicker.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\lx-browser-picker.config.example.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\README.zh-CN.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\CHANGELOG.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Registry]
Root: HKLM; Subkey: "Software\Clients\StartMenuInternet\LXBrowserPicker"; ValueType: string; ValueName: ""; ValueData: "LXBrowserPicker"; Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\Clients\StartMenuInternet\LXBrowserPicker\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\LXBrowserPicker.exe,0"
Root: HKLM; Subkey: "Software\Clients\StartMenuInternet\LXBrowserPicker\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\LXBrowserPicker.exe"" ""%1"""
Root: HKLM; Subkey: "Software\Clients\StartMenuInternet\LXBrowserPicker\Capabilities"; ValueType: string; ValueName: "ApplicationName"; ValueData: "LXBrowserPicker"
Root: HKLM; Subkey: "Software\Clients\StartMenuInternet\LXBrowserPicker\Capabilities"; ValueType: string; ValueName: "ApplicationDescription"; ValueData: "Choose which browser opens external links."
Root: HKLM; Subkey: "Software\Clients\StartMenuInternet\LXBrowserPicker\Capabilities"; ValueType: string; ValueName: "ApplicationIcon"; ValueData: "{app}\LXBrowserPicker.exe,0"
Root: HKLM; Subkey: "Software\Clients\StartMenuInternet\LXBrowserPicker\Capabilities\URLAssociations"; ValueType: string; ValueName: "http"; ValueData: "LXBrowserPickerURL"
Root: HKLM; Subkey: "Software\Clients\StartMenuInternet\LXBrowserPicker\Capabilities\URLAssociations"; ValueType: string; ValueName: "https"; ValueData: "LXBrowserPickerURL"
Root: HKLM; Subkey: "Software\Clients\StartMenuInternet\LXBrowserPicker\Capabilities\FileAssociations"; ValueType: string; ValueName: ".htm"; ValueData: "LXBrowserPickerHTML"
Root: HKLM; Subkey: "Software\Clients\StartMenuInternet\LXBrowserPicker\Capabilities\FileAssociations"; ValueType: string; ValueName: ".html"; ValueData: "LXBrowserPickerHTML"
Root: HKLM; Subkey: "Software\RegisteredApplications"; ValueType: string; ValueName: "LXBrowserPicker"; ValueData: "Software\Clients\StartMenuInternet\LXBrowserPicker\Capabilities"; Flags: uninsdeletevalue
Root: HKLM; Subkey: "Software\Classes\LXBrowserPickerURL"; ValueType: string; ValueName: ""; ValueData: "LXBrowserPicker URL"; Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\Classes\LXBrowserPickerURL"; ValueType: string; ValueName: "URL Protocol"; ValueData: ""
Root: HKLM; Subkey: "Software\Classes\LXBrowserPickerURL\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\LXBrowserPicker.exe,0"
Root: HKLM; Subkey: "Software\Classes\LXBrowserPickerURL\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\LXBrowserPicker.exe"" ""%1"""
Root: HKLM; Subkey: "Software\Classes\LXBrowserPickerHTML"; ValueType: string; ValueName: ""; ValueData: "LXBrowserPicker HTML"; Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\Classes\LXBrowserPickerHTML\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\LXBrowserPicker.exe,0"
Root: HKLM; Subkey: "Software\Classes\LXBrowserPickerHTML\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\LXBrowserPicker.exe"" ""%1"""

[Code]
function InitialAppLanguage(): String;
begin
  if ActiveLanguage = 'zh' then
    Result := 'zh-CN'
  else
    Result := 'en-US';
end;

procedure CreateInitialConfig();
var
  ConfigPath: String;
  ConfigText: String;
begin
  ConfigPath := ExpandConstant('{app}\lx-browser-picker.config.json');
  if FileExists(ConfigPath) then
    Exit;

  ConfigText :=
    '{'#13#10 +
    '  "defaultBrowserPath": "",'#13#10 +
    '  "firstRunScanDone": false,'#13#10 +
    '  "defaultAppGuideCompleted": false,'#13#10 +
    '  "language": "' + InitialAppLanguage() + '",'#13#10 +
    '  "manualBrowsers": [],'#13#10 +
    '  "appRules": []'#13#10 +
    '}'#13#10;
  SaveStringToFile(ConfigPath, ConfigText, False);
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
    CreateInitialConfig();
end;
