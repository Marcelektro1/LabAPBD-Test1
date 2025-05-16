using System.Data;
using LabAPBD_Test1.Model;
using LabAPBD_Test1.Services.Util;
using Microsoft.Data.SqlClient;

namespace LabAPBD_Test1.Services;

public class AppointmentsService(IConfiguration configuration) : IAppointmentsService
{
    public async Task<AppointmentDto> GetAppointmentDetails(int appointmentId)
    {
        await using var connection = new SqlConnection(configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;
        
        try
        {
            command.CommandText = """
                                  SELECT A.date as "date", 
                                         P.first_name as "patientFirstName", P.last_name as "patientLastName", P.date_of_birth as "patientDateOfBirth",
                                         D.doctor_id as "doctorId", D.PWZ as "doctorPwz",
                                         S.name as "serviceName", ASS.service_fee as "serviceFee"
                                  FROM Appointment A
                                  INNER JOIN Patient P on A.patient_id = P.patient_id
                                  INNER JOIN Doctor D on A.doctor_id = D.doctor_id
                                  LEFT JOIN s30500.Appointment_Service ASS on A.appoitment_id = ASS.appoitment_id
                                  LEFT JOIN s30500.Service S on ASS.service_id = S.service_id
                                  WHERE A.appoitment_id = @AppointmentId;
                                  """;
            command.Parameters.AddWithValue("@AppointmentId", appointmentId);
            
            AppointmentDto? appointmentDto = null;

            await using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    if (appointmentDto == null) // first row
                    {
                        appointmentDto = new AppointmentDto
                        {
                            Date = reader.GetDateTime("date"),
                            Patient = new PatientDto
                            {
                                FirstName = reader.GetString("patientFirstName"),
                                LastName = reader.GetString("patientLastName"),
                                DateOfBirth = reader.GetDateTime("patientDateOfBirth")
                            },
                            Doctor = new DoctorDto
                            {
                                DoctorId = reader.GetInt32("doctorId"),
                                Pwz = reader.GetString("doctorPwz")
                            },
                            AppointmentServices =
                            [
                                new AppointmentServiceDto
                                {
                                    Name = reader.GetString("serviceName"),
                                    ServiceFee = Convert.ToDouble(reader.GetDecimal("serviceFee"))
                                }

                            ]
                        };
                        continue;
                    }
                    
                    // now it's not null and we only add more services
                    if (appointmentDto == null) // assert
                        throw new InvalidProgramException("impossible");
                    
                    appointmentDto.AppointmentServices.Add(new AppointmentServiceDto()
                    {
                        Name = reader.GetString("serviceName"),
                        ServiceFee = Convert.ToDouble(reader.GetDecimal("serviceFee"))
                    });
                    
                }
            }

            if (appointmentDto == null)
                throw new ArgumentException("No appointment found by provided id");

            command.Parameters.Clear();

            await transaction.CommitAsync();
            
            return appointmentDto;
            
        } 
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw; // rethrow
        }
        
    }

    public async Task<ServiceResult> CreateAppointment(NewAppointmentDto appointmentDto)
    {
        await using var connection = new SqlConnection(configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;
        
        try
        {
            command.CommandText = """
                                  SELECT 1
                                  FROM Appointment A 
                                  WHERE A.appoitment_id = @AppointmentId;
                                  """;
            command.Parameters.AddWithValue("@AppointmentId", appointmentDto.AppointmentId);

            var checkAppExists = await command.ExecuteScalarAsync();
            if (checkAppExists != null)
            {
                throw new ArgumentException("Appointment with given id already exists");
            }
            command.Parameters.Clear();


            command.CommandText = """
                                  SELECT 1
                                  FROM Patient P 
                                  WHERE P.patient_id = @PatientId;
                                  """;
            command.Parameters.AddWithValue("@PatientId", appointmentDto.PatientId);
            var checkPatientExists = await command.ExecuteScalarAsync();
            if (checkPatientExists == null)
            {
                throw new ArgumentException("Patient with given id does not exist");
            }
            command.Parameters.Clear();


            command.CommandText = """
                                  SELECT D.doctor_id
                                  FROM Doctor D
                                  WHERE D.PWZ = @Pwz;
                                  """;
            command.Parameters.AddWithValue("@Pwz", appointmentDto.Pwz);
            var doctorCheckId = await command.ExecuteScalarAsync();
            if (doctorCheckId == null)
            {
                throw new ArgumentException("Doctor with given id does not exist");
            }
            command.Parameters.Clear();
            
            var doctorId = Convert.ToInt32(doctorCheckId);
            
            
            // adding the new appointment entry
            
            command.CommandText = """
                                  INSERT INTO Appointment(appoitment_id, patient_id, doctor_id, date) 
                                  VALUES(@AppointmentId, @PatientId, @DoctorId, @Date);
                                  """;
            command.Parameters.AddWithValue("@AppointmentId", appointmentDto.AppointmentId);
            command.Parameters.AddWithValue("@PatientId", appointmentDto.PatientId);
            command.Parameters.AddWithValue("@DoctorId", doctorId);
            command.Parameters.AddWithValue("@Date", DateTime.UtcNow); // TODO: task didnt specify what date to use

            var addAppAff = await command.ExecuteNonQueryAsync();

            if (addAppAff != 1)
            {
                // should not happen
                throw new InvalidProgramException("could not add appointment, maybe it already exists?");
            }
            
            command.Parameters.Clear();
            
            
            
            foreach (var newService in appointmentDto.Services)
            {
                command.CommandText = """
                                      SELECT S.service_id
                                      FROM Service S 
                                      WHERE S.name = @ServiceName;
                                      """;
                command.Parameters.AddWithValue("@ServiceName", newService.ServiceName);
                var serviceCheckId = await command.ExecuteScalarAsync();
                if (serviceCheckId == null)
                {
                    throw new ArgumentException("Service with given name does not exist");
                }
                command.Parameters.Clear();
                var serviceId = Convert.ToInt32(serviceCheckId);
                
                // for each service we successfully get, we add them to db
                // since in transaction, we can easily rollback if the next user-provided service doesnt exist
                // so we dont waste app memory :D

                command.CommandText = """
                                      INSERT INTO Appointment_Service(appoitment_id, service_id, service_fee) 
                                      VALUES (@AppointmentId, @ServiceId, @ServiceFee);
                                      """;
                command.Parameters.AddWithValue("@AppointmentId", appointmentDto.AppointmentId);
                command.Parameters.AddWithValue("@ServiceId", serviceId);
                command.Parameters.AddWithValue("@ServiceFee", newService.ServiceFee);
                
                var aff = await command.ExecuteNonQueryAsync();

                if (aff != 1)
                {
                    // should not happen
                    throw new InvalidProgramException("Appointment with given id already has that service registered.");
                }
                
                command.Parameters.Clear();

            }


            await transaction.CommitAsync();
            
            return new ServiceResult(true, "Appointment added")
            {
                Result = appointmentDto.AppointmentId
            };

        } 
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw; // rethrow
        }
    }
    
}