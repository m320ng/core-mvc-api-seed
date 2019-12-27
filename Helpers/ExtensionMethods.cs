using System.Collections.Generic;
using System.Linq;
using SeedApi.Entities;

namespace SeedApi.Helpers {
    public static class ExtensionMethods {
        public static IEnumerable<IssueEmployee> WithoutPasswords(this IEnumerable<IssueEmployee> users) {
            if (users == null) return null;

            return users.Select(x => x.WithoutPassword());
        }

        public static IssueEmployee WithoutPassword(this IssueEmployee user) {
            if (user == null) return null;

            user.Password = null;
            return user;
        }
    }
}