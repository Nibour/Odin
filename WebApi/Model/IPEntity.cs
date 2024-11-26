using System.ComponentModel.DataAnnotations;
using IPStackCommunicationLibrary;

namespace WebApi.Model;

public class IPEntity : IPDetails
{
    [Key]
    public required string IP { get; set; }
    
}
