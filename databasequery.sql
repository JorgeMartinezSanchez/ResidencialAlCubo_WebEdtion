DROP SCHEMA public CASCADE 
CREATE SCHEMA public;

TRUNCATE TABLE late_check_out CASCADE;
TRUNCATE TABLE room_guest CASCADE;
TRUNCATE TABLE booking CASCADE;
TRUNCATE TABLE room CASCADE;
TRUNCATE TABLE room_type CASCADE;
TRUNCATE TABLE guest CASCADE;
TRUNCATE TABLE service_contact CASCADE;
TRUNCATE TABLE config CASCADE;

CREATE TABLE guest(
    id SERIAL PRIMARY KEY,
    first_name VARCHAR(100),
    second_name VARCHAR(100),
    last_name VARCHAR(255),
    id_card VARCHAR(50),
    phone_number VARCHAR(25),
    email VARCHAR(255)
);

CREATE TABLE service_contact(
    id SERIAL PRIMARY KEY,
    first_name VARCHAR(100),
    second_name VARCHAR(100),
    last_name VARCHAR(255),
    job_role VARCHAR(50),
    id_card VARCHAR(50),
    phone_number VARCHAR(25),
    email VARCHAR(255),
    available BOOLEAN
);

CREATE TABLE room_type(
    id SERIAL PRIMARY KEY,
    type_name VARCHAR(255),
    price DECIMAL(10, 2),
    capacity INTEGER
);

CREATE TABLE room(
    id SERIAL PRIMARY KEY,
    room_type_id INTEGER,
    room_number VARCHAR(10),
    occupied BOOLEAN,
    FOREIGN KEY (room_type_id) REFERENCES room_type(id)
);

CREATE TABLE booking(
    id SERIAL PRIMARY KEY,
    room_id INTEGER,
    start_date DATE,
    end_date DATE,
    status VARCHAR,
	check_in_date TIMESTAMP,
	check_out_date TIMESTAMP,
    total DECIMAL(10, 2),
    FOREIGN KEY (room_id) REFERENCES room(id)
);

CREATE TABLE room_guest(
    booking_id INTEGER,
    guest_id INTEGER,
    PRIMARY KEY (booking_id, guest_id),
    FOREIGN KEY (booking_id) REFERENCES booking(id),
    FOREIGN KEY (guest_id) REFERENCES guest(id)
);

CREATE TABLE late_check_out(
    id SERIAL PRIMARY KEY,
    booking_id INTEGER,
    extra_hours INTEGER,
    charge DECIMAL(10, 2),
    FOREIGN KEY (booking_id) REFERENCES booking(id)
);

CREATE TABLE config(
    config_key VARCHAR(100) PRIMARY KEY,
    config_value VARCHAR(255)
);

-- TESTING DATA

INSERT INTO room_type (type_name, price, capacity) VALUES
    ('Simple', 120.00, 1),
    ('Suite', 170.00, 1),
    ('Individual Bed Double', 200.00, 2),
    ('Matrimonial Double', 180.00, 2),
    ('Groupal for 4 people', 240.00, 4),
    ('Groupal for 3 people', 210.00, 3);

-- ============================================
-- INSERT ROOMS
-- ============================================
INSERT INTO room (room_type_id, room_number, occupied) VALUES
    -- Piso 1: Simples
    (1, '101', false),
    (1, '102', false),
    (1, '103', false),
    (1, '104', false),

    -- Piso 2: Dobles
    (3, '201', false),
    (3, '202', false),
    (4, '203', false),
    (4, '204', false),

    -- Piso 3: Grupales
    (5, '301', false),
    (5, '302', false),
    (6, '303', false),
    (6, '304', false),

    -- Piso 4: Suites
    (2, '401', false),
    (2, '402', false);

-- ============================================
-- INSERT GUESTS
-- ============================================
INSERT INTO guest (first_name, second_name, last_name, id_card, phone_number, email) VALUES
    ('Israel', '', 'Gutierrez', '53263563', '76342345', 'israel.gutierrez.s@gmail.com'),
    ('Diego', 'Andres', 'Heredia', '9743598', '78118844', 'diego.a.heredia.t@gmail.com'),
    ('Carlos', 'Andres', 'Mamani', '1234567', '78901234', 'carlos.mamani@gmail.com'),
    ('Sofia', 'Paola', 'Quispe', '2345678', '79012345', 'sofia.quispe@gmail.com'),
    ('Diego', 'Luis', 'Flores', '3456789', '77123456', 'diego.flores@gmail.com'),
    ('Maria', 'Elena', 'Condori', '4567890', '76234567', 'maria.condori@gmail.com'),
    ('Jorge', 'Andres', 'Gutierrez', '5678901', '75345678', 'jorge.gutierrez@gmail.com'),
    ('Mijael', 'Ander', 'Callejas', '68406492', '38797443', 'mijael.callejas@gmail.com');

-- ============================================
-- INSERT SERVICE CONTACTS
-- ============================================
INSERT INTO service_contact (first_name, second_name, last_name, job_role, id_card, phone_number, email, available) VALUES
    ('Ana', 'Maria', 'Lopez', 'Housekeeping', '9876543', '71234567', 'ana.lopez@hotel.com', true),
    ('Pedro', 'Jose', 'Vargas', 'Maintenance', '8765432', '72345678', 'pedro.vargas@hotel.com', true),
    ('Lucia', 'Carmen', 'Rojas', 'Room Service', '7654321', '73456789', 'lucia.rojas@hotel.com', false),
    ('Miguel', 'Angel', 'Salinas', 'Security', '6543210', '74567890', 'miguel.salinas@hotel.com', true);

