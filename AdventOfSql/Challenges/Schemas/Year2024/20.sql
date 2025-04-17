CREATE TABLE web_requests (
    request_id INTEGER IDENTITY(1, 1) PRIMARY KEY,
    url NVARCHAR(MAX) NOT NULL
);
