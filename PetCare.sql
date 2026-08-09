USE [master]
GO
/****** 物件:  Database [PetCare]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE DATABASE [PetCare]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'PetCare', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA\PetCare.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'PetCare_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA\PetCare_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [PetCare] SET COMPATIBILITY_LEVEL = 170
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PetCare].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [PetCare] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PetCare] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PetCare] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PetCare] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PetCare] SET ARITHABORT OFF 
GO
ALTER DATABASE [PetCare] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [PetCare] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PetCare] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PetCare] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PetCare] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PetCare] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PetCare] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PetCare] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PetCare] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [PetCare] SET  ENABLE_BROKER 
GO
ALTER DATABASE [PetCare] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PetCare] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PetCare] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PetCare] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PetCare] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PetCare] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [PetCare] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [PetCare] SET RECOVERY FULL 
GO
ALTER DATABASE [PetCare] SET  MULTI_USER 
GO
ALTER DATABASE [PetCare] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [PetCare] SET DB_CHAINING OFF 
GO
ALTER DATABASE [PetCare] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [PetCare] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [PetCare] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [PetCare] SET OPTIMIZED_LOCKING = OFF 
GO
ALTER DATABASE [PetCare] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [PetCare] SET QUERY_STORE = ON
GO
ALTER DATABASE [PetCare] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [PetCare]
GO
/****** 物件:  Table [dbo].[__EFMigrationsHistory]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[Appointments]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Appointments](
	[AppointmentID] [nchar](20) NOT NULL,
	[PetID] [int] NOT NULL,
	[InstitutionID] [nchar](6) NOT NULL,
	[ServiceType] [nvarchar](50) NOT NULL,
	[ApptDateTime] [datetime2](7) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Appointments] PRIMARY KEY CLUSTERED 
(
	[AppointmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[AuthorizationRecords]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuthorizationRecords](
	[DataAuthorizationNumber] [nchar](20) NOT NULL,
	[PetID] [int] NOT NULL,
	[InstitutionID] [nchar](6) NOT NULL,
	[GrantedDate] [datetime2](7) NOT NULL,
	[RevokedDate] [datetime2](7) NULL,
	[Status] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_AuthorizationRecords] PRIMARY KEY CLUSTERED 
(
	[DataAuthorizationNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[CareGroomingLogs]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CareGroomingLogs](
	[LogID] [int] IDENTITY(1,1) NOT NULL,
	[PetID] [int] NOT NULL,
	[StaffID] [nchar](13) NOT NULL,
	[LogDate] [datetime2](7) NOT NULL,
	[ActivityDetails] [nvarchar](max) NOT NULL,
	[DietNotes] [nvarchar](max) NULL,
 CONSTRAINT [PK_CareGroomingLogs] PRIMARY KEY CLUSTERED 
(
	[LogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[Employments]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employments](
	[UserID] [nchar](13) NOT NULL,
	[InstitutionID] [nchar](6) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[LicenseNumber] [nvarchar](50) NOT NULL,
	[StartDate] [datetime2](7) NULL,
	[EndDate] [datetime2](7) NULL,
	[OccupationID] [nchar](3) NOT NULL,
 CONSTRAINT [PK_Employments] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[Institutions]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Institutions](
	[InstitutionID] [nchar](6) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[LicenseNumber] [nvarchar](50) NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[Address] [nvarchar](255) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Institutions] PRIMARY KEY CLUSTERED 
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[LogPhotos]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogPhotos](
	[PhotoID] [int] IDENTITY(1,1) NOT NULL,
	[LogID] [int] NOT NULL,
	[ImageURL] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_LogPhotos] PRIMARY KEY CLUSTERED 
(
	[PhotoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[MedicalRecords]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MedicalRecords](
	[RecordID] [int] IDENTITY(1,1) NOT NULL,
	[PetID] [int] NOT NULL,
	[VetID] [nchar](13) NOT NULL,
	[VisitDate] [datetime2](7) NOT NULL,
	[Symptoms] [nvarchar](max) NULL,
	[TestResultsFile] [nvarchar](255) NULL,
	[Diagnosis] [nvarchar](max) NULL,
 CONSTRAINT [PK_MedicalRecords] PRIMARY KEY CLUSTERED 
(
	[RecordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[MedicalVocabularyDictionaries]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MedicalVocabularyDictionaries](
	[TermID] [int] IDENTITY(1,1) NOT NULL,
	[TermCode] [nvarchar](50) NOT NULL,
	[Category] [nvarchar](50) NOT NULL,
	[TermName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_MedicalVocabularyDictionaries] PRIMARY KEY CLUSTERED 
(
	[TermID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[Occupations]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Occupations](
	[OccupationID] [nchar](3) NOT NULL,
	[OccupationName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Occupations] PRIMARY KEY CLUSTERED 
(
	[OccupationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[Pets]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pets](
	[PetID] [int] IDENTITY(1,1) NOT NULL,
	[OwnerID] [nchar](13) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Species] [nvarchar](50) NOT NULL,
	[Breed] [nvarchar](50) NULL,
	[Gender] [nvarchar](10) NULL,
	[DateOfBirth] [datetime2](7) NULL,
	[MicrochipNo] [nvarchar](50) NULL,
	[SpecialNotes] [nvarchar](max) NULL,
	[ProfilePhotoUrl] [nvarchar](255) NULL,
 CONSTRAINT [PK_Pets] PRIMARY KEY CLUSTERED 
(
	[PetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[PetWeight]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PetWeight](
	[PetWeightID] [int] IDENTITY(1,1) NOT NULL,
	[PetID] [int] NOT NULL,
	[Weight] [decimal](5, 2) NOT NULL,
	[RecordDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PetWeight] PRIMARY KEY CLUSTERED 
(
	[PetWeightID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[Prescriptions]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Prescriptions](
	[PrescriptionID] [int] IDENTITY(1,1) NOT NULL,
	[RecordID] [int] NOT NULL,
	[MedicationName] [nvarchar](100) NOT NULL,
	[Dosage] [nvarchar](50) NOT NULL,
	[Frequency] [nvarchar](50) NOT NULL,
	[Instructions] [nvarchar](max) NULL,
 CONSTRAINT [PK_Prescriptions] PRIMARY KEY CLUSTERED 
(
	[PrescriptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[RecordDiagnoses]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecordDiagnoses](
	[DetailID] [int] IDENTITY(1,1) NOT NULL,
	[RecordID] [int] NOT NULL,
	[TermID] [int] NOT NULL,
	[Severity] [nvarchar](20) NULL,
	[Remark] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_RecordDiagnoses] PRIMARY KEY CLUSTERED 
(
	[DetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[SysMenus]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SysMenus](
	[MenuId] [int] IDENTITY(1,1) NOT NULL,
	[MenuName] [nvarchar](50) NOT NULL,
	[ControllerName] [nvarchar](50) NULL,
	[ActionName] [nvarchar](50) NULL,
	[IconClass] [nvarchar](255) NULL,
	[SortOrder] [int] NOT NULL,
	[RequiredAuthLevel] [tinyint] NOT NULL,
	[ParentId] [int] NULL,
	[IsVisible] [bit] NOT NULL,
 CONSTRAINT [PK_SysMenus] PRIMARY KEY CLUSTERED 
(
	[MenuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[TaskSchedules]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskSchedules](
	[TaskID] [int] IDENTITY(1,1) NOT NULL,
	[StaffID] [nchar](13) NOT NULL,
	[InstitutionID] [nchar](6) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[StartTime] [datetime2](7) NOT NULL,
	[EndTime] [datetime2](7) NOT NULL,
	[TaskDescription] [nvarchar](max) NULL,
	[Status] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_TaskSchedules] PRIMARY KEY CLUSTERED 
(
	[TaskID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[UserCredentialDetails]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserCredentialDetails](
	[UserID] [nchar](13) NOT NULL,
	[CredentialID] [nchar](3) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[Identifier] [nvarchar](255) NOT NULL,
	[PasswordHash] [nvarchar](255) NULL,
	[PasswordSalt] [nvarchar](255) NULL,
 CONSTRAINT [PK_UserCredentialDetails] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[CredentialID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[UserCredentials]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserCredentials](
	[CredentialID] [nchar](3) NOT NULL,
	[AuthProvider] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_UserCredentials] PRIMARY KEY CLUSTERED 
(
	[CredentialID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[UserProfiles]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProfiles](
	[UserID] [nchar](13) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[Authorization] [tinyint] NOT NULL,
	[Gender] [bit] NULL,
	[DateOfBirth] [datetime2](7) NULL,
	[Address] [nvarchar](255) NULL,
	[JoinDate] [datetime2](7) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[Email] [nvarchar](60) NOT NULL,
 CONSTRAINT [PK_UserProfiles] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_Appointments_InstitutionID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_Appointments_InstitutionID] ON [dbo].[Appointments]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_Appointments_PetID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_Appointments_PetID] ON [dbo].[Appointments]
(
	[PetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_AuthorizationRecords_InstitutionID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_AuthorizationRecords_InstitutionID] ON [dbo].[AuthorizationRecords]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_AuthorizationRecords_PetID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_AuthorizationRecords_PetID] ON [dbo].[AuthorizationRecords]
(
	[PetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_CareGroomingLogs_PetID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_CareGroomingLogs_PetID] ON [dbo].[CareGroomingLogs]
(
	[PetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_CareGroomingLogs_StaffID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_CareGroomingLogs_StaffID] ON [dbo].[CareGroomingLogs]
(
	[StaffID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_Employments_InstitutionID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_Employments_InstitutionID] ON [dbo].[Employments]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_Employments_OccupationID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_Employments_OccupationID] ON [dbo].[Employments]
(
	[OccupationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_LogPhotos_LogID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_LogPhotos_LogID] ON [dbo].[LogPhotos]
(
	[LogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_MedicalRecords_PetID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_MedicalRecords_PetID] ON [dbo].[MedicalRecords]
(
	[PetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_MedicalRecords_VetID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_MedicalRecords_VetID] ON [dbo].[MedicalRecords]
(
	[VetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_Pets_OwnerID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_Pets_OwnerID] ON [dbo].[Pets]
(
	[OwnerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_PetWeight_PetID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_PetWeight_PetID] ON [dbo].[PetWeight]
(
	[PetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_Prescriptions_RecordID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_Prescriptions_RecordID] ON [dbo].[Prescriptions]
(
	[RecordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_RecordDiagnoses_RecordID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_RecordDiagnoses_RecordID] ON [dbo].[RecordDiagnoses]
(
	[RecordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_RecordDiagnoses_TermID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_RecordDiagnoses_TermID] ON [dbo].[RecordDiagnoses]
(
	[TermID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_TaskSchedules_InstitutionID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_TaskSchedules_InstitutionID] ON [dbo].[TaskSchedules]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_TaskSchedules_StaffID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_TaskSchedules_StaffID] ON [dbo].[TaskSchedules]
(
	[StaffID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_UserCredentialDetails_CredentialID]    指令碼日期: 2026/8/10 上午 12:51:09 ******/
