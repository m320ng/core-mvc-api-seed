

using System.Collections.Generic;

namespace SeedApi.Models {


    public class PagingRequest {
        public int page { get; set; } = 1;
        public int limit { get; set; } = 20;
        public Sort sort { get; set; }
        public Condition[] conditions { get; set; }

        public class Sort {
            public string field { get; set; }
            public string dir { get; set; }
        }
        public class Condition {
            public string field { get; set; }
            public string op { get; set; }
            public string value { get; set; }
        }
    }

}