@echo off
set container_name=%1  
docker rm %container_name% -f