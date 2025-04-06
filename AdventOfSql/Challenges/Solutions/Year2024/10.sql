WITH
drinks_per_day AS (
    SELECT
        total_quantity = SUM(quantity),
        drink_name,
        [date]
    FROM drinks
    GROUP BY drink_name, date
)
SELECT
    [date]
FROM drinks_per_day
PIVOT (
    SUM(total_quantity)
    FOR drink_name IN (
        [Hot Cocoa],
        [Peppermint Schnapps],
        [Eggnog]
    )
) AS SD
WHERE [Hot Cocoa] = 38
    AND [Peppermint Schnapps] = 298
    AND [Eggnog] = 198
