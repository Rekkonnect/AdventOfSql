SELECT TOP 5
    children.[name],
    JSON_VALUE(wish_lists.wishes, '$.first_choice')
        AS primary_wish,
    JSON_VALUE(wish_lists.wishes, '$.second_choice')
        AS backup_wish,
    JSON_VALUE(wish_lists.wishes, '$.colors[0]')
        AS favorite_color,
    (SELECT COUNT(*) FROM OPENJSON(wish_lists.wishes, '$.colors'))
        AS color_count,
    CASE
        WHEN toy_catalogue.difficulty_to_make = 1 THEN 'Simple Gift'
        WHEN toy_catalogue.difficulty_to_make = 2 THEN 'Moderate Gift'
        ELSE 'Complex Gift' END
        AS gift_complexity,
    CASE
        WHEN toy_catalogue.category = 'outdoor'
            THEN 'Outside Workshop'
        WHEN toy_catalogue.category = 'educational'
            THEN 'Learning Workshop'
        ELSE 'General Workshop' END
        AS workshop_assignment
FROM children
INNER JOIN wish_lists
    ON children.child_id = wish_lists.child_id
INNER JOIN toy_catalogue ON 1 = 1
WHERE JSON_VALUE(wish_lists.wishes, '$.first_choice')
	= toy_catalogue.toy_name
ORDER BY children.[name] ASC
;
