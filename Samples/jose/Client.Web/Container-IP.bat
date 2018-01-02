@echo off
set container_name=%1  
docker inspect -f "{{ .NetworkSettings.Networks.nat.IPAddress }}" %container_name%