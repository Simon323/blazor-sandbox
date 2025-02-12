## Insert exaplme data
```sql
DECLARE @total INT = 50;
DECLARE @i INT = 1;
DECLARE @LinkId INT = 30002;
DECLARE @CurrentDate DATETIME2 = GETDATE();
DECLARE @RandomDays INT;
DECLARE @RandomDate DATETIME2;

WHILE @i <= @total
BEGIN
    SET @RandomDays = CAST(RAND() * 10 AS INT);
    SET @RandomDate = DATEADD(DAY, -@RandomDays, @CurrentDate);
    INSERT INTO LinkAnalytics(LinkId, CreatedAt)
    VALUES (@LinkId, @RandomDate);

    SET @i = @i + 1;
END;
```