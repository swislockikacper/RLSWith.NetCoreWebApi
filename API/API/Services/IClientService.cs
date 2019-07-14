using API.Models;
using System;
using System.Collections.Generic;

namespace API.Services
{
    public interface IClientService
    {
        IEnumerable<Client> All();
        Client ById(Guid id);
        void Insert(Client client);
        void Update(Client client);
        void Delete(Guid id);
    }
}
