/****** Object:  Database [managementsystem_db]    Script Date: 2024-11-27 1:38:40 PM ******/
CREATE DATABASE [managementsystem_db]  (EDITION = 'GeneralPurpose', SERVICE_OBJECTIVE = 'GP_S_Gen5_1', MAXSIZE = 32 GB) WITH CATALOG_COLLATION = SQL_Latin1_General_CP1_CI_AS, LEDGER = OFF;
GO
ALTER DATABASE [managementsystem_db] SET COMPATIBILITY_LEVEL = 160
GO
ALTER DATABASE [managementsystem_db] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [managementsystem_db] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [managementsystem_db] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [managementsystem_db] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [managementsystem_db] SET ARITHABORT OFF 
GO
ALTER DATABASE [managementsystem_db] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [managementsystem_db] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [managementsystem_db] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [managementsystem_db] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [managementsystem_db] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [managementsystem_db] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [managementsystem_db] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [managementsystem_db] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [managementsystem_db] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO
ALTER DATABASE [managementsystem_db] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [managementsystem_db] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [managementsystem_db] SET  MULTI_USER 
GO
ALTER DATABASE [managementsystem_db] SET ENCRYPTION ON
GO
ALTER DATABASE [managementsystem_db] SET QUERY_STORE = ON
GO
ALTER DATABASE [managementsystem_db] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 100, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
/*** The scripts of database scoped configurations in Azure should be executed inside the target database connection. ***/
GO
-- ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 8;
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetTierByPoints]    Script Date: 2024-11-27 1:38:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_GetTierByPoints] (@LoyaltyPoints INT)
RETURNS VARCHAR(50)
AS
BEGIN
    DECLARE @Tier VARCHAR(50);

    -- Adjusted thresholds for Silver, Gold, Platinum tiers
    IF @LoyaltyPoints < 200
        SET @Tier = 'Silver';
    ELSE IF @LoyaltyPoints >= 200 AND @LoyaltyPoints < 400
        SET @Tier = 'Gold';
    ELSE
        SET @Tier = 'Platinum';

    RETURN @Tier;
END;
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 2024-11-27 1:38:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[CustomerID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[LoyaltyPoints] [int] NULL,
	[Tier] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LoyaltyProgram]    Script Date: 2024-11-27 1:38:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoyaltyProgram](
	[ProgramID] [int] IDENTITY(1,1) NOT NULL,
	[ProgramName] [varchar](100) NOT NULL,
	[Description] [varchar](255) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Tier] [varchar](50) NOT NULL,
	[Points] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ProgramID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Report]    Script Date: 2024-11-27 1:38:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Report](
	[ReportID] [int] IDENTITY(1,1) NOT NULL,
	[ReportName] [varchar](100) NOT NULL,
	[Description] [varchar](255) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ReportData] [nvarchar](max) NULL,
	[ReportType] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ReportID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionLoyalty]    Script Date: 2024-11-27 1:38:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionLoyalty](
	[TransactionLoyaltyID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NULL,
	[ProgramID] [int] NULL,
	[Date] [datetime] NULL,
	[Amount] [decimal](10, 2) NOT NULL,
	[PointsEarned] [int] NULL,
	[PointsRedeemed] [int] NULL,
	[TransactionType] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TransactionLoyaltyID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 2024-11-27 1:38:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[PasswordHashed] [varchar](255) NOT NULL,
	[DateJoined] [datetime] NULL,
	[Role] [varchar](50) NOT NULL,
	[VerificationCode] [varchar](10) NULL,
	[IsVerified] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Customer] ADD  DEFAULT ((0)) FOR [LoyaltyPoints]
GO
ALTER TABLE [dbo].[Customer] ADD  DEFAULT ('Silver') FOR [Tier]
GO
ALTER TABLE [dbo].[Report] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[TransactionLoyalty] ADD  DEFAULT (getdate()) FOR [Date]
GO
ALTER TABLE [dbo].[TransactionLoyalty] ADD  DEFAULT ((0)) FOR [PointsEarned]
GO
ALTER TABLE [dbo].[TransactionLoyalty] ADD  DEFAULT ((0)) FOR [PointsRedeemed]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT (getdate()) FOR [DateJoined]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_Role]  DEFAULT ('Customer') FOR [Role]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT ((0)) FOR [IsVerified]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_User]
GO
ALTER TABLE [dbo].[Report]  WITH CHECK ADD  CONSTRAINT [FK_Report_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[Report] CHECK CONSTRAINT [FK_Report_CreatedBy]
GO
ALTER TABLE [dbo].[TransactionLoyalty]  WITH CHECK ADD  CONSTRAINT [FK_TransactionLoyalty_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customer] ([CustomerID])
GO
ALTER TABLE [dbo].[TransactionLoyalty] CHECK CONSTRAINT [FK_TransactionLoyalty_Customer]
GO
ALTER TABLE [dbo].[TransactionLoyalty]  WITH CHECK ADD  CONSTRAINT [FK_TransactionLoyalty_Program] FOREIGN KEY([ProgramID])
REFERENCES [dbo].[LoyaltyProgram] ([ProgramID])
GO
ALTER TABLE [dbo].[TransactionLoyalty] CHECK CONSTRAINT [FK_TransactionLoyalty_Program]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD CHECK  (([Tier]='Platinum' OR [Tier]='Gold' OR [Tier]='Silver'))
GO
ALTER TABLE [dbo].[LoyaltyProgram]  WITH CHECK ADD  CONSTRAINT [CK__LoyaltyPr__Tier__3C34F16F] CHECK  (([Tier]='Silver' OR [Tier]='Gold' OR [Tier]='Platinum'))
GO
ALTER TABLE [dbo].[LoyaltyProgram] CHECK CONSTRAINT [CK__LoyaltyPr__Tier__3C34F16F]
GO
ALTER TABLE [dbo].[Report]  WITH CHECK ADD CHECK  (([ReportType]='Customer' OR [ReportType]='Admin'))
GO
ALTER TABLE [dbo].[TransactionLoyalty]  WITH CHECK ADD  CONSTRAINT [CK_TransactionLoyalty_TransactionType] CHECK  (([TransactionType]='Redemption' OR [TransactionType]='Program' OR [TransactionType]='Purchase'))
GO
ALTER TABLE [dbo].[TransactionLoyalty] CHECK CONSTRAINT [CK_TransactionLoyalty_TransactionType]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD CHECK  (([Role]='Customer' OR [Role]='Admin'))
GO
ALTER DATABASE [managementsystem_db] SET  READ_WRITE 
GO
