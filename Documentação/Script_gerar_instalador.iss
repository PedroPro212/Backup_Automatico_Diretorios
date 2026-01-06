; ---------------------------------
; Instalador Automatizar Backup
; ---------------------------------

[Setup]
AppId={{A1F8D9C2-7E6F-4C6D-9E4A-9A0F5B6E2C01}}
AppName=Automatizar Backup
AppVersion=1.0.0
AppPublisher=Programação Diária
DefaultDirName={autopf}\Automatizar Backup
DefaultGroupName=Automatizar Backup
OutputDir=C:\Users\Pedro\Desktop
OutputBaseFilename=Instalador_Automatizar_Backup
SetupIconFile=C:\Codigos\Backup_Automatico_Diretorios\Backup_Automatico_HdExterno\Backup_Automatico_HdExterno\logo.ico
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
WizardStyle=modern

[Languages]
Name: "portuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "Criar atalho na área de trabalho"; GroupDescription: "Atalhos:"

[Files]
; Pasta publicada do sistema
Source: "C:\Codigos\Backup_Automatico_Diretorios\Backup_Automatico_HdExterno\Backup_Automatico_HdExterno\bin\Release\*"; \
DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Automatizar Backup"; Filename: "{app}\Backup_Automatico_HdExterno.exe"
Name: "{commondesktop}\Automatizar Backup"; Filename: "{app}\Backup_Automatico_HdExterno.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\Backup_Automatico_HdExterno.exe"; \
Description: "Executar Automatizar Backup"; \
Flags: nowait postinstall skipifsilent
