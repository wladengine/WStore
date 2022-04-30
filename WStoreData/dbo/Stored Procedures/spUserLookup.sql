CREATE PROCEDURE [dbo].[spUserLookup]
	@Id nvarchar(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT U.Id, U.FirstName, U.[LastName], U.[Email], U.CreatedDate
	FROM dbo.[User] U
	WHERE U.Id = @Id
END