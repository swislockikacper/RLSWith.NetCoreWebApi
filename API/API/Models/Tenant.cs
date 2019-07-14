using System;

namespace API.Models
{
    public class Tenant
    {
        public Guid TenantId { get; set; }
        public Guid ApiKey { get; set; }
        public string Name { get; set; }
    }
}
