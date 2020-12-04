rem - CTR-tools ISO-rebuild

rem - _INIT.BAT: Crash Team Racing ISO rebuild batch file
rem - --- use Notepad++ for nice syntax highlighting ---

rem - !!! variables !!! change, if needed !!!

	set ctr_iso=ctr_rebuild
	set src_path=ctr_source
	set big_name=bigfile


rem - sets current dir

	cd /d "%~dp0"


rem - moving data file to root folder

	copy %src_path%\%big_name%.big %big_name%.big /y


rem - extracting bigfile big

	tools\ctrtools\bigtool %~dp0%big_name%.big


rem - deleting unknown md5 file

	del unknown_md5.txt

