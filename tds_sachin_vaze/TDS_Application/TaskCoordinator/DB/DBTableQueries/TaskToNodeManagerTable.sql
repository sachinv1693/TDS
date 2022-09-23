CREATE TABLE [dbo].[TaskToNodeManager] (
    [id]     INT NOT NULL,
    [taskId] INT NOT NULL,
    [nodeId] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_TaskToNodeManager_Task] FOREIGN KEY ([taskId]) REFERENCES [dbo].[Task] ([id]),
    CONSTRAINT [FK_TaskToNodeManager_Node] FOREIGN KEY ([nodeId]) REFERENCES [dbo].[Node] ([Id])
);
