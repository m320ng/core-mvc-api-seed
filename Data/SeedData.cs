using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SeedApi.Entities;
using SeedApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeedApi.Data {
    public static class SeedData {
        public static void Initialize(IServiceProvider serviceProvider) {

            using (var context = new SeedApiContext(serviceProvider.GetRequiredService<DbContextOptions<SeedApiContext>>())) {

                // 데이터가 있다면 skip
                if (context.IssueEmployee.Any()) {
                    return;
                }

                Console.WriteLine("Seed Data..");

                var list = new List<IssueEmployee>();

                for (var i = 0; i <= 300; i++) {
                    list.AddRange(new IssueEmployee[] {
                        new IssueEmployee {
                            Name = "김철수"+(i>0?$"{i}":""),
                            Account = "test"+(i>0?$"{i}":""),
                            Password = "test11",
                            EmployeeNo = "123123",
                            Created = DateTime.Parse("1989-2-12"),
                            User = new User {
                                Name = "김철수"+(i>0?$"{i}":""),
                                Role = Role.Manager,
                            }
                        },
                        new IssueEmployee {
                            Name = "홍길동"+(i>0?$"{i}":""),
                            Account = "bang"+(i>0?$"{i}":""),
                            Password = "bang11",
                            EmployeeNo = "123123",
                            Created = DateTime.Parse("2002-2-12"),
                            User = new User {
                                Name = "홍길동"+(i>0?$"{i}":""),
                                Role = Role.User,
                            }
                        },
                        new IssueEmployee {
                            Name = "관리자"+(i>0?$"{i}":""),
                            Account = "admin"+(i>0?$"{i}":""),
                            Password = "admin11",
                            EmployeeNo = "123123",
                            Created = DateTime.Parse("2012-2-12"),
                            User = new User {
                                Name = "관리자"+(i>0?$"{i}":""),
                                Role = Role.Admin,
                            }
                        }
                    });
                }

                context.IssueEmployee.AddRange(list);
                context.SaveChanges();
            }
        }
    }
}