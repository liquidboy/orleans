:: ===============================================
:: launching the client as a dotnet initiated proces
:: ===============================================

set proxy_ip=%1
set proxy_gateway_port=%2

cd c:\orleans\client
call dotnet Client.Web.dll proxyip=%proxy_ip% proxyport=%proxy_gateway_port%
