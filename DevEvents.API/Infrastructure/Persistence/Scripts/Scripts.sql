IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Attendees] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Attendees] PRIMARY KEY ([Id])
);

CREATE TABLE [Conferences] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Conferences] PRIMARY KEY ([Id])
);

CREATE TABLE [Registrations] (
    [Id] int NOT NULL IDENTITY,
    [IdConference] int NOT NULL,
    [IdAttendee] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Registrations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Registrations_Attendees_IdAttendee] FOREIGN KEY ([IdAttendee]) REFERENCES [Attendees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Registrations_Conferences_IdConference] FOREIGN KEY ([IdConference]) REFERENCES [Conferences] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Speakers] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Bio] nvarchar(max) NOT NULL,
    [Website] nvarchar(max) NOT NULL,
    [IdConference] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Speakers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Speakers_Conferences_IdConference] FOREIGN KEY ([IdConference]) REFERENCES [Conferences] ([Id]) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX [IX_Registrations_IdAttendee] ON [Registrations] ([IdAttendee]);

CREATE INDEX [IX_Registrations_IdConference] ON [Registrations] ([IdConference]);

CREATE INDEX [IX_Speakers_IdConference] ON [Speakers] ([IdConference]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250303104857_InitialCreate', N'9.0.1');

ALTER TABLE [Conferences] ADD [Location] nvarchar(max) NOT NULL DEFAULT N'';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250303105044_AddLocationToConference', N'9.0.1');

COMMIT;
GO