DECLARE @result VARCHAR(MAX);

WITH
all_letters AS
(
    SELECT * FROM letters_a
    UNION
    SELECT * FROM letters_b
),
all_chars AS
(
    SELECT
        id,
        CHAR([value]) AS letter
    FROM all_letters
),
valid_letters AS
(
    SELECT id, letter
    FROM all_chars
    WHERE
        ([letter] >= 'a' AND [letter] <= 'z')
        OR ([letter] >= 'A' AND [letter] <= 'Z')
        OR ([letter] IN (' ', '!', '"', '''', '(', ')', ',', '-', '.', ':', ';', '?'))
)
SELECT @result = COALESCE(@result, '') + letter
FROM valid_letters
ORDER BY id;

SELECT @result AS message;
