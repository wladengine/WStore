CREATE PROCEDURE [dbo].[spSale_SaleReport]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		[s].[SaleDate], 
		[s].[SubTotal], 
		[s].[Tax], 
		[s].[Total], 
		u.LastName, 
		u.FirstName, 
		u.Email
	FROM dbo.Sale s
	INNER JOIN dbo.[User] u on s.CashierId = u.Id
END
