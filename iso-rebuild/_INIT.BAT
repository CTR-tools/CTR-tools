@echo off

rem - _INIT.BAT: Crash Team Racing ISO rebuild batch file
rem - --- use Notepad++ for nice syntax highlighting ---

echo CTR-tools ISO-rebuild script by dcxdemo


rem - !!! variables !!! change, if needed !!!

	set rebuild=ctr_rebuild
	set big_name=bigfile

	set dumpiso=%~dp0\tools\mkpsxiso\dumpsxiso.exe
	set bigtool=%~dp0\tools\ctrtools\bigtool.exe

echo current directory: %~dp0

	cd /d "%~dp0"


echo checking tools...

	if not exist %dumpiso% (
		echo Error! No mkpsxiso found.
		goto :end
	)

	if not exist %bigtool% (
		echo Error! No bigtool found.
		goto :end
	)

echo OK!


rem - checking param

	if "%~1"=="" (
		echo Pass BIN/ISO image as a parameter for this script.
		goto :end
	)

	if not exist %1 (
		echo File doesn't exist.
		goto :end
	)


echo dumping iso files

	%dumpiso% %1 -x "%~dp0\%rebuild%" -s "%~dp0\%rebuild%.xml"


echo dumping bigfile contents

	%bigtool% "%~dp0\%rebuild%\%big_name%.big"


echo Work complete!

:end