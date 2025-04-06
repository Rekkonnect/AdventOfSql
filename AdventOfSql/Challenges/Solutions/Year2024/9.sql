WITH
average_speeds AS (
    SELECT
        average_speed = AVG(speed_record),
        exercise_name,
        training_sessions.reindeer_id
    FROM training_sessions
    INNER JOIN reindeers
        ON reindeers.reindeer_id = training_sessions.reindeer_id
    WHERE reindeers.reindeer_name != 'Rudolf'
    GROUP BY exercise_name, training_sessions.reindeer_id
),
reindeer_score_speeds AS (
    SELECT
        highest_average_score = MAX(average_speed),
        reindeer_id
    FROM average_speeds
    GROUP BY reindeer_id
)
SELECT TOP 3
    reindeers.reindeer_name,
    reindeer_score_speeds.highest_average_score,
    [name,highest_average_score] = CONCAT(
			reindeers.reindeer_name, ',',
			CAST(ROUND(reindeer_score_speeds.highest_average_score, 2) AS DECIMAL(16,2))
		)
FROM reindeer_score_speeds
    INNER JOIN reindeers
        ON reindeers.reindeer_id = reindeer_score_speeds.reindeer_id
ORDER BY highest_average_score DESC
;
