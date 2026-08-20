USE [master]
GO
/****** 物件:  Database [PetCare]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
/****** 物件:  Table [dbo].[__EFMigrationsHistory]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
/****** 物件:  Table [dbo].[Appointments]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
/****** 物件:  Table [dbo].[AuthorizationRecords]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
/****** 物件:  Table [dbo].[CareGroomingLogs]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
/****** 物件:  Table [dbo].[Employments]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
/****** 物件:  Table [dbo].[Institutions]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Institutions](
	[InstitutionID] [nchar](6) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[LicenseNumber] [nvarchar](50) NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[Address] [nvarchar](255) NOT NULL,
	[StatusId] [nvarchar](3) NOT NULL,
	[TypeId] [nvarchar](4) NOT NULL,
	[OwnerID] [nchar](13) NOT NULL,
 CONSTRAINT [PK_Institutions] PRIMARY KEY CLUSTERED 
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[InstitutionStatuses]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InstitutionStatuses](
	[StatusId] [nvarchar](3) NOT NULL,
	[StatusName] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_InstitutionStatuses] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[InstitutionTypes]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InstitutionTypes](
	[TypeId] [nvarchar](4) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_InstitutionTypes] PRIMARY KEY CLUSTERED 
(
	[TypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[LogPhotos]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogPhotos](
	[PhotoID] [int] IDENTITY(1,1) NOT NULL,
	[LogID] [int] NOT NULL,
	[ImageURL] [nvarchar](255) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_LogPhotos] PRIMARY KEY CLUSTERED 
(
	[PhotoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[MedicalRecords]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
/****** 物件:  Table [dbo].[MedicalVocabularyDictionaries]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
/****** 物件:  Table [dbo].[Occupations]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
/****** 物件:  Table [dbo].[Pets]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
/****** 物件:  Table [dbo].[PetWeight]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
/****** 物件:  Table [dbo].[Prescriptions]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
/****** 物件:  Table [dbo].[RecordDiagnoses]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
/****** 物件:  Table [dbo].[Role]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [tinyint] NOT NULL,
	[RoleName] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** 物件:  Table [dbo].[SysMenus]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
/****** 物件:  Table [dbo].[TaskSchedules]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
/****** 物件:  Table [dbo].[UserCredentialDetails]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
/****** 物件:  Table [dbo].[UserCredentials]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
/****** 物件:  Table [dbo].[UserProfiles]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
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
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260803035159_InitialCreate', N'10.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260804074031_AddSysMenuTable', N'10.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260804114712_AddPetPhotoColumn', N'10.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260807124252_UpdateEmploymentModel', N'10.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260808114832_AddPetWeightTableWithRelate', N'10.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260816075529_updateInst', N'10.0.11')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260817115536_addRoleTable', N'10.0.11')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260818143638_updateInst1', N'10.0.11')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260820140204_updateLogPhoto', N'10.0.11')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260820141721_AddLogPhotoCreatedDate', N'10.0.11')
GO
INSERT [dbo].[Appointments] ([AppointmentID], [PetID], [InstitutionID], [ServiceType], [ApptDateTime], [Status]) VALUES (N'A20260803-I00001-01 ', 1, N'I00001', N'健康檢查', CAST(N'2026-08-20T22:43:31.5692374' AS DateTime2), N'Confirmed')
INSERT [dbo].[Appointments] ([AppointmentID], [PetID], [InstitutionID], [ServiceType], [ApptDateTime], [Status]) VALUES (N'A20260803-I00002-01 ', 2, N'I00002', N'洗澡修剪', CAST(N'2026-08-23T22:43:31.5692374' AS DateTime2), N'Pending')
GO
INSERT [dbo].[AuthorizationRecords] ([DataAuthorizationNumber], [PetID], [InstitutionID], [GrantedDate], [RevokedDate], [Status]) VALUES (N'R20260803-I00001-01 ', 1, N'I00001', CAST(N'2026-02-18T22:43:31.5692374' AS DateTime2), NULL, N'Active')
INSERT [dbo].[AuthorizationRecords] ([DataAuthorizationNumber], [PetID], [InstitutionID], [GrantedDate], [RevokedDate], [Status]) VALUES (N'R20260803-I00002-01 ', 2, N'I00002', CAST(N'2026-06-18T22:43:31.5692374' AS DateTime2), NULL, N'Active')
GO
SET IDENTITY_INSERT [dbo].[CareGroomingLogs] ON 

INSERT [dbo].[CareGroomingLogs] ([LogID], [PetID], [StaffID], [LogDate], [ActivityDetails], [DietNotes]) VALUES (1, 2, N'U202607280003', CAST(N'2026-07-14T19:59:19.5180000' AS DateTime2), N'完成洗澡、剪指甲、清耳朵。情緒稍稍緊張但無攻擊性。', N'給予肉泥零食，進食正常。')
SET IDENTITY_INSERT [dbo].[CareGroomingLogs] OFF
GO
INSERT [dbo].[Employments] ([UserID], [InstitutionID], [Status], [LicenseNumber], [StartDate], [EndDate], [OccupationID]) VALUES (N'U202607280002', N'I00001', N'Active', N'VET-123456', CAST(N'2024-08-18T22:43:31.5692374' AS DateTime2), NULL, N'O09')
INSERT [dbo].[Employments] ([UserID], [InstitutionID], [Status], [LicenseNumber], [StartDate], [EndDate], [OccupationID]) VALUES (N'U202607280003', N'I00002', N'Active', N'GROOM-654321', CAST(N'2025-08-18T22:43:31.5692374' AS DateTime2), NULL, N'O11')
INSERT [dbo].[Employments] ([UserID], [InstitutionID], [Status], [LicenseNumber], [StartDate], [EndDate], [OccupationID]) VALUES (N'U202608170001', N'I00003', N'Active', N'abc-123', CAST(N'2026-08-18T23:36:55.7742638' AS DateTime2), NULL, N'O00')
GO
INSERT [dbo].[Institutions] ([InstitutionID], [Name], [LicenseNumber], [Phone], [Address], [StatusId], [TypeId], [OwnerID]) VALUES (N'I00001', N'快樂動物醫院', NULL, N'07-1234567', N'高雄市苓雅區某某路1號', N'S01', N'IT01', N'U202607280002')
INSERT [dbo].[Institutions] ([InstitutionID], [Name], [LicenseNumber], [Phone], [Address], [StatusId], [TypeId], [OwnerID]) VALUES (N'I00002', N'毛小孩美容坊', NULL, N'07-7654321', N'高雄市鳳山區某某路2號', N'S01', N'IT02', N'U202607280003')
INSERT [dbo].[Institutions] ([InstitutionID], [Name], [LicenseNumber], [Phone], [Address], [StatusId], [TypeId], [OwnerID]) VALUES (N'I00003', N'恩澤動物醫院', N'abc-123', N'077654321', N'澄清路33巷34號', N'S00', N'IT01', N'U202608170001')
GO
INSERT [dbo].[InstitutionStatuses] ([StatusId], [StatusName]) VALUES (N'S00', N'審核中')
INSERT [dbo].[InstitutionStatuses] ([StatusId], [StatusName]) VALUES (N'S01', N'啟用')
INSERT [dbo].[InstitutionStatuses] ([StatusId], [StatusName]) VALUES (N'S02', N'停用')
GO
INSERT [dbo].[InstitutionTypes] ([TypeId], [Name]) VALUES (N'IT01', N'動物醫院')
INSERT [dbo].[InstitutionTypes] ([TypeId], [Name]) VALUES (N'IT02', N'寵物美容坊')
GO
SET IDENTITY_INSERT [dbo].[LogPhotos] ON 

INSERT [dbo].[LogPhotos] ([PhotoID], [LogID], [ImageURL], [CreatedDate]) VALUES (1, 1, N'U202607280004_2log01.jpg', CAST(N'2026-08-20T00:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[LogPhotos] OFF
GO
SET IDENTITY_INSERT [dbo].[MedicalRecords] ON 

INSERT [dbo].[MedicalRecords] ([RecordID], [PetID], [VetID], [VisitDate], [Symptoms], [TestResultsFile], [Diagnosis]) VALUES (1, 1, N'U202607280002', CAST(N'2026-07-24T19:59:19.5185394' AS DateTime2), N'食慾不佳，連續嘔吐兩次', NULL, N'急性腸胃炎')
INSERT [dbo].[MedicalRecords] ([RecordID], [PetID], [VetID], [VisitDate], [Symptoms], [TestResultsFile], [Diagnosis]) VALUES (2, 22, N'U202607280002', CAST(N'2026-08-07T21:19:00.0000000' AS DateTime2), N'123', NULL, N'123')
INSERT [dbo].[MedicalRecords] ([RecordID], [PetID], [VetID], [VisitDate], [Symptoms], [TestResultsFile], [Diagnosis]) VALUES (3, 22, N'U202607280002', CAST(N'2026-08-07T21:37:00.0000000' AS DateTime2), N'123', N'c67b45f5-52e1-42e8-b5dc-a30d7c9973f9_商務系統軟體分析第一次實作練習01.docx', NULL)
INSERT [dbo].[MedicalRecords] ([RecordID], [PetID], [VetID], [VisitDate], [Symptoms], [TestResultsFile], [Diagnosis]) VALUES (4, 22, N'U202607280002', CAST(N'2026-08-07T21:58:00.0000000' AS DateTime2), N'456', N'83beefdb-106b-4194-9ed2-f5eb1e40a7a8_商務系統軟體分析第一次實作練習.docx', NULL)
SET IDENTITY_INSERT [dbo].[MedicalRecords] OFF
GO
SET IDENTITY_INSERT [dbo].[MedicalVocabularyDictionaries] ON 

INSERT [dbo].[MedicalVocabularyDictionaries] ([TermID], [TermCode], [Category], [TermName], [Description], [UpdatedAt]) VALUES (1, N'ICD10-A09', N'Disease', N'腸胃炎', NULL, CAST(N'2026-08-03T19:59:19.5180000' AS DateTime2))
INSERT [dbo].[MedicalVocabularyDictionaries] ([TermID], [TermCode], [Category], [TermName], [Description], [UpdatedAt]) VALUES (2, N'ICD10-R11', N'Symptom', N'噁心與嘔吐', NULL, CAST(N'2026-08-03T19:59:19.5180000' AS DateTime2))
INSERT [dbo].[MedicalVocabularyDictionaries] ([TermID], [TermCode], [Category], [TermName], [Description], [UpdatedAt]) VALUES (3, N'abc', N'def', N'ghi', N'jklm', CAST(N'2026-08-19T21:13:17.5435065' AS DateTime2))
INSERT [dbo].[MedicalVocabularyDictionaries] ([TermID], [TermCode], [Category], [TermName], [Description], [UpdatedAt]) VALUES (5, N'test', N'test', N'test', NULL, CAST(N'2026-08-19T21:31:20.8990206' AS DateTime2))
SET IDENTITY_INSERT [dbo].[MedicalVocabularyDictionaries] OFF
GO
INSERT [dbo].[Occupations] ([OccupationID], [OccupationName]) VALUES (N'O00', N'機構管理者')
INSERT [dbo].[Occupations] ([OccupationID], [OccupationName]) VALUES (N'O01', N'獸醫師')
INSERT [dbo].[Occupations] ([OccupationID], [OccupationName]) VALUES (N'O02', N'獸醫助理')
INSERT [dbo].[Occupations] ([OccupationID], [OccupationName]) VALUES (N'O09', N'預設獸醫')
INSERT [dbo].[Occupations] ([OccupationID], [OccupationName]) VALUES (N'O11', N'寵物美容師')
INSERT [dbo].[Occupations] ([OccupationID], [OccupationName]) VALUES (N'O19', N'預設美容師')
INSERT [dbo].[Occupations] ([OccupationID], [OccupationName]) VALUES (N'O21', N'寵物保姆')
INSERT [dbo].[Occupations] ([OccupationID], [OccupationName]) VALUES (N'O29', N'預設保姆')
GO
SET IDENTITY_INSERT [dbo].[Pets] ON 

INSERT [dbo].[Pets] ([PetID], [OwnerID], [Name], [Species], [Breed], [Gender], [DateOfBirth], [MicrochipNo], [SpecialNotes], [ProfilePhotoUrl]) VALUES (1, N'U202607280004', N'小白', N'狗', N'馬爾濟斯', N'公', CAST(N'2021-05-10T00:00:00.0000000' AS DateTime2), NULL, N'皮膚容易過敏', NULL)
INSERT [dbo].[Pets] ([PetID], [OwnerID], [Name], [Species], [Breed], [Gender], [DateOfBirth], [MicrochipNo], [SpecialNotes], [ProfilePhotoUrl]) VALUES (2, N'U202607280004', N'胖橘', N'貓', N'米克斯 (Mix)', N'母', CAST(N'2022-10-01T00:00:00.0000000' AS DateTime2), NULL, N'很愛吃
^^', N'/uploads/pets/U202607280004_2.jpg')
INSERT [dbo].[Pets] ([PetID], [OwnerID], [Name], [Species], [Breed], [Gender], [DateOfBirth], [MicrochipNo], [SpecialNotes], [ProfilePhotoUrl]) VALUES (4, N'U198805150001', N'Kuro', N'貓', N'米克斯 (Mix)', N'母(絕育)', CAST(N'2015-10-28T00:00:00.0000000' AS DateTime2), NULL, N'很兇
><', N'/uploads/pets/U198805150001_4.jpg')
INSERT [dbo].[Pets] ([PetID], [OwnerID], [Name], [Species], [Breed], [Gender], [DateOfBirth], [MicrochipNo], [SpecialNotes], [ProfilePhotoUrl]) VALUES (6, N'U202607280004', N'Kuro', N'貓', N'米克斯 (Mix)', N'母(絕育)', CAST(N'2015-10-28T00:00:00.0000000' AS DateTime2), NULL, N'很兇><', N'/uploads/pets/U202607280004_6.jpg')
INSERT [dbo].[Pets] ([PetID], [OwnerID], [Name], [Species], [Breed], [Gender], [DateOfBirth], [MicrochipNo], [SpecialNotes], [ProfilePhotoUrl]) VALUES (8, N'U202607280001', N'うさぎ', N'兔子', N'道奇兔', N'其它', NULL, NULL, NULL, N'/uploads/pets/U202607280001_8.png')
INSERT [dbo].[Pets] ([PetID], [OwnerID], [Name], [Species], [Breed], [Gender], [DateOfBirth], [MicrochipNo], [SpecialNotes], [ProfilePhotoUrl]) VALUES (10, N'U202607280001', N'ちいかわ', N'鼠/貂 (小型哺乳類)', N'倉鼠 (三線/一線/老公公)', N'母', NULL, NULL, NULL, N'/uploads/pets/U202607280001_10.png')
INSERT [dbo].[Pets] ([PetID], [OwnerID], [Name], [Species], [Breed], [Gender], [DateOfBirth], [MicrochipNo], [SpecialNotes], [ProfilePhotoUrl]) VALUES (11, N'U202607280001', N'ハチワレ', N'貓', N'米克斯 (Mix)', N'公', NULL, NULL, NULL, N'/uploads/pets/U202607280001_11.png')
INSERT [dbo].[Pets] ([PetID], [OwnerID], [Name], [Species], [Breed], [Gender], [DateOfBirth], [MicrochipNo], [SpecialNotes], [ProfilePhotoUrl]) VALUES (12, N'U202607280001', N'test', N'狗', N'法國鬥牛犬', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Pets] ([PetID], [OwnerID], [Name], [Species], [Breed], [Gender], [DateOfBirth], [MicrochipNo], [SpecialNotes], [ProfilePhotoUrl]) VALUES (13, N'U202607280001', N'ff', N'貓', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Pets] ([PetID], [OwnerID], [Name], [Species], [Breed], [Gender], [DateOfBirth], [MicrochipNo], [SpecialNotes], [ProfilePhotoUrl]) VALUES (22, N'U202608060002', N'Giraffe', N'其他', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Pets] ([PetID], [OwnerID], [Name], [Species], [Breed], [Gender], [DateOfBirth], [MicrochipNo], [SpecialNotes], [ProfilePhotoUrl]) VALUES (23, N'U202608060002', N'Elephant', N'其他', NULL, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Pets] OFF
GO
SET IDENTITY_INSERT [dbo].[PetWeight] ON 

INSERT [dbo].[PetWeight] ([PetWeightID], [PetID], [Weight], [RecordDate]) VALUES (1, 23, CAST(5.50 AS Decimal(5, 2)), CAST(N'2026-08-08T20:16:42.990' AS DateTime))
INSERT [dbo].[PetWeight] ([PetWeightID], [PetID], [Weight], [RecordDate]) VALUES (2, 23, CAST(5.51 AS Decimal(5, 2)), CAST(N'2026-08-08T20:51:00.497' AS DateTime))
INSERT [dbo].[PetWeight] ([PetWeightID], [PetID], [Weight], [RecordDate]) VALUES (3, 23, CAST(5.52 AS Decimal(5, 2)), CAST(N'2026-08-08T20:52:25.537' AS DateTime))
INSERT [dbo].[PetWeight] ([PetWeightID], [PetID], [Weight], [RecordDate]) VALUES (4, 23, CAST(5.52 AS Decimal(5, 2)), CAST(N'2026-08-08T20:53:04.510' AS DateTime))
INSERT [dbo].[PetWeight] ([PetWeightID], [PetID], [Weight], [RecordDate]) VALUES (5, 23, CAST(5.00 AS Decimal(5, 2)), CAST(N'2026-08-08T21:25:24.490' AS DateTime))
INSERT [dbo].[PetWeight] ([PetWeightID], [PetID], [Weight], [RecordDate]) VALUES (6, 23, CAST(6.70 AS Decimal(5, 2)), CAST(N'2026-08-08T21:25:52.083' AS DateTime))
INSERT [dbo].[PetWeight] ([PetWeightID], [PetID], [Weight], [RecordDate]) VALUES (7, 23, CAST(5.00 AS Decimal(5, 2)), CAST(N'2026-08-08T21:28:03.337' AS DateTime))
INSERT [dbo].[PetWeight] ([PetWeightID], [PetID], [Weight], [RecordDate]) VALUES (10, 23, CAST(6.00 AS Decimal(5, 2)), CAST(N'2026-08-08T22:31:35.677' AS DateTime))
INSERT [dbo].[PetWeight] ([PetWeightID], [PetID], [Weight], [RecordDate]) VALUES (11, 23, CAST(5.50 AS Decimal(5, 2)), CAST(N'2026-08-08T22:41:00.130' AS DateTime))
INSERT [dbo].[PetWeight] ([PetWeightID], [PetID], [Weight], [RecordDate]) VALUES (12, 23, CAST(5.60 AS Decimal(5, 2)), CAST(N'2026-08-08T22:44:45.250' AS DateTime))
INSERT [dbo].[PetWeight] ([PetWeightID], [PetID], [Weight], [RecordDate]) VALUES (13, 23, CAST(5.70 AS Decimal(5, 2)), CAST(N'2026-08-08T22:45:59.883' AS DateTime))
INSERT [dbo].[PetWeight] ([PetWeightID], [PetID], [Weight], [RecordDate]) VALUES (14, 23, CAST(5.75 AS Decimal(5, 2)), CAST(N'2026-08-08T22:46:15.450' AS DateTime))
INSERT [dbo].[PetWeight] ([PetWeightID], [PetID], [Weight], [RecordDate]) VALUES (15, 23, CAST(5.78 AS Decimal(5, 2)), CAST(N'2026-08-08T22:46:38.613' AS DateTime))
INSERT [dbo].[PetWeight] ([PetWeightID], [PetID], [Weight], [RecordDate]) VALUES (16, 23, CAST(5.79 AS Decimal(5, 2)), CAST(N'2026-08-08T22:46:50.853' AS DateTime))
SET IDENTITY_INSERT [dbo].[PetWeight] OFF
GO
SET IDENTITY_INSERT [dbo].[Prescriptions] ON 

INSERT [dbo].[Prescriptions] ([PrescriptionID], [RecordID], [MedicationName], [Dosage], [Frequency], [Instructions]) VALUES (1, 1, N'Cerenia 止吐藥', N'16mg', N'一天一次', N'飯後餵食，若持續嘔吐須回診')
SET IDENTITY_INSERT [dbo].[Prescriptions] OFF
GO
SET IDENTITY_INSERT [dbo].[RecordDiagnoses] ON 

INSERT [dbo].[RecordDiagnoses] ([DetailID], [RecordID], [TermID], [Severity], [Remark], [CreatedDate]) VALUES (1, 1, 1, N'Moderate', N'注意水分補充', CAST(N'2026-08-03T19:59:19.5185394' AS DateTime2))
SET IDENTITY_INSERT [dbo].[RecordDiagnoses] OFF
GO
INSERT [dbo].[Role] ([Id], [RoleName]) VALUES (0, N'Member')
INSERT [dbo].[Role] ([Id], [RoleName]) VALUES (1, N'Vet')
INSERT [dbo].[Role] ([Id], [RoleName]) VALUES (2, N'Beauty')
INSERT [dbo].[Role] ([Id], [RoleName]) VALUES (3, N'Nanny')
INSERT [dbo].[Role] ([Id], [RoleName]) VALUES (4, N'VetManage')
INSERT [dbo].[Role] ([Id], [RoleName]) VALUES (5, N'BeautyManage')
INSERT [dbo].[Role] ([Id], [RoleName]) VALUES (6, N'NannyManage')
INSERT [dbo].[Role] ([Id], [RoleName]) VALUES (9, N'Admin')
GO
SET IDENTITY_INSERT [dbo].[SysMenus] ON 

INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (2, N'使用者列表', N'UserProfile', N'Index', NULL, 0, 9, NULL, 1)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (3, N'寵物列表', N'Pet', N'Index', NULL, 0, 0, NULL, 1)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (4, N'新增寵物', N'Pet', N'Create', NULL, 0, 0, NULL, 0)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (5, N'編輯寵物資料', N'Pet', N'Edit', NULL, 0, 0, NULL, 0)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (6, N'寵物詳細資料', N'Pet', N'Details', NULL, 0, 0, NULL, 0)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (7, N'病歷管理', N'MedicalRecord', N'Index', NULL, 0, 0, NULL, 1)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (8, N'建立病歷', N'MedicalRecord', N'Create', NULL, 0, 0, NULL, 0)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (10, N'建立使用者(後台)', N'AdminUser', N'Create', NULL, 0, 9, NULL, 1)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (11, N'體重管理', N'PetWeight', N'Index', NULL, 0, 0, NULL, 0)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (12, N'建立量測紀錄', N'PetWeight', N'Create', NULL, 0, 0, NULL, 0)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (13, N'照護日誌列表', N'CareGroomingLog', N'Index', NULL, 0, 0, NULL, 1)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (14, N'日誌編輯', N'CareGroomingLog', N'Edit', NULL, 0, 0, NULL, 0)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (15, N'機構管理', N'Institution', N'Index', NULL, 0, 9, NULL, 1)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (16, N'申請建立機構', N'Institution', N'Create', NULL, 0, 0, NULL, 1)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (17, N'機構編輯', N'Institution', N'Edit', NULL, 0, 0, NULL, 0)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (18, N'職務管理', N'Occupations', N'Index', NULL, 0, 9, NULL, 1)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (20, N'員工管理(manager)', N'Employment', N'Index', NULL, 0, 4, NULL, 1)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (21, N'詞彙庫', N'MedicalVocabularyDictionary', N'Index', NULL, 0, 9, NULL, 1)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (23, N'詞彙編輯', N'MedicalVocabularyDictionary', N'Edit', NULL, 0, 9, NULL, 0)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (25, N'建立詞彙', N'MedicalVocabularyDictionary', N'Create', NULL, 0, 9, NULL, 0)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (26, N'功能管理', N'SysMenu', N'Index', NULL, 0, 9, NULL, 1)
INSERT [dbo].[SysMenus] ([MenuId], [MenuName], [ControllerName], [ActionName], [IconClass], [SortOrder], [RequiredAuthLevel], [ParentId], [IsVisible]) VALUES (27, N'照護明細', N'CareGroomingLog', N'Details', NULL, 0, 0, NULL, 0)
SET IDENTITY_INSERT [dbo].[SysMenus] OFF
GO
SET IDENTITY_INSERT [dbo].[TaskSchedules] ON 

INSERT [dbo].[TaskSchedules] ([TaskID], [StaffID], [InstitutionID], [CreatedDate], [StartTime], [EndTime], [TaskDescription], [Status]) VALUES (3, N'U202607280003', N'I00002', CAST(N'2026-08-18T22:43:31.5692374' AS DateTime2), CAST(N'2026-08-18T23:43:31.5692374' AS DateTime2), CAST(N'2026-08-19T03:43:31.5692374' AS DateTime2), N'下午場次美容服務排班', N'Pending')
SET IDENTITY_INSERT [dbo].[TaskSchedules] OFF
GO
INSERT [dbo].[UserCredentialDetails] ([UserID], [CredentialID], [CreatedDate], [Identifier], [PasswordHash], [PasswordSalt]) VALUES (N'U198805150001', N'C01', CAST(N'2026-08-03T19:00:00.0000000' AS DateTime2), N'geo.light.deathnote@gmail.com', N'KA24uCGeQEuRKPpA6OLRW9BxUpWieoroLsXdfC/33Ke+mgB/zeJJ8t4Ah07muXNESL685RQLDaRlUEHbAQkXEw==', N'gQFEMrh4ZF6Q9R/RGeCNuUL9ZhPCTsFxC4YKFWfWLA8YXkpazNd45tmPN00QcuF3fTXI8CaWzkct7X1vEbRqtOgMrWKXBaSDj60KoIKTrL8z9PBBfij/s0t5+FW8F292LlRSS8F7s3rrvUP+2Rf7Idkp+9e8NtekQJKZ9POAF70=')
INSERT [dbo].[UserCredentialDetails] ([UserID], [CredentialID], [CreatedDate], [Identifier], [PasswordHash], [PasswordSalt]) VALUES (N'U198805150001', N'C02', CAST(N'2026-08-17T20:11:17.5305483' AS DateTime2), N'geo.light.deathnote@gmail.com', NULL, NULL)
INSERT [dbo].[UserCredentialDetails] ([UserID], [CredentialID], [CreatedDate], [Identifier], [PasswordHash], [PasswordSalt]) VALUES (N'U202607280001', N'C01', CAST(N'2026-08-03T19:59:19.5185394' AS DateTime2), N'admin', N'GL8nNT5OUWbC0YWLP5XTNEkvHxVRHc6r6Z6xEbvy/XHx0SPoZLFJYDQhbB0C1uMHDxGm/SlyELKeEmrHvsThWA==', N'6/RL23DB62PfsmlAQvGSYxMqzE9rOxDOO28aOxN7FDObhMLy0N1xop/j4H89uGlG8FYXAZ6C6QQcyLlNKT2/UI4MdXYc9dHIiXOcKw70gn+k+FB/qsRa8ftfxJwc1/HeK0z4xsgSCndUhqh1180NQ+LpdfWeFxzjBsnsz1I5Psg=')
INSERT [dbo].[UserCredentialDetails] ([UserID], [CredentialID], [CreatedDate], [Identifier], [PasswordHash], [PasswordSalt]) VALUES (N'U202607280002', N'C01', CAST(N'2026-08-03T19:59:19.5185394' AS DateTime2), N'vet@test.com', N'q7d3vOUX0r/vNnbF5/o0QJqUPy2NXCsTZ6zZZPDutHbVtOr8gvErPBhxtHGayap2ot7tYyPaK2c2n7Hntj1+Og==', N'cnlYeOE1AZsdvLdzVADjq1j8PqzPmVazU46FdBv4PMmZWdyBGjeGHTYOSoLiV49K1JQjCM4pIf+qx8DcYj11bkmLczNO0ANIuh/6MbqFko3OIf5aop6hOvYQYI8WsdqdJx4EtNfPN4LnYh/Y4jVfU0WrUm7jYQZothUAFj1tfjo=')
INSERT [dbo].[UserCredentialDetails] ([UserID], [CredentialID], [CreatedDate], [Identifier], [PasswordHash], [PasswordSalt]) VALUES (N'U202607280004', N'C01', CAST(N'2026-08-03T19:59:19.5185394' AS DateTime2), N'google_id_12345', N'1Ri6epLAKBMiU6Xt4hioyPiEGPDUb8K0LkFnuTcGkrZE2XIRwd+3lgHKPlRTDksPwqDBHcD1B7aTf637IFGEmQ==', N'DOeR5cjpCAm6qjhKgYZikI6hegLw8aqseUI4/cjkJkOUpn1oePySLJy2D+sJsDyrlBf2aZdOuG+keifpN0FVkS5WH33ekskm5Ro6MoJM0wA4H6M8BKh7twLixQXZZ02QZ6dSbsq50gGwfEwiUBvtZjqYGeY2VGreGqRZgwZqGfM=')
INSERT [dbo].[UserCredentialDetails] ([UserID], [CredentialID], [CreatedDate], [Identifier], [PasswordHash], [PasswordSalt]) VALUES (N'U202608060001', N'C01', CAST(N'2026-08-06T00:29:56.2290297' AS DateTime2), N'admin@geo.com', N'dFE7vyrhQAf8mWSjn8zGyc3umJVJrUtndUI0pmaDj7BxS7c6AcIYZtH3N7q/mZq4XZlWbC8JAtjSbsIqHoyejg==', N'taGqwcbpEBqnGo01f76D0iS0/jC3g8dfA7c2mHIEEKKP61hnLdYBj3AzVgXFdaq2xKpxYTeHFQOG5d0LtPSrUHPsQ28QGKpVEbFjhOd3LBy+/69TMkg5HtafLPOIBykfZuNH7UK1V9wFvPtR2+CoxF0iIwOut4aPKk1XajlnnEA=')
INSERT [dbo].[UserCredentialDetails] ([UserID], [CredentialID], [CreatedDate], [Identifier], [PasswordHash], [PasswordSalt]) VALUES (N'U202608060002', N'C01', CAST(N'2026-08-06T13:31:54.9574495' AS DateTime2), N'owner@geo.com', N'CUPDXvfQ0rSJAUbFdDeYJquIqpMR3HEDGPTCrNJmYmhulF2KoesvBfoZhQSotOTpKDGmj2aQweKGjvtF13ICXg==', N'FfjJWwvsmg9ivB/TsmbmjXRGIYk39vxlkxAOUC69pOgsLCUD0DQ4U2jmI387Vzy42MZHp0x8to97becHZr7YtsC2ucqhtcr8U5pbnt/Y8eQIVc4JWNgYAo7T5QExvDsiLAZEOybOUbJ1iQ91d/EHJdi1QhttlNfvlEIzVu2blRA=')
INSERT [dbo].[UserCredentialDetails] ([UserID], [CredentialID], [CreatedDate], [Identifier], [PasswordHash], [PasswordSalt]) VALUES (N'U202608170001', N'C01', CAST(N'2026-08-17T20:45:30.0612154' AS DateTime2), N'vetManage@test.com', N'r8B3NUGDrXfdY3L0dFW13Wa7gwwKwftwVi1hXdxu99UuCvoNYHrhmgaUQtCxCoTa4PR2WYOpibdqXb0f+fJQQg==', N'OIOedTkJR1w77PY1DLdHopiWjbxcno6k72eWAPsEgnFPre3zTq8WwsZNyHebwei5W1dT0mx0JidQjAtw76nx7+mzkCuW+4IiaH/tzDxwmwQU+WH2f5+lyv4ipLrrfz4KDJ/W2K28ZLRK88aO1h10uZReGD6UwnVpfv+pk5JZ1SA=')
GO
INSERT [dbo].[UserCredentials] ([CredentialID], [AuthProvider]) VALUES (N'C01', N'Local')
INSERT [dbo].[UserCredentials] ([CredentialID], [AuthProvider]) VALUES (N'C02', N'Google')
INSERT [dbo].[UserCredentials] ([CredentialID], [AuthProvider]) VALUES (N'C03', N'Line')
GO
INSERT [dbo].[UserProfiles] ([UserID], [Name], [Phone], [Authorization], [Gender], [DateOfBirth], [Address], [JoinDate], [Status], [Email]) VALUES (N'U198805150001', N'吳宗鴻', N'0912813570', 9, NULL, NULL, NULL, CAST(N'2026-08-03T20:14:00.0000000' AS DateTime2), 1, N'geo.light.deathnote@gmail.com')
INSERT [dbo].[UserProfiles] ([UserID], [Name], [Phone], [Authorization], [Gender], [DateOfBirth], [Address], [JoinDate], [Status], [Email]) VALUES (N'U202607280001', N'系統管理員', N'0900000000', 9, NULL, NULL, NULL, CAST(N'2026-08-03T19:59:19.5185394' AS DateTime2), 1, N'admin@test.com')
INSERT [dbo].[UserProfiles] ([UserID], [Name], [Phone], [Authorization], [Gender], [DateOfBirth], [Address], [JoinDate], [Status], [Email]) VALUES (N'U202607280002', N'預設獸醫', N'0911111111', 1, NULL, NULL, NULL, CAST(N'2026-08-03T19:59:19.5185394' AS DateTime2), 1, N'vet@test.com')
INSERT [dbo].[UserProfiles] ([UserID], [Name], [Phone], [Authorization], [Gender], [DateOfBirth], [Address], [JoinDate], [Status], [Email]) VALUES (N'U202607280003', N'陳美容', N'0922222222', 2, NULL, NULL, NULL, CAST(N'2026-08-03T19:59:19.5185394' AS DateTime2), 1, N'groomer@test.com')
INSERT [dbo].[UserProfiles] ([UserID], [Name], [Phone], [Authorization], [Gender], [DateOfBirth], [Address], [JoinDate], [Status], [Email]) VALUES (N'U202607280004', N'林飼主', N'0933333333', 0, NULL, NULL, NULL, CAST(N'2026-08-03T19:59:19.5185394' AS DateTime2), 1, N'owner@test.com')
INSERT [dbo].[UserProfiles] ([UserID], [Name], [Phone], [Authorization], [Gender], [DateOfBirth], [Address], [JoinDate], [Status], [Email]) VALUES (N'U202608060001', N'Admin', N'0912813570', 9, NULL, NULL, NULL, CAST(N'2026-08-06T00:29:56.2283975' AS DateTime2), 1, N'admin@geo.com')
INSERT [dbo].[UserProfiles] ([UserID], [Name], [Phone], [Authorization], [Gender], [DateOfBirth], [Address], [JoinDate], [Status], [Email]) VALUES (N'U202608060002', N'一般飼主', N'0912345678', 0, NULL, NULL, NULL, CAST(N'2026-08-06T13:31:54.9547662' AS DateTime2), 1, N'owner@geo.com')
INSERT [dbo].[UserProfiles] ([UserID], [Name], [Phone], [Authorization], [Gender], [DateOfBirth], [Address], [JoinDate], [Status], [Email]) VALUES (N'U202608170001', N'獸醫管理', N'0911222333', 0, NULL, NULL, NULL, CAST(N'2026-08-17T20:45:30.0589637' AS DateTime2), 1, N'vetManage@test.com')
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_Appointments_InstitutionID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_Appointments_InstitutionID] ON [dbo].[Appointments]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_Appointments_PetID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_Appointments_PetID] ON [dbo].[Appointments]
(
	[PetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_AuthorizationRecords_InstitutionID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_AuthorizationRecords_InstitutionID] ON [dbo].[AuthorizationRecords]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_AuthorizationRecords_PetID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_AuthorizationRecords_PetID] ON [dbo].[AuthorizationRecords]
(
	[PetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_CareGroomingLogs_PetID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_CareGroomingLogs_PetID] ON [dbo].[CareGroomingLogs]
(
	[PetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_CareGroomingLogs_StaffID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_CareGroomingLogs_StaffID] ON [dbo].[CareGroomingLogs]
(
	[StaffID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_Employments_InstitutionID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_Employments_InstitutionID] ON [dbo].[Employments]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_Employments_OccupationID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_Employments_OccupationID] ON [dbo].[Employments]
(
	[OccupationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_Institutions_OwnerID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_Institutions_OwnerID] ON [dbo].[Institutions]
(
	[OwnerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_Institutions_StatusId]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_Institutions_StatusId] ON [dbo].[Institutions]
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_Institutions_TypeId]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_Institutions_TypeId] ON [dbo].[Institutions]
(
	[TypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_LogPhotos_LogID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_LogPhotos_LogID] ON [dbo].[LogPhotos]
(
	[LogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_MedicalRecords_PetID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_MedicalRecords_PetID] ON [dbo].[MedicalRecords]
(
	[PetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_MedicalRecords_VetID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_MedicalRecords_VetID] ON [dbo].[MedicalRecords]
(
	[VetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_Pets_OwnerID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_Pets_OwnerID] ON [dbo].[Pets]
(
	[OwnerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_PetWeight_PetID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_PetWeight_PetID] ON [dbo].[PetWeight]
(
	[PetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_Prescriptions_RecordID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_Prescriptions_RecordID] ON [dbo].[Prescriptions]
(
	[RecordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_RecordDiagnoses_RecordID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_RecordDiagnoses_RecordID] ON [dbo].[RecordDiagnoses]
(
	[RecordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_RecordDiagnoses_TermID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_RecordDiagnoses_TermID] ON [dbo].[RecordDiagnoses]
(
	[TermID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_TaskSchedules_InstitutionID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_TaskSchedules_InstitutionID] ON [dbo].[TaskSchedules]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_TaskSchedules_StaffID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_TaskSchedules_StaffID] ON [dbo].[TaskSchedules]
(
	[StaffID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** 物件:  Index [IX_UserCredentialDetails_CredentialID]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_UserCredentialDetails_CredentialID] ON [dbo].[UserCredentialDetails]
(
	[CredentialID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** 物件:  Index [IX_UserProfiles_Authorization]    指令碼日期: 2026/8/20 下午 10:20:19 ******/
