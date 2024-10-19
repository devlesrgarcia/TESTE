using APIUsuario.Models;
using Microsoft.EntityFrameworkCore;

namespace APIUsuario.Context;

public class APIUsuarioContext : DbContext
{
	public APIUsuarioContext(DbContextOptions<APIUsuarioContext> options) : base(options)
	{
	}
	public DbSet<Usuario>? Usuarios { get; set; }
}
