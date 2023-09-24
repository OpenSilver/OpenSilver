CREATE TABLE [aspnet_Applications]
(
	[ApplicationId] TEXT PRIMARY KEY UNIQUE NOT NULL,
	[ApplicationName] TEXT UNIQUE NOT NULL,
	[Description] TEXT NULL
);

CREATE TABLE [aspnet_Roles]
(
	[RoleId] TEXT PRIMARY KEY UNIQUE NOT NULL,
	[RoleName] TEXT NOT NULL,
	[LoweredRoleName] TEXT NOT NULL,
	[ApplicationId] TEXT NOT NULL
);

CREATE TABLE [aspnet_UsersInRoles]
(
	[UserId] TEXT NOT NULL,
	[RoleId] TEXT NOT NULL
);

CREATE TABLE [aspnet_Profile] (
 [UserId] TEXT UNIQUE NOT NULL,
 [LastUpdatedDate] TIMESTAMP NOT NULL,
 [PropertyNames] TEXT NOT NULL,
 [PropertyValuesString] TEXT NOT NULL,
 [PropertyValuesBinary] BLOB NOT NULL
);

CREATE TABLE [aspnet_Users] (
[UserId] TEXT PRIMARY KEY UNIQUE NOT NULL,
[Username] TEXT NOT NULL,
[LoweredUsername] TEXT NOT NULL,
[ApplicationId] TEXT NOT NULL,
[Email] TEXT NULL,
[LoweredEmail] TEXT NULL,
[Comment] TEXT NULL,
[Password] TEXT NOT NULL,
[PasswordFormat] TEXT NOT NULL,
[PasswordSalt] TEXT NOT NULL,
[PasswordQuestion] TEXT NULL,
[PasswordAnswer] TEXT NULL,
[IsApproved] BOOL NOT NULL,
[IsAnonymous] BOOL  NOT NULL,
[LastActivityDate] TIMESTAMP  NOT NULL,
[LastLoginDate] TIMESTAMP NOT NULL,
[LastPasswordChangedDate] TIMESTAMP NOT NULL,
[CreateDate] TIMESTAMP  NOT NULL,
[IsLockedOut] BOOL NOT NULL,
[LastLockoutDate] TIMESTAMP NOT NULL,
[FailedPasswordAttemptCount] INTEGER NOT NULL,
[FailedPasswordAttemptWindowStart] TIMESTAMP NOT NULL,
[FailedPasswordAnswerAttemptCount] INTEGER NOT NULL,
[FailedPasswordAnswerAttemptWindowStart] TIMESTAMP NOT NULL
);

CREATE UNIQUE INDEX idxUsers ON [aspnet_Users] ( 'LoweredUsername' , 'ApplicationId' );

CREATE INDEX idxUsersAppId ON [aspnet_Users] ( 'ApplicationId' );

CREATE UNIQUE INDEX idxRoles ON [aspnet_Roles] ( 'LoweredRoleName' , 'ApplicationId' );

CREATE UNIQUE INDEX idxUsersInRoles ON [aspnet_UsersInRoles] ( 'UserId', 'RoleId');