CREATE NONCLUSTERED INDEX [IX_UserProfiles_Authorization] ON [dbo].[UserProfiles]
(
	[Authorization] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Employments] ADD  DEFAULT (N'') FOR [OccupationID]
GO
ALTER TABLE [dbo].[Institutions] ADD  DEFAULT (N'') FOR [StatusId]
GO
ALTER TABLE [dbo].[Institutions] ADD  DEFAULT (N'') FOR [TypeId]
GO
ALTER TABLE [dbo].[Institutions] ADD  DEFAULT (N'') FOR [OwnerID]
GO
ALTER TABLE [dbo].[LogPhotos] ADD  DEFAULT (getdate()) FOR [CreatedDate]
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
ALTER TABLE [dbo].[Institutions]  WITH CHECK ADD  CONSTRAINT [FK_Institutions_InstitutionStatuses_StatusId] FOREIGN KEY([StatusId])
REFERENCES [dbo].[InstitutionStatuses] ([StatusId])
GO
ALTER TABLE [dbo].[Institutions] CHECK CONSTRAINT [FK_Institutions_InstitutionStatuses_StatusId]
GO
ALTER TABLE [dbo].[Institutions]  WITH CHECK ADD  CONSTRAINT [FK_Institutions_InstitutionTypes_TypeId] FOREIGN KEY([TypeId])
REFERENCES [dbo].[InstitutionTypes] ([TypeId])
GO
ALTER TABLE [dbo].[Institutions] CHECK CONSTRAINT [FK_Institutions_InstitutionTypes_TypeId]
GO
ALTER TABLE [dbo].[Institutions]  WITH CHECK ADD  CONSTRAINT [FK_Institutions_UserProfiles_OwnerID] FOREIGN KEY([OwnerID])
REFERENCES [dbo].[UserProfiles] ([UserID])
GO
ALTER TABLE [dbo].[Institutions] CHECK CONSTRAINT [FK_Institutions_UserProfiles_OwnerID]
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
ALTER TABLE [dbo].[UserProfiles]  WITH CHECK ADD  CONSTRAINT [FK_UserProfiles_Role_Authorization] FOREIGN KEY([Authorization])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[UserProfiles] CHECK CONSTRAINT [FK_UserProfiles_Role_Authorization]
GO
USE [master]
GO
ALTER DATABASE [PetCare] SET  READ_WRITE 
GO
