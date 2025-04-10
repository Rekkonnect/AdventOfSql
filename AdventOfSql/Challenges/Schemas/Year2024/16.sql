CREATE TABLE sleigh_locations (
    timestamp DATETIMEOFFSET NOT NULL,
    coordinate GEOGRAPHY NOT NULL
);

CREATE TABLE areas (
    place_name VARCHAR(255) NOT NULL,
    polygon GEOGRAPHY NOT NULL
);
