:: ===============================================
:: launching the silo as a dotnet initiated proces
:: ===============================================


set postgresql_server=%1  
set proxy_gateway_port=%2
set dev_mode=%3
set is_primary=%4
set silo_name=%5

cd c:\orleans\silo
call dotnet Silo.dll %postgresql_server% %proxy_gateway_port% %dev_mode% %is_primary% %silo_name%

rem run neverending service so container doesn't exit
rem call PowerShell c:\orleans\silo\wait-service.ps1 -ServiceName "CExecSvc"