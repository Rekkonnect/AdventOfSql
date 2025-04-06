WITH
average_gift_price AS (
    SELECT AVG(price) AS average_price
    FROM gifts
),
children_with_gifts AS (
    SELECT
        children.name AS child_name,
        gifts.*
    FROM children
    INNER JOIN gifts
        ON gifts.child_id = children.child_id
),
children_with_expensive_gifts AS (
    SELECT
        child_name,
        price
    FROM children_with_gifts
    WHERE children_with_gifts.price > (
        SELECT average_price FROM average_gift_price)
)
SELECT TOP 1 *
FROM children_with_expensive_gifts
ORDER BY price
;
