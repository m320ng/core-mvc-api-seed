using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IssueTracker.Entities;
using System;
using System.Linq;

namespace IssueTracker.Data {
    public static class SeedData {
        public static void Initialize(IServiceProvider serviceProvider) {
            using (var context = new IssueTrackerContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<IssueTrackerContext>>())) {

                // 데이터가 있다면 skip
                if (context.IssueEmployee.Any()) {
                    return;
                }

                context.IssueEmployee.AddRange(
                    new IssueEmployee {
                        Name = "김철수",
                        Account = "test",
                        Password = "test11",
                        Created = DateTime.Parse("1989-2-12"),
                    },
                    new IssueEmployee {
                        Name = "이방언",
                        Account = "bang",
                        Password = "bang11",
                        Created = DateTime.Parse("2002-2-12"),
                    },
                    new IssueEmployee {
                        Name = "관리자",
                        Account = "admin",
                        Password = "admin11",
                        Created = DateTime.Parse("2012-2-12"),
                    }

                );
                context.SaveChanges();
            }
        }
    }
}