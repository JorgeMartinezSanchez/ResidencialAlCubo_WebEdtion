CREATE TABLE Guest(
	Id SERIAL PRIMARY KEY,
	FirstName VARCHAR(100),
	SecondName VARCHAR(100),
	LastName VARCHAR(255),
	IDCard VARCHAR(50),
	PhoneNumber VARCHAR(25),
	Email VARCHAR(255)
)

CREATE TABLE ServiceContact(
	Id SERIAL PRIMARY KEY,
	FirstName VARCHAR(100),
	SecondName VARCHAR(100),
	LastName VARCHAR(255),
	IDCard VARCHAR(50),
	PhoneNumber VARCHAR(25),
	Email VARCHAR(255),
	Available BOOLEAN
)

CREATE TABLE RoomType(
	Id SERIAL PRIMARY KEY,
	Name VARCHAR(255),
	Price DECIMAL(10, 2),
	Capacity INTEGER
)

CREATE TABLE Room(
	Id SERIAL PRIMARY KEY,
	RoomTypeId INTEGER,
	RoomNumber VARCHAR(10),
	Busy BOOLEAN,

	FOREIGN KEY (RoomTypeId) REFERENCES RoomType(Id)
)

CREATE TABLE RoomReservation(
	Id SERIAL PRIMARY KEY,
	GuestId INTEGER,
	RoomId INTEGER,
	StartDate DATE,
	EndDate DATE,
	CheckInTime DATE,
	CheckOutTime DATE,
	GuestCount INTEGER,
	State VARCHAR,
	Total DECIMAL(10, 2),
	
	FOREIGN KEY (GuestId) REFERENCES Guest(Id),
	FOREIGN KEY (RoomId) REFERENCES Room(Id)
)

CREATE TABLE LateCheckOut(
	Id SERIAL PRIMARY KEY,
	RoomReservationId INTEGER,
	ExtraHours INTEGER,
	Charge DECIMAL(10, 2),

	FOREIGN KEY (RoomReservationId) REFERENCES RoomReservation(Id)
);