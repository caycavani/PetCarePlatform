using System;
using System.Collections.Generic;
using PetCare.Auth.Domain.Entities;

namespace PetCare.Booking.Tests.Seeders
{
    public static class UserSeeder
    {
        public static List<User> GetSeededUsers()
        {
            // Roles comunes
            var clienteRole = new Role("cliente");
            var cuidadorRole = new Role("cuidador");

            var users = new List<User>();

            // 👤 Cliente
            var cliente = new User(
                "cliente@test.com",
                "", // contraseña temporal
                "cliente_carlos",
                "Carlos Cliente",
                "3000000001",
                clienteRole.Id
            );
            cliente.SetPassword("Cliente123!"); // 🔐 Hash seguro
            cliente.AssignRole(clienteRole);
            users.Add(cliente);

            // 👤 Cuidador
            var cuidador = new User(
                "cuidador@test.com",
                "",
                "cuidador_daniel",
                "Daniel Cuidador",
                "3000000002",
                cuidadorRole.Id
            );
            cuidador.SetPassword("Cuidador123!");
            cuidador.AssignRole(cuidadorRole);
            users.Add(cuidador);

            return users;
        }

        public static List<Role> GetRoles()
        {
            return new List<Role>
            {
                new Role("cliente"),
                new Role("cuidador"),
                new Role("admin")
            };
        }
    }
}
