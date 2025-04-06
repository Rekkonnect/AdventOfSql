WITH
production_difference AS (
    SELECT
        today.production_date,
        today.toys_produced,
        yesterday.toys_produced AS previous_day_production
    FROM toy_production today
    INNER JOIN toy_production yesterday
        ON DATEADD(DAY, 1, yesterday.production_date) = today.production_date
),
difference_analysis AS (
    SELECT
        production_date,
        toys_produced,
        previous_day_production,
        production_change = (toys_produced - previous_day_production),
        production_change_percentage = CAST(toys_produced - previous_day_production AS DECIMAL) / previous_day_production * 100
    FROM production_difference
)
SELECT TOP 1 *
FROM difference_analysis
ORDER BY production_change_percentage DESC;