CREATE NONCLUSTERED INDEX [IX_UserCredentialDetails_CredentialID] ON [dbo].[UserCredentialDetails]
(
	[CredentialID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Employments] ADD  DEFAULT (N'') FOR [OccupationID]
GO
ALTER TABLE [dbo].[UserCredentialDetails] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[UserProfiles] ADD  DEFAULT (getdate()) FOR [JoinDate]
GO
ALTER TABLE [dbo].[Appointments]  WITH CHECK ADD  CONSTRAINT [FK_Appointments_Institutions_InstitutionID] FOREIGN KEY([InstitutionID])
REFERENCES [dbo].[Institutions] ([InstitutionID])
GO
ALTER TABLE [dbo].[Appointments] CHECK CONSTRAINT [FK_Appointments_Institutions_InstitutionID]
GO
ALTER TABLE [dbo].[Appointments]  WITH CHECK ADD  CONSTRAINT [FK_Appointments_Pets_PetID] FOREIGN KEY([PetID])
REFERENCES [dbo].[Pets] ([PetID])
GO
ALTER TABLE [dbo].[Appointments] CHECK CONSTRAINT [FK_Appointments_Pets_PetID]
GO
ALTER TABLE [dbo].[AuthorizationRecords]  WITH CHECK ADD  CONSTRAINT [FK_AuthorizationRecords_Institutions_InstitutionID] FOREIGN KEY([InstitutionID])
REFERENCES [dbo].[Institutions] ([InstitutionID])
GO
ALTER TABLE [dbo].[AuthorizationRecords] CHECK CONSTRAINT [FK_AuthorizationRecords_Institutions_InstitutionID]
GO
ALTER TABLE [dbo].[AuthorizationRecords]  WITH CHECK ADD  CONSTRAINT [FK_AuthorizationRecords_Pets_PetID] FOREIGN KEY([PetID])
REFERENCES [dbo].[Pets] ([PetID])
GO
ALTER TABLE [dbo].[AuthorizationRecords] CHECK CONSTRAINT [FK_AuthorizationRecords_Pets_PetID]
GO
ALTER TABLE [dbo].[CareGroomingLogs]  WITH CHECK ADD  CONSTRAINT [FK_CareGroomingLogs_Pets_PetID] FOREIGN KEY([PetID])
REFERENCES [dbo].[Pets] ([PetID])
GO
ALTER TABLE [dbo].[CareGroomingLogs] CHECK CONSTRAINT [FK_CareGroomingLogs_Pets_PetID]
GO
ALTER TABLE [dbo].[CareGroomingLogs]  WITH CHECK ADD  CONSTRAINT [FK_CareGroomingLogs_UserProfiles_StaffID] FOREIGN KEY([StaffID])
REFERENCES [dbo].[UserProfiles] ([UserID])
GO
ALTER TABLE [dbo].[CareGroomingLogs] CHECK CONSTRAINT [FK_CareGroomingLogs_UserProfiles_StaffID]
GO
ALTER TABLE [dbo].[Employments]  WITH CHECK ADD  CONSTRAINT [FK_Employments_Institutions_InstitutionID] FOREIGN KEY([InstitutionID])
REFERENCES [dbo].[Institutions] ([InstitutionID])
GO
ALTER TABLE [dbo].[Employments] CHECK CONSTRAINT [FK_Employments_Institutions_InstitutionID]
GO
ALTER TABLE [dbo].[Employments]  WITH CHECK ADD  CONSTRAINT [FK_Employments_Occupations_OccupationID] FOREIGN KEY([OccupationID])
REFERENCES [dbo].[Occupations] ([OccupationID])
GO
ALTER TABLE [dbo].[Employments] CHECK CONSTRAINT [FK_Employments_Occupations_OccupationID]
GO
ALTER TABLE [dbo].[Employments]  WITH CHECK ADD  CONSTRAINT [FK_Employments_UserProfiles_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[UserProfiles] ([UserID])
GO
ALTER TABLE [dbo].[Employments] CHECK CONSTRAINT [FK_Employments_UserProfiles_UserID]
GO
ALTER TABLE [dbo].[LogPhotos]  WITH CHECK ADD  CONSTRAINT [FK_LogPhotos_CareGroomingLogs_LogID] FOREIGN KEY([LogID])
REFERENCES [dbo].[CareGroomingLogs] ([LogID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LogPhotos] CHECK CONSTRAINT [FK_LogPhotos_CareGroomingLogs_LogID]
GO
ALTER TABLE [dbo].[MedicalRecords]  WITH CHECK ADD  CONSTRAINT [FK_MedicalRecords_Pets_PetID] FOREIGN KEY([PetID])
REFERENCES [dbo].[Pets] ([PetID])
GO
ALTER TABLE [dbo].[MedicalRecords] CHECK CONSTRAINT [FK_MedicalRecords_Pets_PetID]
GO
ALTER TABLE [dbo].[MedicalRecords]  WITH CHECK ADD  CONSTRAINT [FK_MedicalRecords_UserProfiles_VetID] FOREIGN KEY([VetID])
REFERENCES [dbo].[UserProfiles] ([UserID])
GO
ALTER TABLE [dbo].[MedicalRecords] CHECK CONSTRAINT [FK_MedicalRecords_UserProfiles_VetID]
GO
ALTER TABLE [dbo].[Pets]  WITH CHECK ADD  CONSTRAINT [FK_Pets_UserProfiles_OwnerID] FOREIGN KEY([OwnerID])
REFERENCES [dbo].[UserProfiles] ([UserID])
GO
ALTER TABLE [dbo].[Pets] CHECK CONSTRAINT [FK_Pets_UserProfiles_OwnerID]
GO
ALTER TABLE [dbo].[PetWeight]  WITH CHECK ADD  CONSTRAINT [FK_PetWeight_Pets_PetID] FOREIGN KEY([PetID])
REFERENCES [dbo].[Pets] ([PetID])
GO
ALTER TABLE [dbo].[PetWeight] CHECK CONSTRAINT [FK_PetWeight_Pets_PetID]
GO
ALTER TABLE [dbo].[Prescriptions]  WITH CHECK ADD  CONSTRAINT [FK_Prescriptions_MedicalRecords_RecordID] FOREIGN KEY([RecordID])
REFERENCES [dbo].[MedicalRecords] ([RecordID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Prescriptions] CHECK CONSTRAINT [FK_Prescriptions_MedicalRecords_RecordID]
GO
ALTER TABLE [dbo].[RecordDiagnoses]  WITH CHECK ADD  CONSTRAINT [FK_RecordDiagnoses_MedicalRecords_RecordID] FOREIGN KEY([RecordID])
REFERENCES [dbo].[MedicalRecords] ([RecordID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RecordDiagnoses] CHECK CONSTRAINT [FK_RecordDiagnoses_MedicalRecords_RecordID]
GO
ALTER TABLE [dbo].[RecordDiagnoses]  WITH CHECK ADD  CONSTRAINT [FK_RecordDiagnoses_MedicalVocabularyDictionaries_TermID] FOREIGN KEY([TermID])
REFERENCES [dbo].[MedicalVocabularyDictionaries] ([TermID])
GO
ALTER TABLE [dbo].[RecordDiagnoses] CHECK CONSTRAINT [FK_RecordDiagnoses_MedicalVocabularyDictionaries_TermID]
GO
ALTER TABLE [dbo].[TaskSchedules]  WITH CHECK ADD  CONSTRAINT [FK_TaskSchedules_Institutions_InstitutionID] FOREIGN KEY([InstitutionID])
REFERENCES [dbo].[Institutions] ([InstitutionID])
GO
ALTER TABLE [dbo].[TaskSchedules] CHECK CONSTRAINT [FK_TaskSchedules_Institutions_InstitutionID]
GO
ALTER TABLE [dbo].[TaskSchedules]  WITH CHECK ADD  CONSTRAINT [FK_TaskSchedules_UserProfiles_StaffID] FOREIGN KEY([StaffID])
REFERENCES [dbo].[UserProfiles] ([UserID])
GO
ALTER TABLE [dbo].[TaskSchedules] CHECK CONSTRAINT [FK_TaskSchedules_UserProfiles_StaffID]
GO
ALTER TABLE [dbo].[UserCredentialDetails]  WITH CHECK ADD  CONSTRAINT [FK_UserCredentialDetails_UserCredentials_CredentialID] FOREIGN KEY([CredentialID])
REFERENCES [dbo].[UserCredentials] ([CredentialID])
GO
ALTER TABLE [dbo].[UserCredentialDetails] CHECK CONSTRAINT [FK_UserCredentialDetails_UserCredentials_CredentialID]
GO
ALTER TABLE [dbo].[UserCredentialDetails]  WITH CHECK ADD  CONSTRAINT [FK_UserCredentialDetails_UserProfiles_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[UserProfiles] ([UserID])
GO
ALTER TABLE [dbo].[UserCredentialDetails] CHECK CONSTRAINT [FK_UserCredentialDetails_UserProfiles_UserID]
GO
USE [master]
GO
ALTER DATABASE [PetCare] SET  READ_WRITE 
GO
