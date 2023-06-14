using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChallengeBackend.Data.Entities;

[Table("VentaDetalle")]
public partial class VentaDetalle
{
    [Key]
    [Column("ID_VentaDetalle")]
    public long IdVentaDetalle { get; set; }

    [Column("ID_Venta")]
    public long IdVenta { get; set; }

    [Column("Precio_Unitario")]
    public int PrecioUnitario { get; set; }

    public int Cantidad { get; set; }

    public int TotalLinea { get; set; }

    [Column("ID_Producto")]
    public long IdProducto { get; set; }

    [ForeignKey("IdProducto")]
    [InverseProperty("VentaDetalles")]
    public virtual Producto IdProductoNavigation { get; set; } = null!;

    [ForeignKey("IdVenta")]
    [InverseProperty("VentaDetalles")]
    public virtual Ventum IdVentaNavigation { get; set; } = null!;
}
