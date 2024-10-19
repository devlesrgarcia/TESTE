using System.ComponentModel.DataAnnotations;

namespace APIUsuario.Models;

public class Usuario
{

    public int Usuarioid { get; set; }
    [StringLength(100)]
    public string? Nome { get; set; }
    [StringLength(14)]
    public string? Telefone { get; set; }
    [StringLength(300)]
    public string? Rua { get; set; }
    [StringLength(300)]
    public string? Cidade { get; set; }
    [StringLength(100)]
    public string? Estado { get; set; }
}
