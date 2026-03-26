@echo off
echo Checking services...

echo.
echo NginX:
tasklist | findstr nginx

echo.
echo MinIO:
tasklist | findstr minio

echo.
echo Backend (.NET):
tasklist | findstr dotnet

echo.
echo Ports:
echo Port 80 (NginX HTTP):
netstat -ano | findstr :80 | findstr LISTENING
echo Port 443 (NginX HTTPS):
netstat -ano | findstr :443 | findstr LISTENING
echo Port 8080 (Backend):
netstat -ano | findstr :8080 | findstr LISTENING
echo Port 9000 (MinIO API):
netstat -ano | findstr :9000 | findstr LISTENING
echo Port 9001 (MinIO Console):
netstat -ano | findstr :9001 | findstr LISTENING

pause