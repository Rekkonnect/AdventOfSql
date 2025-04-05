CREATE TABLE children (
  child_id INT PRIMARY KEY,
  name VARCHAR(100),
  age INT
);
CREATE TABLE wish_lists (
  list_id INT PRIMARY KEY,
  child_id INT,
  wishes NVARCHAR(MAX),
  submitted_date DATE
);
CREATE TABLE toy_catalogue (
  toy_id INT PRIMARY KEY,
  toy_name VARCHAR(100),
  category VARCHAR(50),
  difficulty_to_make INT
);