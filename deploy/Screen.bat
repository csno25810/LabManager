@echo off
REM 研究室テレビPC 起動スクリプト（旧 Screen.bat 相当）
REM 設定ファイル: C:\MyReader\SQLReader.ini, GoogleCalenderReader.ini
cd /d "%~dp0"

if not exist "LabManager.exe" (
    echo [ERROR] LabManager.exe が見つかりません。
    echo bin\Release のビルド成果物をこのフォルダにコピーしてください。
    pause
    exit /b 1
)

start "" "LabManager.exe" /tv
