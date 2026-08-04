# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY MentorOS.csproj .
RUN dotnet restore MentorOS.csproj

COPY . .
RUN dotnet publish MentorOS.csproj -c Release -o /app/publish

# Runtime stage -- smaller image, no SDK/build tools included
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Render sets $PORT at runtime; Program.cs reads it and binds to it.
ENTRYPOINT ["dotnet", "MentorOS.dll"]
