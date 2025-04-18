WITH
play_sessions AS (
    SELECT
        songs.song_id,
        song_title,
        play_id,
        was_skipped = CASE
			WHEN (user_plays.duration IS NULL
				OR user_plays.duration < songs.song_duration)
				THEN 1
				ELSE 0
			END
    FROM user_plays
    INNER JOIN songs
        ON user_plays.song_id = songs.song_id
),
play_stats AS (
    SELECT
        song_id,
        total_plays = COUNT(*),
        total_skips = SUM(was_skipped)
    FROM play_sessions
    GROUP BY song_id
)
SELECT TOP 1
    songs.song_title
FROM play_stats
INNER JOIN songs
    ON play_stats.song_id = songs.song_id
ORDER BY
    total_plays DESC,
    total_skips
;
