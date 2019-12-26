using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using SeedApi.Data;
using SeedApi.Models;
using SeedApi.Entities;
using SeedApi.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SeedApi.Controllers {
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

        public async Task<IActionResult> Index() {
            return View(await _context.IssueEmployee.AsNoTracking().Take(100).ToListAsync());
        }

        public IActionResult Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var emp = _service.Get(id);
            if (emp == null) {
                return NotFound();
            }

            return View(emp);
        }

        public IActionResult Create() {
            return View("Edit");
        }

        [HttpGet]
        public IActionResult Edit(int? id) {
            if (id == null) {
                return NotFound();
            }
            var emp = _service.Get(id);
            if (emp == null) {
                return NotFound();
            }
            emp.ConfirmPassword = emp.Password;
            return View(emp);
        }

        [HttpPost]
        public IActionResult Edit(int id, IssueEmployee emp) {
            if (id != emp.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _service.Save(emp);
                } catch (DbUpdateException ex) {
                    throw ex;
                }
                return RedirectToAction("Index");
            }
            return View(emp);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}