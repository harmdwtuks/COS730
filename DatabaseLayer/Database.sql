USE [master]
GO
/****** Object:  Database [CoachIt] ******/
CREATE DATABASE [CoachIt]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CoachIt', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\CoachIt.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'CoachIt_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\CoachIt_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [CoachIt] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CoachIt].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CoachIt] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CoachIt] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CoachIt] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CoachIt] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CoachIt] SET ARITHABORT OFF 
GO
ALTER DATABASE [CoachIt] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [CoachIt] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CoachIt] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CoachIt] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CoachIt] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CoachIt] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CoachIt] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CoachIt] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CoachIt] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CoachIt] SET  DISABLE_BROKER 
GO
ALTER DATABASE [CoachIt] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CoachIt] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CoachIt] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CoachIt] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CoachIt] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CoachIt] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CoachIt] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CoachIt] SET RECOVERY FULL 
GO
ALTER DATABASE [CoachIt] SET  MULTI_USER 
GO
ALTER DATABASE [CoachIt] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CoachIt] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CoachIt] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CoachIt] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [CoachIt] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'CoachIt', N'ON'
GO
ALTER DATABASE [CoachIt] SET QUERY_STORE = OFF
GO
USE [CoachIt]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
USE [CoachIt]
GO
/****** Object:  User [skynetUser]    Script Date: 2021/06/21 22:26:56 ******/
CREATE USER [skynetUser] FOR LOGIN [skynetUser] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [skynetUser]
GO
ALTER ROLE [db_datareader] ADD MEMBER [skynetUser]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [skynetUser]
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Messages]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Messages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[SenderId] [int] NOT NULL,
	[TeamId] [int] NOT NULL,
 CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MetricClass]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MetricClass](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Class] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_MetricClass] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MetricRecords]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MetricRecords](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Measurement] [float] NOT NULL,
	[MetricTypeId] [int] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_MetricRecords] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MetricType]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MetricType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](500) NOT NULL,
	[MetricUnitId] [int] NOT NULL,
	[MetricClassId] [int] NOT NULL,
 CONSTRAINT [PK_MetricType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MetricUnit]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MetricUnit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Unit] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MetricUnit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Teams]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Teams](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TeamName] [nvarchar](128) NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[CreatorId] [int] NOT NULL,
 CONSTRAINT [PK_Teams] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TeamsUsers]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeamsUsers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TeamId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_TeamsUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_Membership]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_Membership](
	[UserId] [int] NOT NULL,
	[CreateDate] [datetime] NULL,
	[ConfirmationToken] [nvarchar](128) NULL,
	[IsConfirmed] [bit] NULL,
	[LastPasswordFailureDate] [datetime] NULL,
	[PasswordFailuresSinceLastSuccess] [int] NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[PasswordChangedDate] [datetime] NULL,
	[PasswordSalt] [nvarchar](128) NOT NULL,
	[PasswordVerificationToken] [nvarchar](128) NULL,
	[PasswordVerificationTokenExpirationDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_OAuthMembership]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_OAuthMembership](
	[Provider] [nvarchar](30) NOT NULL,
	[ProviderUserId] [nvarchar](100) NOT NULL,
	[UserId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Provider] ASC,
	[ProviderUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_Roles]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_Roles](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](256) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_Users]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](56) NOT NULL,
	[FirstName] [nvarchar](256) NULL,
	[Surname] [nvarchar](256) NULL,
	[EmailAddress] [nvarchar](256) NULL,
	[ContactNumber] [nvarchar](20) NULL,
	[TelegramUserId] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_UsersInRoles]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_UsersInRoles](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WorkoutExerciseCategories]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkoutExerciseCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Category] [nvarchar](1024) NOT NULL,
 CONSTRAINT [PK_WorkoutExerciseCategories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WorkoutExerciseLinkCompleted]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkoutExerciseLinkCompleted](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WorkoutExerciseLinkId] [int] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[Sets] [int] NOT NULL,
	[Repititions] [int] NOT NULL,
	[Duration] [time](7) NOT NULL,
	[Weight] [float] NOT NULL,
 CONSTRAINT [PK_WorkoutExerciseLinkCompleted] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WorkoutExercises]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkoutExercises](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[Exercise] [nvarchar](1024) NOT NULL,
	[Instructions] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_WorkoutExercises] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WorkoutExercisesLInk]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkoutExercisesLInk](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WorkoutUsersId] [int] NOT NULL,
	[ExerciseId] [int] NOT NULL,
	[Sets] [int] NOT NULL,
	[Repititions] [int] NOT NULL,
	[Duration] [time](7) NOT NULL,
	[Weight] [float] NOT NULL,
 CONSTRAINT [PK_WorkoutExercisesUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WorkoutUsers]    Script Date: 2021/06/21 22:26:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkoutUsers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Workout] [nvarchar](512) NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[UserId] [int] NOT NULL,
	[Duration] [time](7) NOT NULL,
 CONSTRAINT [PK_Workouts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 2021/06/21 22:26:56 ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserId]    Script Date: 2021/06/21 22:26:56 ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserId]    Script Date: 2021/06/21 22:26:56 ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_RoleId]    Script Date: 2021/06/21 22:26:56 ******/
