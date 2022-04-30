@echo off

rem - _BUILD.BAT: Crash Team Racing ISO rebuild batch file
rem - --- use Notepad++ for nice syntax highlighting ---

echo CTR-tools ISO-rebuild script by dcxdemo


rem - !!! variables !!! change, if needed !!!

	rem - Your emulator location here, put full path to your emu here
	set epsxe_path="epsxe.exe"
	set duck_path="duckstation.exe"

	set rebuild=ctr_rebuild
	set big_name=bigfile

	set makeiso=%~dp0\tools\mkpsxiso\mkpsxiso.exe
	set bigtool=%~dp0\tools\ctrtools\bigtool.exe


echo current directory: %~dp0

	cd /d "%~dp0\%rebuild%\"


echo checking tools...

	if not exist %makeiso% (
		echo Error! No mkpsxiso found.
		goto :end
	)

	if not exist %bigtool% (
		echo Error! No bigtool found.
		goto :end
	)

echo OK!


echo rebuilding bigfile

	%bigtool% "%big_name%.txt"


rem - get back to root

	cd /d "%~dp0"


echo rebuilding ISO

	%makeiso% "%~dp0\%rebuild%.xml" -y -q -o "%~dp0\%rebuild%.bin"


rem - maybe we've got an emulator?

	if exist %duck_path% (
		echo launching Duckstation
		start "duckstation" %duck_path% "%~dp0\%rebuild%.bin"
		goto :end
	)

	if exist %epsxe_path% (
		echo launching ePSXe
		start "epsxe" %epsxe_path% -nogui -loadbin "%~dp0\%rebuild%.bin"
		goto :end
	)


echo Work complete!

:end