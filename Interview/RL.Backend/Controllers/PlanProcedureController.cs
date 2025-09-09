using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RL.Data;
using RL.Data.DataModels;
using RL.Backend.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RL.Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class PlanProcedureController : ControllerBase
{
    private readonly ILogger<PlanProcedureController> _logger;
    private readonly RLContext _context;

    public PlanProcedureController(ILogger<PlanProcedureController> logger, RLContext context)
    {
        _logger = logger;
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet]
    [EnableQuery]
    public IEnumerable<PlanProcedure> Get()
    {
        return _context.PlanProcedures;
    }

    [HttpGet("Assignments")]
    [EnableQuery]
    public IEnumerable<PlanProcedureUser> GetAssignments([FromQuery][BindRequired] int planId)
    {
        return _context.PlanProcedureUsers.Where(x => x.PlanId == planId);
    }

    

    [HttpPost("Assignments")]
    public IActionResult UpsertAssignments([FromBody] UpsertAssignmentsDto dto)
    {
        if (dto == null) return BadRequest();

        var existing = _context.PlanProcedureUsers
            .Where(a => a.PlanId == dto.PlanId && a.ProcedureId == dto.ProcedureId)
            .ToList();

        foreach (var e in existing)
        {
            if (!dto.UserIds.Contains(e.UserId))
            {
                _context.PlanProcedureUsers.Remove(e);
            }
        }

        var existingIds = existing.Select(e => e.UserId).ToHashSet();
        foreach (var uid in dto.UserIds.Distinct())
        {
            if (!existingIds.Contains(uid))
            {
                _context.PlanProcedureUsers.Add(new PlanProcedureUser
                {
                    PlanId = dto.PlanId,
                    ProcedureId = dto.ProcedureId,
                    UserId = uid
                });
            }
        }

        _context.SaveChanges();

        return Ok();
    }
}
