rem - CTR-tools ISO-rebuild

rem - _BUILD.BAT: Crash Team Racing ISO rebuild batch file
rem - --- use Notepad++ for nice syntax highlighting ---

rem - !!! variables !!! change, if needed !!!

	set epsxe_path="D:\Games\Emu\epsxe 2.0.5\epsxe.exe"

	set ctr_iso=ctr_rebuild
	set src_path=ctr_source
	set big_name=bigfile


rem - licensing region /us, /eu or /jp

	set region=/us


rem - sets current dir

	cd /d "%~dp0"


rem - assembling bigtool.big

	tools\ctrtools\bigtool %big_name%.txt


rem - move data file to source folder

	move /y %big_name%.big %src_path%


rem - building XML tree of source directory only if xml is not present!
	if not exist %src_path%.xml ( 
		tools\mkpsxiso\mkisoxml %src_path% -o %src_path%.xml
	)

rem - building ISO

	tools\mkpsxiso\mkpsxiso %src_path%.xml -y -q -o %ctr_iso%.bin


rem - licensing ISO

	tools\psxlicense\psxlicense %region% /i %ctr_iso%.bin


rem - done, launch epsxe

	start "epsxe" %epsxe_path% -nogui -loadbin %ctr_iso%.bin

