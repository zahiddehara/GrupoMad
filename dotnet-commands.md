# dotnet commands

## Crear nuevo proyecto
dotnet new mvc -o GrupoMad

## Crear nuevo proyecto webapp (no usar este)
dotnet new webapp -o GrupoMad

# Crear gitignore
dotnet new gitignore

# Instalacion EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore --version 9.0.10

# Instalacion EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.10

# Instalacion EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 9.0.10

# Instalacion de la herramienta cli dotnet-ef
dotnet tool install --global dotnet-ef

# Crear la migración inicial
dotnet ef migrations add InitialCreate

# Aplicar la migración (crea la base de datos)
dotnet ef database update

## Para instalar el codegenerator es necesario instalar net runtime 8, bajarlo del siguiente link
## .NET Runtime 8.0.21
https://dotnet.microsoft.com/en-us/download/dotnet/8.0
## Instalar dotnet aspnet-codegenerator
dotnet tool install -g dotnet-aspnet-codegenerator
## Despues agregar el paquete al proyecto
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design

# Generar controller + vistas CRUD
dotnet aspnet-codegenerator controller -name StoreController -m Store -dc ApplicationDbContext -outDir Controllers -dbProvider sqlite

# convertir a lf
dos2unix archivo.cs

# 🚀 Para actualizar la aplicación después de cambios:

## En el VPS
cd /var/www/GrupoMad
git pull origin main
dotnet publish -c Release -o /var/www/GrupoMad/publish
sudo systemctl restart grupomad-erp.service

## Crear nuevo controller (Todavia no se comprueba)
dotnet new mvccontroller -n ProductController -o Controllers
