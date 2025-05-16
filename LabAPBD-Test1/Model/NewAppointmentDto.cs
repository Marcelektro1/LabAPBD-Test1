using System.ComponentModel.DataAnnotations;

namespace LabAPBD_Test1.Model;

public class NewAppointmentDto
{
    [Required]
    public int? AppointmentId { get; set; }
    
    [Required]
    public int? PatientId { get; set; }
    
    [Required]
    public string? Pwz { get; set; }
    
    [Required]
    public List<NewAppointmentServiceDto>? Services { get; set; }
    
}


public class NewAppointmentServiceDto
{
    [Required]
    public string? ServiceName { get; set; }
    
    [Required]
    public double? ServiceFee { get; set; }
}
