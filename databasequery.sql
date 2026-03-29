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
    total DECIMAL(10, 2),
    FOREIGN KEY (room_id) REFERENCES room(id)
);

CREATE TABLE room_guest(
    booking_id INTEGER,
    guest_id INTEGER,
    PRIMARY KEY (booking, guest_id),
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

ALTER TABLE room_guest RENAME COLUMN room_reservation_id TO booking_id

CREATE TABLE config(
    config_key VARCHAR(100) PRIMARY KEY,
    config_value VARCHAR(255)
);

-- DATA ADDING

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



INSERT INTO config (config_key, config_value) VALUES ('LateCheckOutHourlyRate', '50.00');
SELECT config_value FROM Config WHERE config_key = 'LateCheckOutHourlyRate';

SELECT R.id, R.room_number, RT.type_name as room_type, RT.price, RT.capacity, R.occupied FROM room AS R
INNER JOIN room_type AS RT ON R.room_type_id = RT.Id
