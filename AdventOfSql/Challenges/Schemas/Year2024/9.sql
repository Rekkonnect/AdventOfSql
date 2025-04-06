CREATE TABLE Reindeers (
    reindeer_id INTEGER PRIMARY KEY,
    reindeer_name VARCHAR(50) NOT NULL,
    years_of_service INTEGER NOT NULL,
    speciality VARCHAR(100)
);

CREATE TABLE Training_Sessions (
    session_id INTEGER IDENTITY(1, 1) PRIMARY KEY,
    reindeer_id INTEGER REFERENCES Reindeers(reindeer_id),
    exercise_name VARCHAR(100) NOT NULL,
    speed_record DECIMAL(5,2) NOT NULL,
    session_date DATE NOT NULL,
    weather_conditions VARCHAR(50)
);
