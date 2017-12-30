
note: run these from the "Silo" folder in powershell

=================
START
=================

	cd E:\Source\gh\dotnet\orleans\Samples\jose\Silo



=================
build
=================

	.\Container-Build orleans-silo



=================
run
=================

	.\Container-IP orleans-postgresql-5435
	.\Container-Run 192.168.248.78 orleans-silo 30005



=================
test
=================

	- get ip address of running container

		.\Container-IP orleans-silo

	- manually spin up a container to test it

		docker create -t --name testing -i jose/orleans/silo:latest
		docker start -i testing
		cd c:\orleans\silo\Run-Orleans.bat 192.168.248.78 30005 false
		docker rm testing

Run-Orleans.bat [DB-IP] [CONTAINER-PORT] false

=================
END
=================

	.\Container-Kill orleans-silo



=================
- remote debugging
=================
https://stackoverflow.com/questions/36420337/running-visual-studio-remote-debugger-in-windows-container-docker-managed
