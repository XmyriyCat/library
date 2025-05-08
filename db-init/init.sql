IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'LibraryDb')
BEGIN
    CREATE DATABASE [LibraryDb];
END
