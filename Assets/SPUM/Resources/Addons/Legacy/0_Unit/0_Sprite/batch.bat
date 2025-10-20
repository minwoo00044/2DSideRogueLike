@echo off
setlocal enabledelayedexpansion

REM CSV 파일 이름 설정
set "OutputFile=png_list_simple.csv"

REM CSV 파일 헤더 수정 ("FileName"만 포함)
echo "PartType","FileName" > %OutputFile%

REM 현재 폴더의 모든 하위 폴더를 대상으로 반복
for /d %%d in (*) do (
    REM 폴더 이름에서 PartType 추출
    for /f "tokens=1,* delims=_" %%a in ("%%~nd") do (
        set "PartType=%%b"
    )

    REM 하위 폴더 안의 .png 파일 정보 추출
    for %%f in ("%%d\*.png") do (
        REM PartType과 확장자(.png)를 뺀 파일 이름만 기록
        echo "!PartType!","%%~nf" >> %OutputFile%
    )
)

echo.
echo %OutputFile% 파일 생성이 완료되었습니다.
pause