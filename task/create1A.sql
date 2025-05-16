-- tables
-- Table: Appointment
CREATE TABLE Appointment (
    appoitment_id int  NOT NULL,
    patient_id int  NOT NULL,
    doctor_id int  NOT NULL,
    date datetime  NOT NULL,
    CONSTRAINT Appointment_pk PRIMARY KEY  (appoitment_id)
);

-- Table: Appointment_Service
CREATE TABLE Appointment_Service (
    appoitment_id int  NOT NULL,
    service_id int  NOT NULL,
    service_fee decimal(10,2)  NOT NULL,
    CONSTRAINT Appointment_Service_pk PRIMARY KEY  (service_id,appoitment_id)
);

-- Table: Doctor
CREATE TABLE Doctor (
    doctor_id int  NOT NULL,
    first_name nvarchar(100)  NOT NULL,
    last_name nvarchar(100)  NOT NULL,
    PWZ nvarchar(7)  NOT NULL,
    CONSTRAINT Doctor_pk PRIMARY KEY  (doctor_id)
);

-- Table: Patient
CREATE TABLE Patient (
    patient_id int  NOT NULL,
    first_name nvarchar(100)  NOT NULL,
    last_name nvarchar(100)  NOT NULL,
    date_of_birth datetime  NOT NULL,
    CONSTRAINT Patient_pk PRIMARY KEY  (patient_id)
);

-- Table: Service
CREATE TABLE Service (
    service_id int  NOT NULL,
    name nvarchar(100)  NOT NULL,
    base_fee decimal(10,2)  NOT NULL,
    CONSTRAINT Service_pk PRIMARY KEY  (service_id)
);

-- foreign keys
-- Reference: Appointment_Doctor (table: Appointment)
ALTER TABLE Appointment ADD CONSTRAINT Appointment_Doctor
    FOREIGN KEY (doctor_id)
    REFERENCES Doctor (doctor_id);

-- Reference: Appointment_Patient (table: Appointment)
ALTER TABLE Appointment ADD CONSTRAINT Appointment_Patient
    FOREIGN KEY (patient_id)
    REFERENCES Patient (patient_id);

-- Reference: Appointment_Service_Appointment (table: Appointment_Service)
ALTER TABLE Appointment_Service ADD CONSTRAINT Appointment_Service_Appointment
    FOREIGN KEY (appoitment_id)
    REFERENCES Appointment (appoitment_id);

-- Reference: Appointment_Service_Service (table: Appointment_Service)
ALTER TABLE Appointment_Service ADD CONSTRAINT Appointment_Service_Service
    FOREIGN KEY (service_id)
    REFERENCES Service (service_id);

-- End of file.

-- Doctors
INSERT INTO Doctor (doctor_id, first_name, last_name, PWZ) VALUES
(1, 'Anna', 'Kowalska', 'PWZ1234'),
(2, 'Jan', 'Nowak', 'PWZ5678'),
(3, 'Ewa', 'Wiśniewska', 'PWZ9012');

-- Patients
INSERT INTO Patient (patient_id, first_name, last_name, date_of_birth) VALUES
(1, 'Michał', 'Zieliński', '1990-03-15'),
(2, 'Katarzyna', 'Wójcik', '1985-07-23'),
(3, 'Tomasz', 'Kamiński', '1978-11-02'),
(4, 'Agnieszka', 'Lewandowska', '2000-01-17'),
(5, 'Piotr', 'Dąbrowski', '1995-09-30');

-- Services
INSERT INTO Service (service_id, name, base_fee) VALUES
(1, 'Consultation', 100.00),
(2, 'Blood Test', 50.00),
(3, 'X-Ray', 120.00),
(4, 'ECG', 80.00),
(5, 'Vaccination', 60.00);

-- Appointments
INSERT INTO Appointment (appoitment_id, patient_id, doctor_id, date) VALUES
(1, 1, 1, '2024-05-01 09:00'),
(2, 2, 2, '2024-05-01 10:00'),
(3, 3, 3, '2024-05-02 11:30'),
(4, 4, 1, '2024-05-03 14:00'),
(5, 5, 2, '2024-05-04 16:00'),
(6, 1, 3, '2024-05-05 08:30');

-- Appointment_Service
INSERT INTO Appointment_Service (appoitment_id, service_id, service_fee) VALUES
(1, 1, 100.00),   -- Consultation
(1, 2, 55.00),    -- Blood Test
(2, 1, 100.00),   -- Consultation
(3, 3, 125.00),   -- X-Ray
(4, 4, 80.00),    -- ECG
(5, 5, 60.00),    -- Vaccination
(6, 1, 100.00),   -- Consultation
(6, 2, 50.00);    -- Blood Test
