CREATE OR ALTER FUNCTION GetGuestRootNode(@root XML)
RETURNS XML
WITH EXECUTE AS CALLER
AS
BEGIN

	DECLARE @filteredRoot XML = @root.query(
		'(//*[contains(fn:local-name(), ''guest'') or contains(fn:local-name(), ''parti'')])[1]');
	RETURN @filteredRoot;

END
;

GO

CREATE OR ALTER FUNCTION GetGuestCount(@root XML)
RETURNS INTEGER
WITH EXECUTE AS CALLER
AS
BEGIN
	DECLARE @result INTEGER;
	SET @result = (
		SELECT T.c.value('.', 'int') AS [value]
		FROM @root.nodes('./*/*[text()]') T(c)
		WHERE ISNUMERIC(T.c.value('.', 'nvarchar(max)')) = 1
	);
	RETURN @result;
END
;

GO

CREATE OR ALTER FUNCTION GetFoodItemIds(@root XML)
RETURNS @returnTable TABLE(id INTEGER)
AS
BEGIN
	INSERT INTO @returnTable
		SELECT T.c.value('.', 'int') AS Result
		FROM @root.nodes('//food_item_id') T(c);
	RETURN;
END
;

GO

WITH
xml_parsed_menus AS (
	SELECT
		id,
		CONVERT(XML,
			-- We store the characters in NVARCHAR, so we must
			-- force the parser to recognize the UTF-16 form of our text
			-- Also use STUFF for better performance and to ensure
			-- we only change the encoding that is found in the header
			-- https://stackoverflow.com/a/38911646
			COALESCE(
				STUFF(
					menu_data,
					CHARINDEX('UTF-8', menu_data),
					LEN('UTF-8'),
					'UTF-16'),
				menu_data),
			2) AS menu
	FROM christmas_menus
),
successful_dinners AS (
	SELECT *
	FROM xml_parsed_menus
	WHERE dbo.GetGuestCount(
		dbo.GetGuestRootNode(menu)) > 78
),
appearing_food_ids AS (
	SELECT
		food_items.id AS food_item_id,
		COUNT(*) AS frequency
	FROM successful_dinners
	CROSS APPLY dbo.GetFoodItemIds(menu)
		AS food_items
	GROUP BY food_items.id
)
SELECT TOP 1
	food_item_id,
	frequency
FROM appearing_food_ids
ORDER BY frequency DESC
;
