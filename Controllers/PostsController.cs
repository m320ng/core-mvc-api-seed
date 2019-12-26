using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SeedApi.Services;
using SeedApi.Models;
using SeedApi.Entities;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace SeedApi.Controllers {
    public class SignInModel {
        public string email { get; set; }
        public string password { get; set; }
    }

    [Authorize]
    [ApiController]
    //[ApiVersion("1")]
    //[Route("posts")]
    [Route("api/[controller]/[action]")]
    public class PostsController : ControllerBase {
        private readonly ILogger<PostsController> _logger;
        private IssueEmployeeService _issueEmpService;

        public PostsController(ILogger<PostsController> logger, IssueEmployeeService issueEmpService) {
            _logger = logger;
            _issueEmpService = issueEmpService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult SignIn([FromBody]SignInModel model) {
            var emp = _issueEmpService.Login(model.email, model.password, null);

            if (emp == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var user = emp.User;
            user.Permissions = new string[] { "Admin" };

            return Ok(emp.User);
        }

        /// <summary>
        /// 관리자 목록
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll([FromQuery]PaginationRequest req) {
            var query = _issueEmpService.All();
            _logger.LogInformation($"getall page={req.page}");

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
            if (req.sort != null) {
                switch ($"{req.sort.field}_{req.sort.field}") {
                    case "name_asc":
                        query = query.OrderBy(x => x.Name);
                        break;
                    case "name_desc":
                        query = query.OrderByDescending(x => x.Name);
                        break;
                }
            }

            var list = query.Select(x => new {
                x.Id,
                x.Name,
                x.Account,
                x.EmployeeNo
            });

            var paging = PaginationList.Pagination(list, req.page, req.limit);
            return Ok(paging);
        }

        /// <summary>
        /// 관리자 상세
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(int id) {
            // only allow admins to access other user records
            /*
            var currentUserId = int.Parse(User.Identity.Name);
            if (id != currentUserId && !User.IsInRole(Role.Admin))
                return Forbid();
            */
            var user = _issueEmpService.Get(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// 권한 상세
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult Role(int id) {
            return Ok(User.Identity);
        }
    }
}

public class IssueEmployeeViewModel {
    public string Name { get; set; }
}