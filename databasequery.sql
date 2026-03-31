DROP SCHEMA public CASCADE 
CREATE SCHEMA public;

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

INSERT INTO room_type (
	type_name,
	price,
	capacity
) VALUES
	('Simple', 120.00, 1),
	('Suite', 170.00, 1),
	('Individual Bed Double', 200.00, 2),
	('Matrimonial Double', 180.00, 2),
	('Groupal for 4 people', 240.00, 4),
	('Groupal for 3 people', 210.00, 3)

INSERT INTO room (room_type_id, room_number, occupied)
VALUES
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

-- GUESTS
INSERT INTO guest (first_name, second_name, last_name, id_card, phone_number, email) VALUES
    ('Carlos', 'Andres', 'Mamani', '1234567', '78901234', 'carlos.mamani@gmail.com'),
    ('Sofia', 'Paola', 'Quispe', '2345678', '79012345', 'sofia.quispe@gmail.com'),
    ('Diego', 'Luis', 'Flores', '3456789', '77123456', 'diego.flores@gmail.com'),
    ('Maria', 'Elena', 'Condori', '4567890', '76234567', 'maria.condori@gmail.com'),
    ('Jorge', 'Andres', 'Gutierrez', '5678901', '75345678', 'jorge.gutierrez@gmail.com'),
	('Mijael', 'Ander', 'Callejas', '68406492', '38797443', 'mijael.callejas@gmail.com');

-- SERVICE CONTACTS
INSERT INTO service_contact (first_name, second_name, last_name, job_role, id_card, phone_number, email, available) VALUES
    ('Ana', 'Maria', 'Lopez', 'Housekeeping', '9876543', '71234567', 'ana.lopez@hotel.com', true),
    ('Pedro', 'Jose', 'Vargas', 'Maintenance', '8765432', '72345678', 'pedro.vargas@hotel.com', true),
    ('Lucia', 'Carmen', 'Rojas', 'Room Service', '7654321', '73456789', 'lucia.rojas@hotel.com', false),
    ('Miguel', 'Angel', 'Salinas', 'Security', '6543210', '74567890', 'miguel.salinas@hotel.com', true);

-- BOOKINGS
INSERT INTO booking (room_id, start_date, end_date, status, total) VALUES
    (1, '2026-03-25', '2026-03-28', 'active', 360.00),      -- Simple 3 noches
    (5, '2026-03-26', '2026-03-30', 'active', 800.00),      -- Doble individual 4 noches
    (13, '2026-04-01', '2026-04-05', 'pending', 680.00),    -- Suite 4 noches
    (9, '2026-04-10', '2026-04-13', 'pending', 720.00);     -- Grupal 4 personas 3 noches

-- ROOM GUESTS (asociar huéspedes a reservas)
INSERT INTO room_guest (booking_id, guest_id) VALUES
    (1, 1),   -- Carlos en reserva 1
    (2, 2),   -- Sofia en reserva 2
    (2, 3),   -- Diego también en reserva 2
    (3, 4),   -- Maria en reserva 3
    (4, 5),   -- Jorge en reserva 4
    (4, 1),   -- Carlos también en reserva 4
    (4, 2),   -- Sofia también en reserva 4
    (4, 3);   -- Diego también en reserva 4

-- LATE CHECK OUT de prueba
INSERT INTO late_check_out (booking_id, extra_hours, charge) VALUES
    (1, 3, 150.00);



INSERT INTO config (config_key, config_value) VALUES ('LateCheckOutHourlyRate', '50.0');
SELECT config_value FROM Config WHERE config_key = 'LateCheckOutHourlyRate';

SELECT * FROM room_type

SELECT R.id, R.room_number, RT.type_name as room_type, RT.price, RT.capacity, R.occupied FROM room AS R
INNER JOIN room_type AS RT ON R.room_type_id = RT.Id

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
