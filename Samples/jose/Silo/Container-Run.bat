@echo off
set db_container_ip=%1
set silo_container_name=%2
set silo_container_port=%3
set is_primary=%4

docker run -p %silo_container_port%:%silo_container_port% -td --name %silo_container_name% jose/orleans/silo:latest c:\orleans\silo\Run-Orleans.bat %db_container_ip% %silo_container_port% false %is_primary% %silo_container_name%

rem docker run -p %silo_container_port%:%silo_container_port% -p 11111:11111 -td --name %silo_container_name% jose/orleans/silo:latest c:\orleans\silo\Run-Orleans.bat %db_container_ip% %silo_container_port% false %is_primary% %silo_container_name%

rem docker create -p %silo_container_port%:%silo_container_port% -p 11111:11111 -t --name %silo_container_name% jose/orleans/silo:latest
rem docker start %silo_container_name%
rem docker exec -d %silo_container_name% c:\orleans\silo\Run-Orleans.bat %db_container_ip% %silo_container_port% false %is_primary%

rem timeout /t 5

call Container-IP.bat %silo_container_name%