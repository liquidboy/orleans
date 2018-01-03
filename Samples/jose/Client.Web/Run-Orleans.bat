:: ===============================================
:: launching the client as a dotnet initiated proces
:: ===============================================

set silo_gateways=%1

cd c:\orleans\client
call dotnet Client.Web.dll silo_gateways=%silo_gateways%
