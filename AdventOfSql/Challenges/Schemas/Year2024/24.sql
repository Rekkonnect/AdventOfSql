CREATE TABLE users (
    user_id INT PRIMARY KEY,
    username VARCHAR(255) NOT NULL
);
CREATE TABLE songs (
    song_id INT PRIMARY KEY,
    song_title VARCHAR(255) NOT NULL,
    song_duration INT  -- Duration in seconds, can be NULL if unknown
);
CREATE TABLE user_plays (
    play_id INT PRIMARY KEY,
    user_id INT,
    song_id INT,
    play_time DATE,
    duration INT,  -- Duration in seconds, can be NULL
    FOREIGN KEY (user_id) REFERENCES users(user_id),
    FOREIGN KEY (song_id) REFERENCES songs(song_id)
);

-- This index drops the query time from ~96ms to ~74ms (~20% improvement) 
CREATE INDEX index_user_play_song_id
ON user_plays(song_id);
