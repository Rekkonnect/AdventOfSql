WITH
workshops_with_timezone_name AS (
    SELECT
        Workshops.*,
        [utc_offset] = [timezones].identifiers.offset
    FROM Workshops
    LEFT OUTER JOIN [timezones].identifiers
        ON Workshops.timezone = [timezones].identifiers.[name]
),
workshops_business_times AS (
    SELECT
        workshop_id,
        business_start_time_utc
            = TODATETIMEOFFSET(
				DATEADD(DAY, 2,
					CONVERT(DATETIMEOFFSET, business_start_time)),
				[utc_offset])
			AT TIME ZONE 'UTC',
        business_end_time_utc
            = TODATETIMEOFFSET(
				DATEADD(DAY, 2,
					CONVERT(DATETIMEOFFSET, business_end_time)),
				[utc_offset])
			AT TIME ZONE 'UTC'
    FROM workshops_with_timezone_name
),
workshops_start_times AS (
    SELECT business_start_time_utc AS start_time_utc
    FROM workshops_business_times
    GROUP BY business_start_time_utc
),
meeting_times_compatibility AS (
    SELECT
		outer_times.workshop_id,
		outer_times.business_start_time_utc,
		outer_times.business_end_time_utc,
        start_time_utc,
        suitable = (
            SELECT
                suitable =
					CASE WHEN (
						business_start_time_utc <= start_time_utc
							AND business_end_time_utc >= DATEADD(HOUR, 1, start_time_utc)
					)
					OR (
						DATEADD(DAY, 1, business_start_time_utc) <= start_time_utc
							AND DATEADD(DAY, 1, business_end_time_utc) >= DATEADD(HOUR, 1, start_time_utc)
					)
					OR (
						DATEADD(DAY, -1, business_start_time_utc) <= start_time_utc
							AND DATEADD(DAY, -1, business_end_time_utc) >= DATEADD(HOUR, 1, start_time_utc)
					)
					THEN 1
					ELSE 0
					END
            FROM workshops_business_times inner_times
			WHERE outer_times.workshop_id = inner_times.workshop_id
        )
    FROM workshops_start_times,
		workshops_business_times outer_times
),
grouped_meeting_times_compatibility AS (
    SELECT
        start_time_utc,
        suitable = MIN(suitable)
    FROM meeting_times_compatibility
	GROUP BY start_time_utc
),
possible_meeting_times AS (
    SELECT
        start_time_utc = CAST(CONVERT(DATETIME, start_time_utc) AS TIME)
    FROM grouped_meeting_times_compatibility
    WHERE suitable = 1
)
SELECT TOP 1
	start_time_utc
FROM possible_meeting_times
ORDER BY start_time_utc
;
