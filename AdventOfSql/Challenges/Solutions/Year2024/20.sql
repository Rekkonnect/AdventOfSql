WITH
query_strings AS (
    SELECT
        [request_id],
        [query_string] =
            CASE CHARINDEX('?', [url])
            WHEN 0 THEN ''
            ELSE SUBSTRING([url], CHARINDEX('?', [url]) + 1, LEN([url]))
            END
    FROM web_requests
),
query_parameters AS (
    SELECT
        [request_id],
        [key] =
            SUBSTRING(value, 1, CHARINDEX('=', value) - 1),
        [value] =
            SUBSTRING(value, CHARINDEX('=', value) + 1, LEN(value))
    FROM query_strings
    CROSS APPLY STRING_SPLIT([query_string], '&')
	WHERE [query_string] != ''
),
requests_sourcing_from_aosql AS (
    SELECT
        DISTINCT [request_id]
    FROM query_parameters
    WHERE [key] = 'utm_source'
        AND [value] = 'advent-of-sql'
),
request_parameter_count AS (
    SELECT
        [request_id],
        param_count = COUNT(*)
    FROM query_parameters
    GROUP BY [request_id]
),
-- It is nowhere specified that we must sort by descending DISTINCT parameter key count
-- This is probably a hidden requirement driven by an unintended bug in the server implementation
distinct_request_parameter_count AS (
    SELECT
        request_parameter_count.[request_id],
        request_parameter_count.param_count,
		distinct_parameter_count = COUNT(DISTINCT [key])
    FROM request_parameter_count
	INNER JOIN query_parameters
		ON request_parameter_count.[request_id]
            = query_parameters.[request_id]
    GROUP BY request_parameter_count.[request_id],
		request_parameter_count.param_count
),
requests_sourcing_from_aosql_parameter_count AS (
    SELECT
        web_requests.[request_id],
        web_requests.[url],
        param_count,
		distinct_parameter_count
    FROM distinct_request_parameter_count
    LEFT JOIN requests_sourcing_from_aosql
        ON distinct_request_parameter_count.[request_id]
            = requests_sourcing_from_aosql.[request_id]
    LEFT JOIN web_requests
        ON distinct_request_parameter_count.[request_id]
            = web_requests.[request_id]
)
SELECT TOP 1
    [url]
FROM requests_sourcing_from_aosql_parameter_count
ORDER BY
	distinct_parameter_count DESC,
	[url]
;
