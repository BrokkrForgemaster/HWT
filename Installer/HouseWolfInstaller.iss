; ---------------------------------------------------------------------------
; HouseWolfInstaller.iss
; ---------------------------------------------------------------------------
#define MyAppName        "House Wolf App"
#define MyAppVersion     "0.1.1"
#define MyAppPublisher   "House Wolf Org"
#define MyAppExeName     "HWT.Presentation.exe"
#define InstallerIcon    "Installer\Icons\installer.ico"
#define DesktopIcon      "Installer\Icons\app-desktop.ico"

[Setup]
; Unique ID—generate a new GUID for your real installer.
AppId={{7B9A5E4F-A8C3-4D2D-9F14-1234567890AB}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}

; Custom icon for the installer window:
SetupIconFile={#InstallerIcon}

; Output installer filename (relative to this script’s folder):
OutputDir=.
OutputBaseFilename={#MyAppName}_Installer_v{#MyAppVersion}

Compression=lzma
SolidCompression=yes

; ---------------------------------------------------------------------------
; Files to include in the installer
; ---------------------------------------------------------------------------
[Files]
; Copy everything under your published folder into {app}\ (e.g. Program Files\House Wolf App\)
; The Published folder is assumed to be at:
;   <repo-root>\HWT.Presentation\bin\Release\net9.0-windows\win-x64\publish\*
; If you used /p:PublishDir, adjust the path accordingly.

Source: "..\HWT.Presentation\bin\Release\net9.0-windows\win-x64\publish\*"; \
  DestDir: "{app}"; \
  Flags: ignoreversion recursesubdirs createallsubdirs; \
  Components: main

; Also copy your desktop‐shortcut icon into the install dir (so Inno can reference it later):
Source: "{#DesktopIcon}"; DestDir: "{app}"; Flags: ignoreversion

; ---------------------------------------------------------------------------
; Tasks: define optional desktop shortcut creation
; ---------------------------------------------------------------------------
[Tasks]
Name: "desktopicon";\
  Description: "Create a &desktop icon";\
  GroupDescription: "Additional icons";\
  Flags: unchecked

; ---------------------------------------------------------------------------
; Icons: start menu & desktop shortcuts
; ---------------------------------------------------------------------------
[Icons]
; Start‐menu shortcut
Name: "{group}\{#MyAppName}"; \
  Filename: "{app}\{#MyAppExeName}"; \
  WorkingDir: "{app}"; \
  IconFilename: "{app}\{#DesktopIcon}"; \
  Components: main

; Desktop shortcut (only if user checks the box)
Name: "{commondesktop}\{#MyAppName}"; \
  Filename: "{app}\{#MyAppExeName}"; \
  Tasks: desktopicon; \
  WorkingDir: "{app}"; \
  IconFilename: "{app}\{#DesktopIcon}"; \
  Components: main

; ---------------------------------------------------------------------------
; Uninstaller settings (optional)
; ---------------------------------------------------------------------------
[UninstallDelete]
Type: files; Name: "{app}\{#DesktopIcon}"
