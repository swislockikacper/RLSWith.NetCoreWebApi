using System;

namespace API.Models
{
    public class Client
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string FullName { get; set; }
        public short Age { get; set;
        }
    }
}
