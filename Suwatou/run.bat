@echo off
setlocal

set "ZIP_NAME=Suwatou"

echo Removing %ZIP_NAME%.zip...
if exist "%ZIP_NAME%.zip" (
    del "%ZIP_NAME%.zip"
    echo %ZIP_NAME%.zip removed.
) else (
    echo %ZIP_NAME%.zip does not exist.
)

echo.
echo Removing Release folder...
if exist "Release" (
    rmdir /s /q "Release"
    echo Release folder removed.
) else (
    echo Release folder does not exist.
)

echo.
echo Publishing project...
dotnet publish -c Release /p:PublishProfile=FolderProfile

echo.
echo Creating %ZIP_NAME%.zip...
7z a "%ZIP_NAME%.zip" ./Release/*

endlocal