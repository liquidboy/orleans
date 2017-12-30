FROM microsoft/dotnet:runtime

ENV ORLEANS_PKG=output/netcoreapp2.0/publish/ \
    ORLEANS_HOME=C:\\orleans\\silo

ADD $ORLEANS_PKG $ORLEANS_HOME

# WORKDIR /app
# COPY ./output/publish/silo  /

# ENTRYPOINT ["dotnet", "Silo.dll"]
