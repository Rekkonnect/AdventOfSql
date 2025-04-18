-- Inserting data into users table
INSERT INTO users (user_id, username) VALUES (1, 'alice');
INSERT INTO users (user_id, username) VALUES (2, 'bob');
INSERT INTO users (user_id, username) VALUES (3, 'carol');

-- Inserting data into songs table, including a song with a NULL duration
INSERT INTO songs (song_id, song_title, song_duration) VALUES (1, 'Jingle Bells', 180);
INSERT INTO songs (song_id, song_title, song_duration) VALUES (2, 'Silent Night', NULL); -- NULL duration
INSERT INTO songs (song_id, song_title, song_duration) VALUES (3, 'Deck the Halls', 150);

-- Inserting example play records into user_plays table, including NULL durations
INSERT INTO user_plays (play_id, user_id, song_id, play_time, duration) VALUES (1, 1, 1, '2024-12-22', 180);  -- Full play
INSERT INTO user_plays (play_id, user_id, song_id, play_time, duration) VALUES (2, 2, 1, '2024-12-22', 100);  -- Skipped
INSERT INTO user_plays (play_id, user_id, song_id, play_time, duration) VALUES (3, 3, 2, '2024-12-22', NULL); -- NULL duration (unknown play)
INSERT INTO user_plays (play_id, user_id, song_id, play_time, duration) VALUES (4, 1, 2, '2024-12-23', 180);  -- Valid duration, but song duration unknown
INSERT INTO user_plays (play_id, user_id, song_id, play_time, duration) VALUES (5, 2, 2, '2024-12-23', NULL); -- NULL duration
INSERT INTO user_plays (play_id, user_id, song_id, play_time, duration) VALUES (6, 3, 3, '2024-12-23', 150);  -- Full play

-- Additional plays with NULLs and shorter durations
INSERT INTO user_plays (play_id, user_id, song_id, play_time, duration) VALUES (7, 1, 3, '2024-12-23', 150);  -- Full play
INSERT INTO user_plays (play_id, user_id, song_id, play_time, duration) VALUES (8, 2, 3, '2024-12-22', 140);  -- Skipped
INSERT INTO user_plays (play_id, user_id, song_id, play_time, duration) VALUES (9, 3, 1, '2024-12-23', NULL); -- NULL duration
INSERT INTO user_plays (play_id, user_id, song_id, play_time, duration) VALUES (10, 1, 3, '2024-12-22', NULL); -- NULL duration
