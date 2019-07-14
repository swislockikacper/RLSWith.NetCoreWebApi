using API.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Services
{
    public class ClientService : IClientService
    {
        private readonly DatabaseContext dbContext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ClientService(DatabaseContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            this.dbContext = dbContext;
            this.httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<Client> All() => dbContext.Clients.ToList();

        public Client ById(Guid id) => dbContext.Clients
            .Where(c => c.Id == id)
            .FirstOrDefault();

        public void Delete(Guid id)
        {
            var clientToDelete = dbContext.Clients
               .Where(c => c.Id == id)
               .FirstOrDefault();

            dbContext.Clients.Remove(clientToDelete);
            dbContext.SaveChanges();
        }

        public void Insert(Client client)
        {
            var tenant = (Tenant)httpContextAccessor.HttpContext.Items["TENANT"];

            client.Id = Guid.NewGuid();
            client.TenantId = tenant.TenantId;

            dbContext.Clients.Add(client);
            dbContext.SaveChanges();
        }

        public void Update(Client client)
        {
            var clientToUpdate = dbContext.Clients
                .Where(c => c.Id == client.Id)
                .FirstOrDefault();

            clientToUpdate.FullName = client.FullName;
            clientToUpdate.Age = client.Age;

            dbContext.SaveChanges();
        }
    }
}
