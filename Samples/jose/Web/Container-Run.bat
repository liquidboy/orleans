@echo off
set silo_container_ip=%1
set silo_container_port=%2
set client_container_name=%3


docker run -p 80:80 -td --name %client_container_name% jose/orleans/client:latest c:\orleans\client\Run-Orleans.bat %silo_container_ip% %silo_container_port%


rem timeout /t 5

call Container-IP.bat %client_container_name%