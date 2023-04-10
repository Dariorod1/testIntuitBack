using System;
using System.Collections.Generic;

namespace testIntuit.Models;

public partial class Cliente
{
    public int Id { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public DateTime? FechaNac { get; set; }

    public string Cuit { get; set; } = null!;

    public string? Domicilio { get; set; }

    public string Telefono { get; set; } = null!;

    public string Email { get; set; } = null!;
}
