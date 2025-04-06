INSERT INTO staff (staff_id, staff_name, manager_id) VALUES
(1, 'Santa Claus', NULL),                -- CEO level
(2, 'Head Elf Operations', 1),           -- Department Head
(3, 'Head Elf Logistics', 1),            -- Department Head
(4, 'Senior Toy Maker', 2),              -- Team Lead
(5, 'Senior Gift Wrapper', 2),           -- Team Lead
(6, 'Inventory Manager', 3),             -- Team Lead
(7, 'Junior Toy Maker', 4),              -- Staff
(8, 'Junior Gift Wrapper', 5),           -- Staff
(9, 'Inventory Clerk', 6),               -- Staff
(10, 'Apprentice Toy Maker', 7);         -- Entry Level
