WITH
number_differences AS (
    SELECT
        id = sequence_table.id,
        previous_id = LAG(sequence_table.id, 1)
            OVER (ORDER BY sequence_table.id)
    FROM sequence_table
),
missing_numbers AS (
    SELECT
        gap_start = previous_id + 1,
        gap_end = id - 1,
        gap_index = ROW_NUMBER() OVER (ORDER BY id)
    FROM number_differences
    WHERE previous_id IS NOT NULL
        AND (id - previous_id) > 1
),
missing_number_gap_generator AS (
    SELECT
        gap_start,
        number_instance = gap_start,
        gap_end,
        gap_index
    FROM missing_numbers
    WHERE gap_start <= gap_end

    UNION ALL

    SELECT
        gap_start,
        number_instance = number_instance + 1,
        gap_end,
        gap_index
    FROM missing_number_gap_generator
    WHERE number_instance < gap_end
),
missing_numbers_list AS (
    SELECT
        missing.*
    FROM missing_number_gap_generator missing
),
missing_numbers_list_string AS (
    SELECT
		gap_index,
        missing_numbers = STRING_AGG(
            CONVERT(NVARCHAR(MAX), number_instance), ',')
			WITHIN GROUP (ORDER BY number_instance ASC)
    FROM missing_numbers_list missing
	GROUP BY gap_index
)
SELECT missing_numbers
FROM missing_numbers_list_string
ORDER BY gap_index
;
