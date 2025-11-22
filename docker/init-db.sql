-- T015: Database initialization script
CREATE DATABASE TeamGoalTracker;
GO

USE TeamGoalTracker;
GO

-- TeamMembers Table
CREATE TABLE TeamMembers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT CK_TeamMembers_Name_NotEmpty CHECK (LEN(TRIM(Name)) > 0)
);

CREATE INDEX IX_TeamMembers_Name ON TeamMembers(Name);

-- Goals Table
CREATE TABLE Goals (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TeamMemberId INT NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    IsCompleted BIT NOT NULL DEFAULT 0,
    Date DATE NOT NULL DEFAULT CAST(GETUTCDATE() AS DATE),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Goals_TeamMember FOREIGN KEY (TeamMemberId) REFERENCES TeamMembers(Id) ON DELETE CASCADE,
    CONSTRAINT CK_Goals_Description_NotEmpty CHECK (LEN(TRIM(Description)) > 0)
);

CREATE INDEX IX_Goals_TeamMemberId_Date ON Goals(TeamMemberId, Date);
CREATE INDEX IX_Goals_Date_IsCompleted ON Goals(Date, IsCompleted);

-- Moods Table
CREATE TABLE Moods (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TeamMemberId INT NOT NULL,
    MoodType INT NOT NULL,
    Date DATE NOT NULL DEFAULT CAST(GETUTCDATE() AS DATE),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Moods_TeamMember FOREIGN KEY (TeamMemberId) REFERENCES TeamMembers(Id) ON DELETE CASCADE,
    CONSTRAINT CK_Moods_MoodType_Range CHECK (MoodType BETWEEN 1 AND 5),
    CONSTRAINT UQ_Moods_TeamMember_Date UNIQUE (TeamMemberId, Date)
);

CREATE INDEX IX_Moods_TeamMemberId_Date ON Moods(TeamMemberId, Date);
CREATE INDEX IX_Moods_Date ON Moods(Date);

-- Seed Data
INSERT INTO TeamMembers (Name) VALUES
('Alice Johnson'),
('Bob Smith'),
('Charlie Davis'),
('Diana Prince'),
('Eve Wilson');

DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE);

INSERT INTO Goals (TeamMemberId, Description, IsCompleted, Date) VALUES
(1, 'Complete project proposal', 0, @Today),
(1, 'Review pull requests', 1, @Today),
(1, 'Update documentation', 0, @Today),
(2, 'Fix bug #123', 1, @Today),
(2, 'Write unit tests', 0, @Today),
(3, 'Design new feature mockups', 0, @Today),
(3, 'Client meeting preparation', 1, @Today),
(4, 'Code review for team', 0, @Today),
(5, 'Sprint planning', 1, @Today);

INSERT INTO Moods (TeamMemberId, MoodType, Date) VALUES
(1, 2, @Today),  -- Alice: Happy
(2, 3, @Today),  -- Bob: Neutral
(3, 1, @Today),  -- Charlie: Very Happy
(4, 4, @Today),  -- Diana: Sad
(5, 5, @Today);  -- Eve: Stressed

GO
