# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем проект и восстанавливаем зависимости
COPY Web_Library.API/Web_Library.API.csproj Web_Library.API/
COPY DTOs/Web_Library.DTOs.csproj DTOs/
COPY Web_Library.Services/Web_Library.Services.csproj Web_Library.Services/
COPY Web_Library.Data/Web_Library.Data.csproj Web_Library.Data/
COPY Web_Library.Models/Web_Library.Models.csproj Web_Library.Models/
COPY Web_Library.Repositories/Web_Library.Repositories.csproj Web_Library.Repositories/
RUN dotnet restore "./Web_Library.API/Web_Library.API.csproj"

# Копируем все файлы
COPY . .

# Собираем проект
WORKDIR "/src/Web_Library.API"
RUN dotnet build "./Web_Library.API.csproj" -c Release -o /app/build

# Этап публикации
FROM build AS publish
RUN dotnet publish "./Web_Library.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Финальный этап
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Добавление пути к глобальным инструментам в переменные окружения
ENV PATH="$PATH:/root/.dotnet/tools"

# Применение миграций при старте контейнера
ENTRYPOINT ["sh", "-c", "dotnet Web_Library.API.dll"]