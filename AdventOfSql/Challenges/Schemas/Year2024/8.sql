CREATE TABLE staff (
    staff_id INT PRIMARY KEY,
    staff_name VARCHAR(100) NOT NULL,
    manager_id INT,
    FOREIGN KEY (manager_id) REFERENCES staff(staff_id)
);

CREATE INDEX IX__staff_manager_id
ON staff(manager_id);
