using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace SeedApi.Models {
    public class PaginationList<T> {
        public int page { get; set; }
        public int total { get; set; }
        public int limit { get; set; }
        public int pages { get; set; }
        public ICollection<T> list { get; set; }

        public static PagingList Pagination(IQueryable<T> list, int page, int limit) {
            var pagedList = new PagingList();
            var skip = (page - 1) * limit;
            list = list.Take(page).Skip(skip);
            pagedList.total = list.Count();
            pagedList.list = list.ToList();
            pagedList.page = page;
            pagedList.pages = (int)Math.Ceiling((float)pagedList.total / (float)limit);
            pagedList.limit = limit;
            return pagedList;
        }
    }

    public class PagingList {
        public int page { get; set; }
        public int total { get; set; }
        public int limit { get; set; }
        public int pages { get; set; }
        public ICollection list { get; set; }

        public static PagingList Pagination(IQueryable<object> list, int page, int limit) {
            var pagedList = new PagingList();
            var skip = (page - 1) * limit;
            list = list.Take(page).Skip(skip);
            pagedList.total = list.Count();
            pagedList.list = list.ToList();
            pagedList.page = page;
            pagedList.pages = (int)Math.Ceiling((float)pagedList.total / (float)limit);
            pagedList.limit = limit;
            return pagedList;
        }

        public static PagingList Pagination<T>(IQueryable<T> list, int page, int limit) {
            var pagedList = new PagingList();
            var skip = (page - 1) * limit;
            pagedList.total = list.Count();
            list = list.Skip(skip).Take(limit);
            pagedList.list = list.ToList();
            pagedList.page = page;
            pagedList.pages = (int)Math.Ceiling((float)pagedList.total / (float)limit);
            pagedList.limit = limit;
            return pagedList;
        }
    }
}
