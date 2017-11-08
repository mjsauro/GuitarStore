CREATE TABLE [dbo].[CourseStudent] (
    [StudentID]  INT            NOT NULL,
    [CourseName] NVARCHAR (100) NOT NULL,
    [Grade]      NVARCHAR (1)   NULL,
    CONSTRAINT [PK_CourseStudent] PRIMARY KEY CLUSTERED ([StudentID] ASC, [CourseName] ASC),
    CONSTRAINT [FK_CourseStudent_Course] FOREIGN KEY ([CourseName]) REFERENCES [dbo].[Course] ([Name]),
    CONSTRAINT [FK_CourseStudent_Student] FOREIGN KEY ([StudentID]) REFERENCES [dbo].[Student] ([ID])
);

