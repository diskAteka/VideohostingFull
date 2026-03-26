@echo off
set MINIO_ROOT_USER=minioadmin
set MINIO_ROOT_PASSWORD=MinioAdmin123!
set MINIO_ACCESS_KEY=minioadmin
set MINIO_SECRET_KEY=MinioAdmin123!

mkdir data

minio.exe server data --console-address :9001 --address :9000

pause