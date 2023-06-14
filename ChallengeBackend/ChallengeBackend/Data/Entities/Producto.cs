using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChallengeBackend.Data.Entities;

[Table("Producto")]
public partial class Producto
{
    [Key]
    [Column("ID_Producto")]
    public long IdProducto { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string Codigo { get; set; } = null!;

    [Column("ID_Marca")]
    public long IdMarca { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Modelo { get; set; } = null!;

    [Column("Costo_Unitario")]
    public int CostoUnitario { get; set; }

    [ForeignKey("IdMarca")]
    [InverseProperty("Productos")]
    public virtual Marca IdMarcaNavigation { get; set; } = null!;

    [InverseProperty("IdProductoNavigation")]
    public virtual ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
}
