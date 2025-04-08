WITH
gift_request_counts AS (
    SELECT
        gift_name = gifts.gift_name,
        request_count = COUNT(*)
    FROM gifts
        INNER JOIN gift_requests
            ON gift_requests.gift_id = gifts.gift_id
    GROUP BY gifts.gift_name
),
gift_request_percentiles AS (
    SELECT
        gift_name,
		request_count,
        percentile = PERCENT_RANK() OVER (
            ORDER BY request_count)
    FROM gift_request_counts
),
first_gift_request_percentiles AS (
    SELECT
        gift_name,
		request_count,
        percentile,
        row_number = ROW_NUMBER() OVER (PARTITION BY percentile ORDER BY gift_name)
    FROM gift_request_percentiles
)
SELECT
    gift_name,
    percentile = ROUND(percentile, 2)
FROM first_gift_request_percentiles
WHERE row_number = 1
ORDER BY percentile DESC
OFFSET 1 ROW
FETCH NEXT 1 ROW ONLY
;
