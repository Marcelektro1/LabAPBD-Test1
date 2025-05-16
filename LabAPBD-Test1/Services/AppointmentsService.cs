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
}