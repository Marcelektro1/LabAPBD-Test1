using LabAPBD_Test1.Model;
using LabAPBD_Test1.Services.Util;

namespace LabAPBD_Test1.Services;

public interface IAppointmentsService
{
    public Task<AppointmentDto> GetAppointmentDetails(int appointmentId);
}