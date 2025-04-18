WITH
elf_skills AS (
    SELECT
        elves.*,
        skill = value
    FROM elves
    CROSS APPLY STRING_SPLIT(elves.skills, ',')
),
elf_sql_skills AS (
    SELECT *
    FROM elf_skills
    WHERE skill = 'SQL'
)
SELECT
    numofelveswithsql = COUNT(DISTINCT elf_sql_skills.id)
FROM elf_sql_skills
;