-- ============================================
-- INSERT BOOKINGS WITH CHECK_OUT_DATE
-- ============================================
-- Note: check_out_date is set when guest actually checks out
-- For active/pending bookings, it's NULL
-- For finished bookings, it's the actual checkout datetime
INSERT INTO booking (room_id, start_date, end_date, status, check_in_date, check_out_date, total) VALUES
    -- Finished bookings (completed stays)
    (1, '2026-03-25', '2026-03-28', 'finished', '2026-03-25 14:30:00', '2026-03-28 11:00:00', 360.00),
    (2, '2026-03-10', '2026-03-15', 'finished', '2026-03-10 15:00:00', '2026-03-15 10:30:00', 600.00),
    
    -- Active bookings (checked in, not checked out yet - check_out_date uses sentinel)
    (3, '2026-03-28', '2026-04-02', 'active', '2026-03-28 14:30:00', '1900-01-01 00:00:00', 480.00),
    (5, '2026-03-29', '2026-04-03', 'active', '2026-03-29 15:15:00', '1900-01-01 00:00:00', 800.00),
    (6, '2026-03-30', '2026-04-05', 'active', '2026-03-30 14:00:00', '1900-01-01 00:00:00', 960.00),
    
    -- Pending bookings (future, not checked in - both dates use sentinel)
    (13, '2026-04-01', '2026-04-05', 'pending', '1900-01-01 00:00:00', '1900-01-01 00:00:00', 680.00),
    (9, '2026-04-10', '2026-04-13', 'pending', '1900-01-01 00:00:00', '1900-01-01 00:00:00', 720.00),
    (11, '2026-04-15', '2026-04-20', 'pending', '1900-01-01 00:00:00', '1900-01-01 00:00:00', 1000.00),
    (14, '2026-05-01', '2026-05-07', 'pending', '1900-01-01 00:00:00', '1900-01-01 00:00:00', 1050.00),
    
    -- Cancelled booking
    (4, '2026-03-20', '2026-03-25', 'cancelled', '1900-01-01 00:00:00', '1900-01-01 00:00:00', 0.00);

-- ============================================
-- INSERT ROOM GUESTS
-- ============================================
INSERT INTO room_guest (booking_id, guest_id) VALUES
    -- Finished bookings
    (1, 3),   -- Carlos en reserva 1
    (2, 4),   -- Sofia en reserva 2
    
    -- Active bookings
    (3, 5),   -- Diego en reserva 3
    (4, 6),   -- Maria en reserva 4
    (4, 1),   -- Israel también en reserva 4
    (5, 7),   -- Jorge en reserva 5
    (5, 2),   -- Diego Heredia también en reserva 5
    (6, 8),   -- Mijael en reserva 6
    
    -- Pending bookings
    (7, 3),   -- Carlos en reserva 7
    (8, 4),   -- Sofia en reserva 8
    (8, 5),   -- Diego también en reserva 8
    (9, 6),   -- Maria en reserva 9
    (9, 1),   -- Israel también en reserva 9
    (9, 7),   -- Jorge también en reserva 9
    (10, 8),  -- Mijael en reserva 10
    (10, 2);  -- Diego Heredia también en reserva 10

-- ============================================
-- INSERT LATE CHECK OUT
-- ============================================
INSERT INTO late_check_out (booking_id, extra_hours, charge) VALUES
    (1, 2, 120.00),  -- Finished booking with late check-out
    (3, 3, 180.00),  -- Active booking that had late check-out
    (4, 1, 60.00);   -- Active booking with late check-out

-- ============================================
-- INSERT CONFIGURATION
-- ============================================
INSERT INTO config (config_key, config_value) VALUES 
    ('LateCheckOutHourlyRate', '50.0'),
    ('checkout_limit_hour', '12'),
    ('max_extra_hours', '6'),
    ('cancellation_policy_days', '2'),
    ('cancellation_fee_percentage', '50'),
    ('booking_advance_days', '30');



SELECT config_value FROM Config WHERE config_key = 'LateCheckOutHourlyRate';

SELECT * FROM booking
SELECT * FROM Room_Guest

SELECT id, room_number, room_type_id FROM room;

SELECT R.id, R.room_number, RT.type_name as room_type, RT.price, RT.capacity, R.occupied FROM room AS R
INNER JOIN room_type AS RT ON R.room_type_id = RT.Id

SELECT id, room_number, occupied FROM room WHERE id = 5;

SELECT id, first_name, last_name FROM guest WHERE id IN (7, 8);

SELECT * FROM config WHERE config_key = 'late_checkout_rate';

SELECT g.id, g.first_name, g.second_name, g.last_name, g.id_card, g.phone_number, g.email FROM guest AS g
INNER JOIN room_guest AS rg ON g.id = rg.guest_id
INNER JOIN booking AS b ON rg.booking_id = b.Id;

SELECT * FROM booking

CREATE STORED PROCEDURE
SELECT DISTINCT g.id, g.first_name, g.second_name, g.last_name, g.id_card, g.phone_number, g.email 
FROM guest AS g
INNER JOIN room_guest AS rg ON g.id = rg.guest_id
INNER JOIN booking AS b ON rg.booking_id = b.id
ORDER BY g.id ASC
