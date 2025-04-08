WITH
receipts AS (
    SELECT
        records.record_id,
        records.record_date,
        receipts.receipt_id,
        receipts.garment,
        receipts.color,
        receipts.drop_off
    FROM SantaRecords records
    CROSS APPLY OPENJSON(records.cleaning_receipts)
    WITH (
        receipt_id VARCHAR(MAX) '$.receipt_id',
        garment VARCHAR(MAX) '$.garment',
        color VARCHAR(MAX) '$.color',
        drop_off DATE '$.drop_off'
    ) receipts
),
receipts_with_green_suit AS (
    SELECT *
    FROM receipts
    WHERE color = 'green'
        AND garment = 'suit'
)
SELECT TOP 1
    drop_off
FROM receipts_with_green_suit
ORDER BY drop_off DESC
;
