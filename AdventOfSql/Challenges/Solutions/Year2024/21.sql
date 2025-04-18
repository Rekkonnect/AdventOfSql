WITH
quarter_sales AS (
    SELECT
        sales.*,
        [year] = DATEPART(YEAR, sale_date),
        [quarter] =
            CASE
            WHEN DATEPART(MONTH, sale_date) BETWEEN 1 AND 3
                THEN 1
            WHEN DATEPART(MONTH, sale_date) BETWEEN 4 AND 6
                THEN 2
            WHEN DATEPART(MONTH, sale_date) BETWEEN 7 AND 9
                THEN 3
            WHEN DATEPART(MONTH, sale_date) BETWEEN 10 AND 12
                THEN 4
            END
    FROM sales
),
total_quarter_sales AS (
    SELECT
        [year],
        [quarter],
        total_sales = SUM(amount)
    FROM quarter_sales
    GROUP BY [year], [quarter]
),
total_quarter_sales_with_previous AS (
    SELECT
        [year],
        [quarter],
        total_sales = total_sales,
        previous_sales = LAG(total_sales, 1) OVER (ORDER BY [year], [quarter])
    FROM total_quarter_sales
),
total_quarter_sales_growth_rates AS (
    SELECT
        [year],
        [quarter],
        total_sales = total_sales,
        growth_rate = (total_sales - previous_sales) / previous_sales
    FROM total_quarter_sales_with_previous
)
SELECT TOP 1
    year_quarter = CONVERT(NVARCHAR(MAX), [year]) + ',' + CONVERT(NVARCHAR(MAX), [quarter])
FROM total_quarter_sales_growth_rates
ORDER BY growth_rate DESC
;