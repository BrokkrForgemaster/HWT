; ---------------------------------------------------------------------------
; HouseWolfInstaller.iss
; ---------------------------------------------------------------------------
#define MyAppName        "Pack Tracker"
#define MyAppVersion     "0.1.5"
#define MyAppPublisher   "House Wolf Org"
#define MyAppExeName     "HWT.Presentation.exe"
#define InstallerIcon    "Icons/installer.ico"
#define DesktopIcon      "Icons/app-desktop.ico"

[Setup]
; Unique ID—generate a new GUID for your real installer.
AppId={{7B9A5E4F-A8C3-4D2D-9F14-1234567890AB}}  ; <-- Added missing closing brace
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}

; Custom icon for the installer window:
SetupIconFile={#InstallerIcon}

; Output installer filename (relative to this script’s folder):
; The compiled EXE will be placed in “HWT\” under the folder containing .iss
OutputDir=HWT
OutputBaseFilename={#MyAppName}-v{#MyAppVersion}

Compression=lzma
SolidCompression=yes

; ---------------------------------------------------------------------------
; Files to include in the installer
; ---------------------------------------------------------------------------
[Files]
; Copy everything under the published win-x64 folder into {app}\
; (.iss is in C:\dev\HWT\Installer\, so we go up one to C:\dev\HWT\ then down into HWT.Presentation\bin\…)
Source: "..\HWT.Presentation\bin\Release\net9.0-windows\win-x64\*"; \
  DestDir: "{app}"; \
  Flags: ignoreversion recursesubdirs createallsubdirs

; Also copy your desktop shortcut icon into the install dir (so Inno can reference it later):
Source: "{#DesktopIcon}"; DestDir: "{app}"; Flags: ignoreversion

; ---------------------------------------------------------------------------
; Tasks: define optional desktop shortcut creation
; ---------------------------------------------------------------------------
[Tasks]
Name: "desktopicon"; \
  Description: "Create a &desktop icon"; \
  GroupDescription: "Additional icons"; \
  Flags: unchecked

; ---------------------------------------------------------------------------
; Icons: start menu & desktop shortcuts
; ---------------------------------------------------------------------------
[Icons]
; Start-menu shortcut
Name: "{group}\{#MyAppName}"; \
  Filename: "{app}\{#MyAppExeName}"; \
  WorkingDir: "{app}"; \
  IconFilename: "{app}\installer.ico"

; Desktop shortcut (only if user checks the box)
Name: "{commondesktop}\{#MyAppName}"; \
  Filename: "{app}\{#MyAppExeName}"; \
  Tasks: desktopicon; \
  WorkingDir: "{app}"; \
  IconFilename: "{app}\app-desktop.ico"

; ---------------------------------------------------------------------------
; Uninstaller settings (optional)
; ---------------------------------------------------------------------------
[UninstallDelete]
Type: files; Name: "{app}\{#DesktopIcon}"
Type: filesandordirs; Name: "{app}\assets"