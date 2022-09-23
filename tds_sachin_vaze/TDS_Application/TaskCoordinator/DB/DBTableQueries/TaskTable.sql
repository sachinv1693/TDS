CREATE TABLE [dbo].[Task] (
    [id]         INT            IDENTITY (1, 1) NOT NULL,
    [clientId]   INT            NOT NULL,
    [taskPath]   VARCHAR (100)  NOT NULL,
    [taskStatus] INT            DEFAULT ((0)) NULL,
    [taskResult] NVARCHAR (MAX) DEFAULT (NULL) NULL,
    [isSuccess]  BIT            DEFAULT ((0)) NULL,
    [taskGuid]   VARCHAR(50)     NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Task_Client] FOREIGN KEY ([clientId]) REFERENCES [dbo].[Client] ([id])
);


GO

CREATE TRIGGER [dbo].[Trigger_Client]
    ON [dbo].[Task]
    AFTER INSERT
    AS
    BEGIN
		SET NOCOUNT ON;
		UPDATE [dbo].[Client]
        SET NoOfTasksGenerated = NoOfTasksGenerated + 1
		FROM [dbo].[Client]
		INNER JOIN [dbo].[Task]
		ON [dbo].[Client].Id = [dbo].[Task].[ClientId]
    END