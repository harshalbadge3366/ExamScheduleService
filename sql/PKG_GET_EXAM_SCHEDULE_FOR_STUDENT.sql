SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.ACD_EXAM_SCHEDULE', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ACD_EXAM_SCHEDULE
    (
        EXAM_ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ACD_EXAM_SCHEDULE PRIMARY KEY,
        STUDENT_ID INT NOT NULL,
        ACD_SESS_ID INT NOT NULL,
        SUBJECT_CODE NVARCHAR(30) NOT NULL,
        SUBJECT_NAME NVARCHAR(150) NOT NULL,
        EXAM_DATE DATE NOT NULL,
        START_TIME TIME(0) NOT NULL,
        END_TIME TIME(0) NOT NULL,
        ROOM_NO NVARCHAR(30) NOT NULL,
        EXAM_TYPE NVARCHAR(50) NOT NULL,
        CREATED_ON DATETIME2(0) NOT NULL CONSTRAINT DF_ACD_EXAM_SCHEDULE_CREATED_ON DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_EXAM_SCHEDULE'
      AND object_id = OBJECT_ID(N'dbo.ACD_EXAM_SCHEDULE')
)
BEGIN
    CREATE INDEX IX_EXAM_SCHEDULE
        ON dbo.ACD_EXAM_SCHEDULE (STUDENT_ID, ACD_SESS_ID, EXAM_DATE, START_TIME)
        INCLUDE (SUBJECT_CODE, SUBJECT_NAME, END_TIME, ROOM_NO, EXAM_TYPE);
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.ACD_EXAM_SCHEDULE WHERE STUDENT_ID = 1001 AND ACD_SESS_ID = 1)
BEGIN
    INSERT INTO dbo.ACD_EXAM_SCHEDULE
    (
        STUDENT_ID,
        ACD_SESS_ID,
        SUBJECT_CODE,
        SUBJECT_NAME,
        EXAM_DATE,
        START_TIME,
        END_TIME,
        ROOM_NO,
        EXAM_TYPE
    )
    VALUES
        (1001, 1, N'CS101', N'Computer Science Fundamentals', DATEADD(DAY, 3, CONVERT(DATE, GETDATE())), '09:00', '12:00', N'A-101', N'Theory'),
        (1001, 1, N'MA101', N'Engineering Mathematics', DATEADD(DAY, 5, CONVERT(DATE, GETDATE())), '10:00', '13:00', N'B-204', N'Theory'),
        (1001, 1, N'PH101', N'Applied Physics', DATEADD(DAY, -2, CONVERT(DATE, GETDATE())), '14:00', '16:00', N'C-302', N'Practical'),
        (1002, 1, N'CS101', N'Computer Science Fundamentals', DATEADD(DAY, 4, CONVERT(DATE, GETDATE())), '09:00', '12:00', N'A-102', N'Theory');
END;
GO

CREATE OR ALTER PROCEDURE dbo.PKG_GET_EXAM_SCHEDULE_FOR_STUDENT
(
    @P_STUDENT_ID INT,
    @P_ACD_SESS_ID INT,
    @P_SUBJECT_SEARCH NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @P_SUBJECT_SEARCH IS NULL
    BEGIN
        SELECT
            EXAM_ID AS ExamId,
            STUDENT_ID AS StudentId,
            ACD_SESS_ID AS AcdSessId,
            SUBJECT_CODE AS SubjectCode,
            SUBJECT_NAME AS SubjectName,
            EXAM_DATE AS ExamDate,
            START_TIME AS StartTime,
            END_TIME AS EndTime,
            ROOM_NO AS RoomNo,
            EXAM_TYPE AS ExamType
        FROM dbo.ACD_EXAM_SCHEDULE
        WHERE STUDENT_ID = @P_STUDENT_ID
          AND ACD_SESS_ID = @P_ACD_SESS_ID
        ORDER BY
            EXAM_DATE ASC,
            START_TIME ASC;
    END
    ELSE
    BEGIN
        DECLARE @SearchText NVARCHAR(104) = N'%' + @P_SUBJECT_SEARCH + N'%';

        SELECT
            EXAM_ID AS ExamId,
            STUDENT_ID AS StudentId,
            ACD_SESS_ID AS AcdSessId,
            SUBJECT_CODE AS SubjectCode,
            SUBJECT_NAME AS SubjectName,
            EXAM_DATE AS ExamDate,
            START_TIME AS StartTime,
            END_TIME AS EndTime,
            ROOM_NO AS RoomNo,
            EXAM_TYPE AS ExamType
        FROM dbo.ACD_EXAM_SCHEDULE
        WHERE STUDENT_ID = @P_STUDENT_ID
          AND ACD_SESS_ID = @P_ACD_SESS_ID
          AND
          (
              SUBJECT_CODE LIKE @SearchText
              OR SUBJECT_NAME LIKE @SearchText
          )
        ORDER BY
            EXAM_DATE ASC,
            START_TIME ASC;
    END
END;
GO
