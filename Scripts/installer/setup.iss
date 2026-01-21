; Script generated for Magidesk POS
; Requires Inno Setup 6.0+

#define AppName "Magidesk POS"
#define AppVersion "1.0.0"
#define AppPublisher "Magidesk"
#define AppExeName "Magidesk.Presentation.exe"
#define AppId "{{GUID-GOES-HERE}}"

[Setup]
AppId={#AppId}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
OutputBaseFilename=MagideskInstaller
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Dirs]
; Create ProgramData folder for writable config/logs
Name: "{commonappdata}\Magidesk"; Permissions: users-modify
Name: "{commonappdata}\Magidesk\Logs"; Permissions: users-modify

[Files]
; 1. Core Application Bundle (Publish Output)
Source: "..\..\src\Magidesk.Presentation\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

; 2. Default Config
Source: "..\..\src\Magidesk.Presentation\Configuration\appsettings.defaults.json"; DestDir: "{app}"; Flags: ignoreversion

; 3. Automation Tools & Bundle
Source: "..\..\redist\efbundle.exe"; DestDir: "{app}\tools"; Flags: ignoreversion
Source: "..\install_db.ps1"; DestDir: "{app}\tools"; Flags: ignoreversion
Source: "..\apply_migrations.ps1"; DestDir: "{app}\tools"; Flags: ignoreversion
Source: "..\db\*.sql"; DestDir: "{app}\tools\db"; Flags: ignoreversion

[Run]
; 1. Install Database (Silent) - Passes in parameters from Wizard (see Code section)
Filename: "powershell.exe"; \
  Parameters: "-ExecutionPolicy Bypass -File ""{app}\tools\install_db.ps1"" -DbHost ""{code:GetDbHost}"" -DbUser ""{code:GetDbUser}"" -DbPass ""{code:GetDbPass}"" -DbName ""{code:GetDbName}"""; \
  Flags: runhidden waituntilterminated; StatusMsg: "Initializing Database..."; \
  Description: "Initialize Database"

; 2. Apply Migrations (Silent)
Filename: "powershell.exe"; \
  Parameters: "-ExecutionPolicy Bypass -File ""{app}\tools\apply_migrations.ps1"" -DbHost ""{code:GetDbHost}"" -DbUser ""{code:GetDbUser}"" -DbPass ""{code:GetDbPass}"" -DbName ""{code:GetDbName}"""; \
  Flags: runhidden waituntilterminated; StatusMsg: "Upgrading Database..."; \
  Description: "Apply Database Updates"

; 3. Launch App
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
var
  DbHostPage: TInputQueryWizardPage;
  DbHost, DbUser, DbPass, DbName: String;

procedure InitializeWizard;
begin
  // Create Custom Page for Database Info
  DbHostPage := CreateInputQueryPage(wpSelectDir,
    'Database Configuration', 'Please enter your PostgreSQL connection details.',
    'These will be used to initialize the database and configure the application.');
  
  DbHostPage.Add('Host:', False);
  DbHostPage.Add('Port:', False);
  DbHostPage.Add('Database Name:', False);
  DbHostPage.Add('Username:', False);
  DbHostPage.Add('Password:', True); // Masked input

  // Defaults
  DbHostPage.Values[0] := 'localhost';
  DbHostPage.Values[1] := '5432';
  DbHostPage.Values[2] := 'magidesk_prod';
  DbHostPage.Values[3] := 'postgres';
end;

function GetDbHost(Param: String): String;
begin
  Result := DbHostPage.Values[0];
end;

function GetDbUser(Param: String): String;
begin
  Result := DbHostPage.Values[3];
end;

function GetDbPass(Param: String): String;
begin
  Result := DbHostPage.Values[4];
end;

function GetDbName(Param: String): String;
begin
  Result := DbHostPage.Values[2];
end;

// On "Next" click after DB Page, we could add connection testing logic here.
