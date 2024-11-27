using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Model;

[ApiController]
[Route("api/[controller]")]
public class IPInfoController : ControllerBase
{
    private readonly IIPInfoService _ipInfoService;

    public IPInfoController(IIPInfoService ipInfoService)
    {
        _ipInfoService = ipInfoService;
    }

    [HttpGet("{ip}")]
    public async Task<IActionResult> GetIPDetails(string ip)
    {
        try
        {
            var details = await _ipInfoService.GetIPDetailsAsync(ip);
            return Ok(details);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("update")]
    public IActionResult UpdateIPDetails([FromBody] List<IPEntityDto> ips)
    {
        if (ips == null || ips.Count == 0)
        {
            return BadRequest("The request must contain a list of IP details to update.");
        }

        // Create an update job in the repository
        var jobId = _ipInfoService.CreateUpdateJob(ips);

        // Return the job GUID to the client
        return Ok(new { JobId = jobId });
    }

    [HttpGet("job/{jobId}")]
    public IActionResult GetJobStatus(Guid jobId)
    {
        var job = _ipInfoService.GetJobStatus(jobId);
        if (job == null)
        {
            return NotFound(new { message = "Job not found" });
        }

        return Ok(job);
    }

}
