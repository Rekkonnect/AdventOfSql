WITH
max_experienced_elves_rows AS (
    SELECT
        ROW_NUMBER() OVER (
            PARTITION BY primary_skill
            ORDER BY years_experience DESC, elf_id) AS row_number,
        workshop_elves.*
    FROM workshop_elves
),
max_experienced_elves AS (
    SELECT
        elf_id,
        primary_skill
    FROM max_experienced_elves_rows
    WHERE row_number = 1
),
min_experienced_elves_rows AS (
    SELECT
        ROW_NUMBER() OVER (
            PARTITION BY primary_skill
            ORDER BY years_experience, elf_id) AS row_number,
        workshop_elves.*
    FROM workshop_elves
),
min_experienced_elves AS (
    SELECT
        elf_id,
        primary_skill
    FROM min_experienced_elves_rows
    WHERE row_number = 1
),
pairs_by_primary_skill AS (
    SELECT
        max_experienced_elves.elf_id AS max_elf_id,
        min_experienced_elves.elf_id AS min_elf_id,
        min_experienced_elves.primary_skill,
        [max_years_experience_elf_id,min_years_experience_elf_id,shared_skill]
            = CAST(max_experienced_elves.elf_id AS VARCHAR(MAX)) + ','
            + CAST(min_experienced_elves.elf_id AS VARCHAR(MAX)) + ','
            + CAST(min_experienced_elves.primary_skill AS VARCHAR(MAX))
    FROM min_experienced_elves
        INNER JOIN max_experienced_elves
            ON min_experienced_elves.primary_skill = max_experienced_elves.primary_skill
    WHERE min_experienced_elves.elf_id != max_experienced_elves.elf_id
)
SELECT TOP 3 *
FROM pairs_by_primary_skill
ORDER BY primary_skill ASC
;
