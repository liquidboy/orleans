
note: run these from the "Silo" folder in powershell

=================
START
=================

	cd E:\Source\gh\dotnet\orleans\Samples\jose\Client.Web



=================
build
=================

	.\Container-Build orleans-client



=================
run
=================

	.\Container-IP orleans-silo-1
	.\Container-Run 192.168.255.32:30006-192.168.254.16:30007 orleans-client-1 80
	.\Container-Run 192.168.253.71:30006 orleans-client-2 81
	
	.\Container-IP orleans-silo-2
	.\Container-Run 192.168.242.11:30007 orleans-client-3 82



=================
test
=================

	- get ip address of running container

		.\Container-IP orleans-client-1
		.\Container-IP orleans-client-2

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

