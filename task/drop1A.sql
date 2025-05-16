-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2025-05-06 18:32:39.331

-- foreign keys
ALTER TABLE Appointment DROP CONSTRAINT Appointment_Doctor;

ALTER TABLE Appointment DROP CONSTRAINT Appointment_Patient;

ALTER TABLE Appointment_Service DROP CONSTRAINT Appointment_Service_Appointment;

ALTER TABLE Appointment_Service DROP CONSTRAINT Appointment_Service_Service;

-- tables
DROP TABLE Appointment;

DROP TABLE Appointment_Service;

DROP TABLE Doctor;

DROP TABLE Patient;

DROP TABLE Service;

-- End of file.

