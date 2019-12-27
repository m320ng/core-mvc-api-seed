using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

using SeedApi.Data;
using SeedApi.Helpers;
using SeedApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SeedApi.Models;

namespace SeedApi.Services {
    public class IssueEmployeeService {
        private readonly ILogger<IssueEmployeeService> _logger;
        private readonly SeedApiContext _context;

        private readonly AppSettings _appSettings;

        public IssueEmployeeService(
            ILogger<IssueEmployeeService> logger,
            SeedApiContext context,
            IOptions<AppSettings> appSettings) {
            _logger = logger;
            _context = context;
            _appSettings = appSettings.Value;
        }

        public void Delete(object id) {
            try {
                IssueEmployee data = _context.IssueEmployee.Find(id);
                data.IsDelete = true;
                _context.SaveChanges();
            } catch (Exception ex) {
                throw ex;
            }
        }

        public void Delete(int[] list) {
            var query = from x in _context.IssueEmployee
                        where list.Contains(x.Id)
                        select x;
            foreach (var item in query) {
                item.IsDelete = true;
            }
            _context.SaveChanges();
        }

        public IssueEmployee Login(string account, string password, string ip) {
            IssueEmployee emp = null;
            try {
                emp = (from c in _context.IssueEmployee.Include(x => x.User)
                       where c.IsDelete == false && c.Account == account
                       select c).FirstOrDefault();
                if (emp != null && emp.Password == password) {
                    // authentication successful so generate jwt token
                    var user = emp.User;
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                    var tokenDescriptor = new SecurityTokenDescriptor {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, user.Id.ToString()),
                            new Claim(ClaimTypes.Role, Role.Admin),
                            new Claim(ClaimTypes.Role, Role.SuperAdmin),
                            new Claim("permissions", "['admin']")
                        }),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    emp.User.Token = tokenHandler.WriteToken(token);

                    _context.SaveChanges();

                } else {
                    return null;
                }
            } catch (Exception ex) {
                throw ex;
            }

            _context.Entry(emp).State = EntityState.Deleted;
            return emp.WithoutPassword();
        }

        public IssueEmployee GetById(int? id, bool deleteCheck = false) {
            var item = _context.IssueEmployee.AsNoTracking()
                            .Where(x => x.Id == id).FirstOrDefault();
            if (deleteCheck && item.IsDelete == true) return null;
            return item;
        }

        public IssueEmployee GetByAccount(string account) {
            IssueEmployee emp = null;
            emp = (from e in _context.IssueEmployee.AsNoTracking()
                   where e.IsDelete == false && e.Account == account
                   select e).FirstOrDefault();
            if (emp != null) {
                _context.Entry(emp).State = EntityState.Deleted;
            }
            return emp.WithoutPassword();
        }

        public void Save(IssueEmployee data) {
            var persist = _context.IssueEmployee
                            .Where(x => x.Id == data.Id).FirstOrDefault();
            if (persist == null) {
                var check = _context.IssueEmployee.Where(x => x.Account == data.Account && x.IsDelete == false).Count();
                if (check > 0) {
                    throw new ServiceException("아이디가 중복됩니다.");
                }
                data.User = new User {
                    Name = data.Name,
                };

                _context.IssueEmployee.Add(data);
                _context.SaveChanges();
            } else {
                persist.Name = data.Name;
                persist.Account = data.Account;
                persist.Password = data.Password;
                persist.EmployeeNo = data.EmployeeNo;
                persist.Tel = data.Tel;
                persist.Email = data.Email;
                //
                _context.SaveChanges();
            }
        }

        public IQueryable<IssueEmployee> GetAll() {
            var query = _context.IssueEmployee.Include(x => x.User).AsNoTracking();
            return query;
        }
    }
}