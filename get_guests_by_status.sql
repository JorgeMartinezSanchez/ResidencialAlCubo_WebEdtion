CREATE OR REPLACE FUNCTION get_guests_by_status(state VARCHAR)
RETURNS TABLE (
    id INTEGER,
    first_name VARCHAR,
    second_name VARCHAR,
    last_name VARCHAR,
    id_card VARCHAR,
    phone_number VARCHAR,
    email VARCHAR
)
AS $$
    SELECT DISTINCT g.*
    FROM guest AS g
    INNER JOIN room_guest AS rg ON g.id = rg.guest_id
    INNER JOIN booking AS b ON rg.booking_id = b.id
    WHERE b.status = state
    ORDER BY g.id ASC;
$$ LANGUAGE sql;