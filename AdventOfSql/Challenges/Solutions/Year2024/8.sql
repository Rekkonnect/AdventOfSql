WITH
nesting_levels AS (
    SELECT
        staff.*,
        1 AS level
    FROM staff
    WHERE manager_id IS NULL

    UNION ALL

    SELECT
        staff.*,
        level = nesting_levels.level + 1
    FROM nesting_levels
        INNER JOIN staff
            ON staff.manager_id = nesting_levels.staff_id
)
SELECT level = MAX(level)
FROM nesting_levels
;
