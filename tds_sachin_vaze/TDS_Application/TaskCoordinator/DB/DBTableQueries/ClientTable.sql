CREATE TABLE [dbo].[Client] (
    [id]                 INT          NOT NULL,
    [clientIpAddress]    VARCHAR (50) NOT NULL,
    [clientPort]         NCHAR (10)   NOT NULL,
    [noOfTasksGenerated] INT          DEFAULT ((0)) NULL,
    [HostName]           VARCHAR (50) NOT NULL,
    [ClientGuid]         VARCHAR (50) NULL,
    CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [UC_Client] UNIQUE NONCLUSTERED ([clientIpAddress] ASC, [clientPort] ASC)
);

