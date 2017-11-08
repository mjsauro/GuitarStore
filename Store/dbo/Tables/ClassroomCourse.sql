CREATE TABLE [dbo].[ClassroomCourse] (
    [ClassroomRoomNumber] NVARCHAR (20)  NOT NULL,
    [CourseName]          NVARCHAR (100) NOT NULL,
    [DayOfWeek]           NVARCHAR (1)   NULL,
    [Time]                TIME (2)       NULL,
    CONSTRAINT [PK_ClassroomCourse] PRIMARY KEY CLUSTERED ([ClassroomRoomNumber] ASC, [CourseName] ASC),
    CONSTRAINT [FK_ClassroomCourse_Classroom] FOREIGN KEY ([ClassroomRoomNumber]) REFERENCES [dbo].[Classroom] ([RoomNumber]),
    CONSTRAINT [FK_ClassroomCourse_Course] FOREIGN KEY ([CourseName]) REFERENCES [dbo].[Course] ([Name])
);

