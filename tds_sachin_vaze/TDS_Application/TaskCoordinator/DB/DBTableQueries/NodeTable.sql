CREATE TABLE [dbo].[Node] (
    [Id]            INT          IDENTITY (1, 1) NOT NULL,
    [nodeIpAddress] VARCHAR (50) NOT NULL,
    [nodePort]      NCHAR (10)   NOT NULL,
    [nodeStatus]    INT          DEFAULT ((0)) NULL,
    [HostName]      VARCHAR (50) NOT NULL,
    [NodeGuid]      VARCHAR(50)   NULL,
    [NodeExecutorType]  INT DEFAULT ((0)) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_Node] UNIQUE NONCLUSTERED ([nodeIpAddress] ASC, [nodePort] ASC)
);
