[Setup]
AppName=House Wolf
AppVersion=0.1.0-alpha
DefaultDirName={autopf}\House Wolf
OutputBaseFilename=HouseWolfInstaller_v0.1.0-alpha
Compression=lzma
SolidCompression=yes
SetupIconFile=HWT.Presentation\bin\Release\net9.0-windows\win-x64\publish\Assets\housewolf.ico

[Files]
Source: "HWT.Presentation\bin\Release\net9.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "HWT.Presentation\bin\Release\net9.0-windows\win-x64\publish\Assets\housewolf.ico"; DestDir: "{app}"

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop icon"; GroupDescription: "AdditionalIcons"; Flags: unchecked

[Icons]
Name: "{group}\House Wolf"; Filename: "{app}\HWT.Presentation.exe"
Name: "{commondesktop}\House Wolf"; Filename: "{app}\HWT.Presentation.exe"; Tasks: desktopicon; IconFilename: {app}\housewolf.ico


