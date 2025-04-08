WITH
harvests_season_id AS (
    SELECT
        field_name,
        harvest_year,
        season_id = harvest_year * 4
           + CASE season
                WHEN 'Spring' THEN 0
                WHEN 'Summer' THEN 1
                WHEN 'Fall' THEN 2
                WHEN 'Winter' THEN 3
                END,
        trees_harvested
    FROM TreeHarvests
),
harvest_slices AS (
    SELECT
        middle.field_name,
        middle.season_id,
        three_season_moving_avg = CAST(
            [left].trees_harvested
                + middle.trees_harvested
                + [right].trees_harvested
              AS FLOAT
            ) / 3
    FROM harvests_season_id middle
        INNER JOIN harvests_season_id [left]
            ON [left].season_id = (middle.season_id - 1)
            AND [left].field_name = middle.field_name
        INNER JOIN harvests_season_id [right]
            ON [right].season_id = (middle.season_id + 1)
            AND [right].field_name = middle.field_name
)
SELECT TOP 1
    three_season_moving_avg = ROUND(three_season_moving_avg, 2)
FROM harvest_slices
ORDER BY three_season_moving_avg DESC
;
