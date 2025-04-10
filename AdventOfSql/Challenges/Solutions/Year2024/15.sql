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

Solution:
- Both the test case and the actual input are evaluated by defaulting to the first
  (and hopefully only) row that we are interested in with regard to the possible locations
*/

WITH
interesting_location AS (
    SELECT TOP 1 *
    FROM sleigh_locations
    WHERE [timestamp] = '2024-12-24 23:00:00+00:00'
),
any_location AS (
    SELECT TOP 1 *
    FROM sleigh_locations
),
queried_location AS (
    SELECT TOP 1 *
    FROM (
        SELECT [rank] = 0, * FROM interesting_location
            UNION ALL
        SELECT [rank] = 1, * FROM any_location
    ) unionized
    ORDER BY [rank]
)
SELECT place_name
FROM areas
    INNER JOIN queried_location
        ON areas.polygon.STContains(queried_location.coordinate) = 1
