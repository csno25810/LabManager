@echo off
chcp 65001 >nul
cd /d "%~dp0"
echo.
echo === 研究室 DB 情報取得 (Phase 0) ===
echo.

where py >nul 2>&1
if %errorlevel%==0 (
    py -3 lab_phase0.py
    goto :end
)

where python >nul 2>&1
if %errorlevel%==0 (
    python lab_phase0.py
    goto :end
)

echo Python が見つかりません。
echo 「その他\タッチする.py」が動く Python を使ってください。
echo 例: py -3 lab_phase0.py
pause
goto :end

:end
