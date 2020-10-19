$idAccount = $args[0]

if ($null -eq $idAccount) {
  Write-Host "Krok 1: Wyloguj się z aplikacji MS Teams"
  [int]$accountNumber = Read-Host "Ile kont?"
  for ($i = 0 ; $i -le $accountNumber; $i++) {

    if ($i -ne 0) {
      Copy-Item "$env:APPDATA\Microsoft\Teams" -Destination "$env:APPDATA\Microsoft\Teams$i" -Recurse
      Copy-Item "$env:LOCALAPPDATA\Microsoft\Teams" -Destination "$env:LOCALAPPDATA\Microsoft\Teams$i" -Recurse
    }

    $WshShell = New-Object -comObject WScript.Shell
    $Shortcut = $WshShell.CreateShortcut("$Home\Desktop\Teamsy\TeamsyKonto" + $i + ".lnk")
    $Shortcut.TargetPath = "powershell.exe" 
    $Shortcut.Arguments = "-noexit -ExecutionPolicy Bypass -File ""$PSScriptRoot\manager.ps1"" $i"

    $Shortcut.Save()
  }
  
}
else {

  $SettingsObject = Get-Content -Path 'settings.json' | ConvertFrom-Json
  $oldid = $SettingsObject.actualid


  if ($oldid -eq $idAccount) {
    Write-Host "Aktualnie zalogowane jest to samo konto, brak zmiany"
    & "C:\Users\bartu\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Microsoft Teams.lnk"
  }
  else {
    Write-Host "Proba zmiany konta na $idAccount, aktualne to: $oldid"
    Write-Host "Zabijam process MS-Teams"
    Stop-Process -Name "Teams"
    Start-Sleep -Seconds 2

    # REMOVE OLD LAST_ID BACKUP
    #Write-Host "Usuwanie starej kopii $oldid"
    #Remove-Item -Path "$env:APPDATA\Microsoft\Teams$oldid" -Recurse
    #Remove-Item -Path "$env:LOCALAPPDATA\Microsoft\Teams$oldid" -Recurse

    # CREATE LAST_ID BACKUP
    Write-Host "Tworzenie nowej kopii $oldid"
    #Copy-Item "$env:APPDATA\Microsoft\Teams" -Destination "$env:APPDATA\Microsoft\Teams$oldid" -Recurse
    #Copy-Item "$env:LOCALAPPDATA\Microsoft\Teams" -Destination "$env:LOCALAPPDATA\Microsoft\Teams$oldid" -Recurse
    Rename-Item "$env:APPDATA\Microsoft\Teams" "$env:APPDATA\Microsoft\Teams$oldid" 
    Rename-Item "$env:LOCALAPPDATA\Microsoft\Teams" "$env:LOCALAPPDATA\Microsoft\Teams$oldid" 

    # REMOVE LAST_ID CURRENT FILES
    #Write-Host "Usuwanie aktualnych plikow $oldid"
    #Remove-Item -Path "$env:APPDATA\Microsoft\Teams\" -Recurse
    #Remove-Item -Path "$env:LOCALAPPDATA\Microsoft\Teams\" -Recurse

    # COPY BACKUP FILES OF NEW ID
    Write-Host "Przywracanie kopii $idAccount"
    #Copy-Item "$env:APPDATA\Microsoft\Teams$idAccount" -Destination "$env:APPDATA\Microsoft\Teams" -Recurse
    #Copy-Item "$env:LOCALAPPDATA\Microsoft\Teams$idAccount" -Destination "$env:LOCALAPPDATA\Microsoft\Teams" -Recurse
    Rename-Item "$env:APPDATA\Microsoft\Teams$idAccount" "$env:APPDATA\Microsoft\Teams" 
    Rename-Item "$env:LOCALAPPDATA\Microsoft\Teams$idAccount" "$env:LOCALAPPDATA\Microsoft\Teams" 


    $SettingsObject.actualid = $idAccount
    $SettingsObject | ConvertTo-Json -depth 1 | Set-Content 'settings.json';


    Write-Host "Uruchamiam proces MS-Teams"
    & "C:\Users\bartu\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Microsoft Teams.lnk"

  }
}