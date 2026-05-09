# Используем легковесный образ Linux
FROM alpine:latest

# Устанавливаем необходимые библиотеки для работы Garage
RUN apk add --no-cache libgcc

# Создаем пользователя и папки внутри контейнера
RUN addgroup -S garage && adduser -S garage -G garage
RUN mkdir -p /garage-data/meta /garage-data/data && chown -R garage:garage /garage-data

# Копируем ваш бинарник garage из текущей папки в контейнер
COPY ./garage /usr/local/bin/garage
# Копируем файл конфигурации из папки config в контейнер
COPY ./config/garage.toml /etc/garage.toml

# Даем права на выполнение бинарника
RUN chmod +x /usr/local/bin/garage

# Переключаемся на непривилегированного пользователя (хорошая практика для безопасности)
USER garage

# Указываем, какие порты будет "слушать" контейнер
EXPOSE 3900
EXPOSE 3903

# Команда, которая выполнится при старте контейнера
ENTRYPOINT ["/usr/local/bin/garage", "server"]