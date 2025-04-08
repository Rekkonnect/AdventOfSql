CREATE TABLE gifts (
    gift_id INTEGER IDENTITY(1, 1) PRIMARY KEY,
    gift_name VARCHAR(100) NOT NULL,
    price DECIMAL(10,2)
);

CREATE TABLE gift_requests (
    request_id INTEGER IDENTITY(1, 1) PRIMARY KEY,
    gift_id INT,
    request_date DATE,
    FOREIGN KEY (gift_id) REFERENCES Gifts(gift_id)
);
