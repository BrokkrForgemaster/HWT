; ---------------------------------------------------------------------------
; HouseWolfInstaller.iss — Installer for Pack Tracker by House Wolf Org
; ---------------------------------------------------------------------------

#define MyAppName        "Pack Tracker"
#define MyAppVersion     "0.1.6"
#define MyAppPublisher   "House Wolf Org"
#define MyAppExeName     "HWT.Presentation.exe"
#define InstallerIcon    "Icons\\installer.ico"
#define DesktopIcon      "Icons\\app-desktop.ico"

[Setup]
AppId={{7B9A5E4F-A8C3-4D2D-9F14-1234567890AB}} ; GUID — generate a new one for production!
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=HWT
OutputBaseFilename={#MyAppName}-v{#MyAppVersion}
Compression=lzma
SolidCompression=yes
SetupIconFile={#InstallerIcon} 
DisableProgramGroupPage=yes
ArchitecturesInstallIn64BitMode=x64

[Files]
Source: "..\\HWT.Presentation\\bin\\Release\\net9.0-windows\\win-x64\\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#InstallerIcon}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DesktopIcon}"; DestDir: "{app}"; Flags: ignoreversion

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop icon"; GroupDescription: "Additional icons"; Flags: unchecked

[Icons]
; Start Menu Shortcut
Name: "{group}\\{#MyAppName}"; Filename: "{app}\\{#MyAppExeName}"; WorkingDir: "{app}"; IconFilename: "{app}\\installer.ico"

; Optional Desktop Shortcut
Name: "{commondesktop}\\{#MyAppName}"; Filename: "{app}\\{#MyAppExeName}"; Tasks: desktopicon; WorkingDir: "{app}"; IconFilename: "{app}\\app-desktop.ico"

[UninstallDelete]
; Remove desktop icon during uninstall
Type: files; Name: "{app}\\app-desktop.ico"
; Remove embedded installer icon copy
Type: files; Name: "{app}\\installer.ico"
; Remove app assets folder
Type: filesandordirs; Name: "{app}\\assets"
