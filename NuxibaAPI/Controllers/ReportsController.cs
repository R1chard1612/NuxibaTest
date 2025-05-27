using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuxibaAPI.Data;
using System.Text;

namespace NuxibaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly NuxibaContext _context;

        public ReportsController(NuxibaContext context)
        {
            _context = context;
        }

        [HttpGet("csv")]
        public async Task<IActionResult> GetWorkHoursCsv()
        {
            var reportData = await (
                from u in _context.Users
                join a in _context.Areas on u.IDArea equals a.IDArea

                // Sesiones emparejadas login->logout con duración en segundos
                join sess in (
                    from l in _context.Logins
                    where l.TipoMov == 1
                    let logoutTime = _context.Logins
                                        .Where(o => o.User_id == l.User_id
                                                 && o.TipoMov == 0
                                                 && o.Fecha > l.Fecha)
                                        .OrderBy(o => o.Fecha)
                                        .Select(o => o.Fecha)
                                        .FirstOrDefault()
                    where logoutTime != default(DateTime)
                    select new
                    {
                        l.User_id,
                        DurationSec = EF.Functions.DateDiffSecond(l.Fecha, logoutTime)
                    }
                ) on u.Id equals sess.User_id

                // Agrupamos por usuario y área
                group sess by new
                {
                    u.Login,
                    u.Nombres,
                    u.ApellidoPaterno,
                    u.ApellidoMaterno,
                    a.AreaName
                } into g

                // Proyección final
                select new
                {
                    g.Key.Login,
                    FullName = string.Concat(g.Key.Nombres, " ",
                                               g.Key.ApellidoPaterno, " ",
                                               g.Key.ApellidoMaterno).Trim(),
                    Area = g.Key.AreaName,
                    // convierte segundos a horas decimales
                    TotalHours = g.Sum(x => x.DurationSec) / 3600.0
                }
            ).ToListAsync();

            // Genera el CSV
            var sb = new StringBuilder();
            sb.AppendLine("Login,NombreCompleto,Area,TotalHoras");
            foreach (var row in reportData)
            {
                sb.AppendLine(
                  $"{row.Login}," +
                  $"\"{row.FullName}\"," +
                  $"\"{row.Area}\"," +
                  $"{row.TotalHours:F2}"
                );
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "workhours_report.csv");
        }
    }
}
