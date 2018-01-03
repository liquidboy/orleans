rem @echo off
set silo_gateways=%1
set client_container_name=%2
set client_container_port=%3

docker run -p %client_container_port%:80 -td --name %client_container_name% jose/orleans/client:latest c:\orleans\client\Run-Orleans.bat %silo_gateways%


rem timeout /t 5

call Container-IP.bat %client_container_name%