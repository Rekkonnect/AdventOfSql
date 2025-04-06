CREATE TABLE Drinks (
    drink_id INTEGER IDENTITY(1, 1) PRIMARY KEY,
    drink_name VARCHAR(50) NOT NULL,
    date DATE NOT NULL,
    quantity INTEGER NOT NULL
);
