using System;
using System.Linq;
using System.Threading.Tasks;

using SeedApi.Data;
using SeedApi.Models;
using SeedApi.Entities;
using SeedApi.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using SeedApi.Helpers;
using System.Data;
using System.IO;
using OfficeOpenXml;

namespace SeedApi.Controllers {
    [Authorize]
    [ApiController]
    [ApiVersion("1")]
    [Route("api/users")]
    public class IssueEmployeeController : Controller {
        private readonly ILogger<IssueEmployeeController> _logger;
        private readonly SeedApiContext _context;
        private readonly IssueEmployeeService _service;

        public IssueEmployeeController(
            ILogger<IssueEmployeeController> logger,
            SeedApiContext context,
            IssueEmployeeService service
            ) {
            _logger = logger;
            _context = context;
            _service = service;
        }

        /// <summary>
        /// 로그인
        /// </summary>
        [AllowAnonymous]
        [HttpPost("[action]")]
        public IActionResult SignIn([FromBody]SignInModel model) {
            var emp = _service.Login(model.Account, model.Password, null);

            if (emp == null)
                return BadRequest("아이디나 비밀번호가 잘못되었습니다.");

            return Ok(new {
                emp.Id,
                emp.Name,
                emp.Account,
                emp.EmployeeNo,
                emp.Email,
                emp.Tel,
                emp.Phone,
                emp.TeamCode,
                emp.TeamName,
                emp.GroupCode,
                emp.User.Permissions,
                emp.User.Token,
            });
        }

        /// <summary>
        /// 목록
        /// </summary>
        [HttpGet]
        public IActionResult List([FromQuery]PagingRequest req) {
            _logger.LogInformation(req.Dump());

            var query = _FilterAndSort(req);
            var list = query.Select(x => new {
                x.Id,
                x.Name,
                팀코드 = x.TeamCode,
                팀명 = x.TeamName,
                x.Account,
                x.EmployeeNo,
                x.Created,
                x.Updated,
            });
            var paging = PagingList.Pagination(list, req.page, req.limit);
            return Ok(paging);
        }

        // 엑셀 다운로드
        [AllowAnonymous]
        [HttpGet("excel")]
        public IActionResult ExportExcel([FromQuery]PagingRequest req, [FromQuery]string mapping) {
            var query = _FilterAndSort(req);

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream)) {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells.LoadFromCollection(query, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"UserList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            //return File(stream, "application/octet-stream", excelName);  
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        /// <summary>
        /// 상세조회
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(int id) {
            var emp = _service.GetById(id);

            if (emp == null)
                return NotFound();

            return Ok(new {
                emp.Id,
                emp.Name,
                emp.TeamCode,
                emp.TeamName,
                emp.IsDeveloper,
                emp.Account,
                emp.EmployeeNo,
                emp.Phone,
                emp.Created,
                emp.Updated,
            });
        }

        /// <summary>
        /// 수정
        /// </summary>
        [HttpPost("{id}")]
        public IActionResult Edit(int id, [FromBody]IssueEmployee emp) {
            _logger.LogInformation(emp.Dump());
            /*
            var currentUserId = int.Parse(User.Identity.Name);
            if (id != currentUserId)
                return Forbid();
            if (!User.IsInRole(Role.Admin))
                return Forbid();
            */

            if (id != emp.Id) {
                return NotFound();
            }

            ModelState.Remove("Password"); // 이 수정에서는 비밀번호 제외
            if (ModelState.IsValid) {
                try {
                    _service.Save(emp);
                } catch (ServiceException ex) {
                    return BadRequest(ex.Message);
                }
            } else {
                foreach (var ms in ModelState.ToArray()) {
                    _logger.LogInformation(ms.Key);
                }
                return BadRequest(ModelState);
            }

            return Ok(emp);
        }

        /// <summary>
        /// 등록
        /// </summary>
        [HttpPut]
        public IActionResult Create([FromBody]IssueEmployee emp) {
            if (!User.IsInRole(Role.Admin))
                return Forbid();

            if (ModelState.IsValid) {
                try {
                    _service.Save(emp);
                } catch (ServiceException ex) {
                    return BadRequest(ex.Message);
                }
            } else {
                return BadRequest(ModelState);
            }
            return Ok(emp);
        }

        // Role 제한 테스트
        [AllowAnonymous]
        [HttpGet("auth")]
        public IActionResult Auth() {

            return Ok();
        }

        // 검색 & 정렬 공통
        private IQueryable<IssueEmployee> _FilterAndSort(PagingRequest req) {
            var query = _service.GetAll();
            if (req.conditions != null) {
                foreach (var cond in req.conditions) {
                    switch ($"{cond.field}_{cond.op}") {
                        case "name_eq":
                            query = query.Where(x => x.Name == cond.value);
                            break;
                        case "name_cn":
                            query = query.Where(x => x.Name.Contains(cond.value));
                            break;
                    }
                }
            }
            query = query.Where(x => x.IsDelete == false);
            if (req.sort != null) {
                switch ($"{req.sort.field}_{req.sort.order}") {
                    case "id_asc":
                        query = query.OrderBy(x => x.Id);
                        break;
                    case "id_desc":
                        query = query.OrderByDescending(x => x.Id);
                        break;
                    case "name_asc":
                        query = query.OrderBy(x => x.Name);
                        break;
                    case "name_desc":
                        query = query.OrderByDescending(x => x.Name);
                        break;
                }
            }
            return query;
        }

    }
}