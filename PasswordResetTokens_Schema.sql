-- Password Reset Tokens Table
-- This table stores secure tokens for password reset functionality

CREATE TABLE PasswordResetTokens (
    TokenID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL FOREIGN KEY REFERENCES Users(UserID),
    Token NVARCHAR(255) UNIQUE NOT NULL,
    ExpirationDate DATETIME NOT NULL,
    IsUsed BIT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    UsedDate DATETIME NULL
);
GO

-- Index for faster token lookup
CREATE INDEX IX_PasswordResetTokens_Token ON PasswordResetTokens(Token);
GO

-- Index for user lookup
CREATE INDEX IX_PasswordResetTokens_UserID ON PasswordResetTokens(UserID);
GO

-- Verification query
SELECT * FROM PasswordResetTokens;
GO