CREATE NONCLUSTERED INDEX [IX_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserId]    Script Date: 2021/06/21 22:26:56 ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserRoles]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 2021/06/21 22:26:56 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[MetricRecords] ADD  CONSTRAINT [DF_MetricRecords_DateTimeRecorded]  DEFAULT (getdate()) FOR [Timestamp]
GO
ALTER TABLE [dbo].[webpages_Membership] ADD  DEFAULT ((0)) FOR [IsConfirmed]
GO
ALTER TABLE [dbo].[webpages_Membership] ADD  DEFAULT ((0)) FOR [PasswordFailuresSinceLastSuccess]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_Messages_Teams] FOREIGN KEY([TeamId])
REFERENCES [dbo].[Teams] ([Id])
GO
ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_Messages_Teams]
GO
ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_Messages_webpages_Users] FOREIGN KEY([SenderId])
REFERENCES [dbo].[webpages_Users] ([UserId])
GO
ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_Messages_webpages_Users]
GO
ALTER TABLE [dbo].[MetricRecords]  WITH CHECK ADD  CONSTRAINT [FK_MetricRecords_MetricType] FOREIGN KEY([MetricTypeId])
REFERENCES [dbo].[MetricType] ([Id])
GO
ALTER TABLE [dbo].[MetricRecords] CHECK CONSTRAINT [FK_MetricRecords_MetricType]
GO
ALTER TABLE [dbo].[MetricRecords]  WITH CHECK ADD  CONSTRAINT [FK_MetricRecords_webpages_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[webpages_Users] ([UserId])
GO
ALTER TABLE [dbo].[MetricRecords] CHECK CONSTRAINT [FK_MetricRecords_webpages_Users]
GO
ALTER TABLE [dbo].[MetricType]  WITH CHECK ADD  CONSTRAINT [FK_MetricType_MetricClass] FOREIGN KEY([MetricClassId])
REFERENCES [dbo].[MetricClass] ([Id])
GO
ALTER TABLE [dbo].[MetricType] CHECK CONSTRAINT [FK_MetricType_MetricClass]
GO
ALTER TABLE [dbo].[MetricType]  WITH CHECK ADD  CONSTRAINT [FK_MetricType_MetricUnit] FOREIGN KEY([MetricUnitId])
REFERENCES [dbo].[MetricUnit] ([Id])
GO
ALTER TABLE [dbo].[MetricType] CHECK CONSTRAINT [FK_MetricType_MetricUnit]
GO
ALTER TABLE [dbo].[Teams]  WITH CHECK ADD  CONSTRAINT [FK_Teams_webpages_Users] FOREIGN KEY([CreatorId])
REFERENCES [dbo].[webpages_Users] ([UserId])
GO
ALTER TABLE [dbo].[Teams] CHECK CONSTRAINT [FK_Teams_webpages_Users]
GO
ALTER TABLE [dbo].[TeamsUsers]  WITH CHECK ADD  CONSTRAINT [FK_TeamsUsers_Teams] FOREIGN KEY([TeamId])
REFERENCES [dbo].[Teams] ([Id])
GO
ALTER TABLE [dbo].[TeamsUsers] CHECK CONSTRAINT [FK_TeamsUsers_Teams]
GO
ALTER TABLE [dbo].[TeamsUsers]  WITH CHECK ADD  CONSTRAINT [FK_TeamsUsers_webpages_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[webpages_Users] ([UserId])
GO
ALTER TABLE [dbo].[TeamsUsers] CHECK CONSTRAINT [FK_TeamsUsers_webpages_Users]
GO
ALTER TABLE [dbo].[webpages_UsersInRoles]  WITH CHECK ADD  CONSTRAINT [fk_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[webpages_Roles] ([RoleId])
GO
ALTER TABLE [dbo].[webpages_UsersInRoles] CHECK CONSTRAINT [fk_RoleId]
GO
ALTER TABLE [dbo].[webpages_UsersInRoles]  WITH CHECK ADD  CONSTRAINT [fk_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[webpages_Users] ([UserId])
GO
ALTER TABLE [dbo].[webpages_UsersInRoles] CHECK CONSTRAINT [fk_UserId]
GO
ALTER TABLE [dbo].[WorkoutExerciseLinkCompleted]  WITH CHECK ADD  CONSTRAINT [FK_WorkoutExerciseLinkCompleted_WorkoutExercisesLInk] FOREIGN KEY([WorkoutExerciseLinkId])
REFERENCES [dbo].[WorkoutExercisesLInk] ([Id])
GO
ALTER TABLE [dbo].[WorkoutExerciseLinkCompleted] CHECK CONSTRAINT [FK_WorkoutExerciseLinkCompleted_WorkoutExercisesLInk]
GO
ALTER TABLE [dbo].[WorkoutExercises]  WITH CHECK ADD  CONSTRAINT [FK_WorkoutExercises_WorkoutExerciseCategories] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[WorkoutExerciseCategories] ([Id])
GO
ALTER TABLE [dbo].[WorkoutExercises] CHECK CONSTRAINT [FK_WorkoutExercises_WorkoutExerciseCategories]
GO
ALTER TABLE [dbo].[WorkoutExercisesLInk]  WITH CHECK ADD  CONSTRAINT [FK_WorkoutExercisesUsers_WorkoutExercises] FOREIGN KEY([ExerciseId])
REFERENCES [dbo].[WorkoutExercises] ([Id])
GO
ALTER TABLE [dbo].[WorkoutExercisesLInk] CHECK CONSTRAINT [FK_WorkoutExercisesUsers_WorkoutExercises]
GO
ALTER TABLE [dbo].[WorkoutExercisesLInk]  WITH CHECK ADD  CONSTRAINT [FK_WorkoutExercisesUsers_WorkoutUsers] FOREIGN KEY([WorkoutUsersId])
REFERENCES [dbo].[WorkoutUsers] ([Id])
GO
ALTER TABLE [dbo].[WorkoutExercisesLInk] CHECK CONSTRAINT [FK_WorkoutExercisesUsers_WorkoutUsers]
GO
ALTER TABLE [dbo].[WorkoutUsers]  WITH CHECK ADD  CONSTRAINT [FK_WorkoutUsers_webpages_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[webpages_Users] ([UserId])
GO
ALTER TABLE [dbo].[WorkoutUsers] CHECK CONSTRAINT [FK_WorkoutUsers_webpages_Users]
GO
USE [master]
GO
ALTER DATABASE [CoachIt] SET  READ_WRITE 
GO

--Default Roles
insert into webpages_Roles(RoleName) values ('System Administrator')
insert into webpages_Roles(RoleName) values ('Coach')
insert into webpages_Roles(RoleName) values ('Moderator')
insert into webpages_Roles(RoleName) values ('User')
GO

--Default User
insert into webpages_Users(username, FirstName,Surname,EmailAddress,ContactNumber) values ('USERNAME','NAME','LASTNAME','email@example.com','0123456789')
 GO

--Default Membership
insert into webpages_Membership(UserId,CreateDate,IsConfirmed,[Password],PasswordSalt) values (1,getdate(),1,'ALYDXQkAYOtyPhmXXNhOJfx8U/hRHC0URFGhwh52ioQiJAb6hkuBCC+UTbSy6t+yeg==','')
 GO

--Default Admin User Roles
insert into webpages_UsersInRoles(UserId,RoleId) values (1,1)
GO
