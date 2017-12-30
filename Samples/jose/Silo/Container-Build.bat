:: ===============================================
:: deleting/rebuilding the image
:: ===============================================
set container_name=%1  

rmdir /s /q output
dotnet build
dotnet publish
docker rm %container_name% -f
docker rmi jose/orleans/silo:latest
docker build -t jose/orleans/silo:latest  -f Dockerfile .
