using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.SqlClient;

namespace API.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Client> Clients { get; set; }

        private SqlConnection connection;
        private Guid tenantId;

        private readonly IHttpContextAccessor httpContextAccessor;

        public DatabaseContext(DbContextOptions<DatabaseContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tenant>().ToTable("Tenant");
            modelBuilder.Entity<Client>().ToTable("Client");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=PLACEHOLDER;Trusted_Connection=True;MultipleActiveResultSets=true";

            connection = new SqlConnection(connectionString);
            connection.StateChange += Connection_StateChange;

            optionsBuilder.UseSqlServer(connection);

            base.OnConfiguring(optionsBuilder);
        }

        private void Connection_StateChange(object sender, StateChangeEventArgs e)
        {
            if (e.CurrentState == ConnectionState.Open)
            {
                if (httpContextAccessor.HttpContext.Items["TENANT"] != null)
                {
                    var tenant = (Tenant)httpContextAccessor.HttpContext.Items["TENANT"];
                    tenantId = tenant.TenantId;
                }

                var command = connection.CreateCommand();

                command.CommandText = @"exec sp_set_session_context @key=N'TenantId', @value=@TenantId";
                command.Parameters.AddWithValue("@TenantId", tenantId);

                command.ExecuteNonQuery();
            }
        }
    }
}
