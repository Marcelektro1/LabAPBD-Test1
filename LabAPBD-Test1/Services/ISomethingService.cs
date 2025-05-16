using LabAPBD_Test1.Services.Util;

namespace LabAPBD_Test1.Services;

public interface ISomethingService
{
    Task<ServiceResult> DoSomethingAsync();
}