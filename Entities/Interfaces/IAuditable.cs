using System;
using SeedApi.Entities;

namespace SeedApi.Entities.Interfaces {
    public interface IAuditable {
        public int? CreateById { get; set; }
        public int? UpdateById { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
