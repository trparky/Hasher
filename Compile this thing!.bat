MSBuild.exe "Hasher.sln" /noconsolelogger /t:Rebuild /p:Configuration=Debug
MSBuild.exe "Hasher.sln" /noconsolelogger /t:Rebuild /p:Configuration=Release
copy "Hasher\bin\Release\Hasher.exe" "%OneDrive%\Utilities\Hasher.exe"
cd Hasher\bin\Release
call "Package This Program!.bat"