/*
Notes on working out this problem's solution
Schema:
- TIMESTAMP had to be replaced with DATETIMEOFFSET
- GEOGRAPHY(...) had to be replaced with GEOGRAPHY

Input:
- The input needed its time offsets in the form "HH:MM", while it was provided as "HH"
- The GEOGRAPHY::Point and GEOGRAPHY::STPolyFromText methods had to be used instead
- The arguments to GEOGRAPHY::Point are (Lat, Long, SRID) instead of (Long, Lat) in ST_Point,
  so they had to be switched
*/

WITH
location_intervals AS (
    SELECT
        *,
        enter_time = [timestamp],
        leave_time = LEAD([timestamp]) OVER (ORDER BY [timestamp])
    FROM sleigh_locations
),
location_stays AS (
    SELECT
        *,
        duration_seconds = DATEDIFF(SECOND, enter_time, leave_time)
    FROM location_intervals
    WHERE leave_time IS NOT NULL
),
place_name_stays AS (
    SELECT
        place_name,
        duration_seconds
    FROM areas
        INNER JOIN location_stays
            ON areas.polygon.STContains(coordinate) = 1
),
grouped_place_stays AS (
    SELECT
        place_name,
        total_duration_seconds = SUM(duration_seconds)
    FROM place_name_stays
    GROUP BY place_name
)
SELECT TOP 1
    place_name,
    total_hours_spent = total_duration_seconds / 60 / 60
FROM grouped_place_stays
ORDER BY total_duration_seconds DESC
;
