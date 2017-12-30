
note: run these from the "Silo" folder in powershell

=================
START
=================

	cd E:\Source\gh\dotnet\orleans\Samples\jose\Web



=================
build
=================

	.\Container-Build orleans-client



=================
run
=================

	.\Container-IP orleans-silo
	.\Container-Run 192.168.243.243 30005 orleans-client



=================
test
=================

	- get ip address of running container

		.\Container-IP orleans-client

	- manually spin up a container to test in

		docker create -p 80:80 -t --name testing -i jose/orleans/client:latest
		docker start -i testing
		cd c:\orleans\client\Run-Orleans.bat [SILO-CONTAINER-IP] [SILO-CONTAINER-PORT]
		docker rm testing

	- navigate to 
	
		http://[CLIENT-IP]/api/counter



=================
END
=================

	.\Container-Kill orleans-client



=================
- remote debugging
=================

	https://stackoverflow.com/questions/36420337/running-visual-studio-remote-debugger-in-windows-container-docker-managed

