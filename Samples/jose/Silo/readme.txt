
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
	.\Container-Run 192.168.252.210 orleans-silo-1 30006 true
	.\Container-Run 192.168.252.210 orleans-silo-2 30007 false



=================
test
=================

	- get ip address of running container

		.\Container-IP orleans-silo-1
		.\Container-IP orleans-silo-2

	- manually spin up a container to test it

		docker create -p 30006:30005 -p 11112:11111 -t --name orleans-silo-2 -i jose/orleans/silo:latest
		docker start -i orleans-silo-2
		c:\orleans\silo\Run-Orleans.bat 192.168.248.78 30005 false false orleans-silo-2
		docker rm orleans-silo-2

Run-Orleans.bat [DB-IP] [CONTAINER-PORT] false

=================
END
=================

	.\Container-Kill orleans-silo



=================
- remote debugging
=================
https://stackoverflow.com/questions/36420337/running-visual-studio-remote-debugger-in-windows-container-docker-managed
