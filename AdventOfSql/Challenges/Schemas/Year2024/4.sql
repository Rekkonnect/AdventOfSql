-- The problem requires that the DBMS supports array columns, like Postgres does
-- MS SQL Server 16.0 does not support them.
-- This constraint completely changes the nature of the problem and is thus
-- ignored for the purposes of solving those problems in MS SQL as long as they are
-- capable of solving natively with little modification to the original design

-- Quoting:
-- Q: What database system are we using?
-- A: I'm basing it on Postgres, but you should be able to use any SQL database, your mileage may vary.
RAISERROR ('Unsolvable due to incompatible schema', 15, 1);
