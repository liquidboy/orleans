@echo off
set db_container_ip=%1
set silo_container_name=%2
set silo_container_port=%3

docker run -p %silo_container_port%:%silo_container_port% -p 11111:11111 -td --name %silo_container_name% jose/orleans/silo:latest c:\orleans\silo\Run-Orleans.bat %db_container_ip% %silo_container_port% false

rem timeout /t 5

call Container-IP.bat %silo_container_name%