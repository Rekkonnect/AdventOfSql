CREATE TABLE christmas_menus (
    id INTEGER PRIMARY KEY,
    -- inserting as a raw string to avoid issues with DTD
    menu_data NVARCHAR(MAX)
);
