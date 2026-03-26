@echo off

REM Создаем папку для сертификатов
mkdir certs

REM Генерируем самоподписанный сертификат
openssl req -x509 -nodes -days 365 -newkey rsa:2048 ^
    -keyout certs/private.key ^
    -out certs/public.crt ^
    -subj "/C=RU/ST=Moscow/L=Moscow/O=VideoHosting/CN=localhost"

echo Certificates created successfully
pause