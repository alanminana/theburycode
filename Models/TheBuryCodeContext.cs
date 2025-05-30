using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

public partial class TheBuryCodeContext : DbContext
{
    public TheBuryCodeContext()
    {
    }

    public TheBuryCodeContext(DbContextOptions<TheBuryCodeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AlertaLog> AlertaLogs { get; set; }

    public virtual DbSet<Alertum> Alerta { get; set; }

    public virtual DbSet<ArchivoCredito> ArchivoCreditos { get; set; }

    public virtual DbSet<Banco> Bancos { get; set; }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<Ciudad> Ciudads { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<CompraDetalle> CompraDetalles { get; set; }

    public virtual DbSet<Configuracion> Configuracions { get; set; }

    public virtual DbSet<Conyuge> Conyuges { get; set; }

    public virtual DbSet<Cotizacion> Cotizacions { get; set; }

    public virtual DbSet<CotizacionDetalle> CotizacionDetalles { get; set; }

    public virtual DbSet<Cuota> Cuota { get; set; }

    public virtual DbSet<DocumentoCliente> DocumentoClientes { get; set; }

    public virtual DbSet<DomicilioLaboral> DomicilioLaborals { get; set; }

    public virtual DbSet<DomicilioParticular> DomicilioParticulars { get; set; }

    public virtual DbSet<EstadoCivil> EstadoCivils { get; set; }

    public virtual DbSet<EstadoCompra> EstadoCompras { get; set; }

    public virtual DbSet<EstadoCuota> EstadoCuota { get; set; }

    public virtual DbSet<EstadoEntrega> EstadoEntregas { get; set; }

    public virtual DbSet<EstadoProducto> EstadoProductos { get; set; }

    public virtual DbSet<EstadoRma> EstadoRmas { get; set; }

    public virtual DbSet<EstadoRmadetalle> EstadoRmadetalles { get; set; }

    public virtual DbSet<EstadoSolicitudCredito> EstadoSolicitudCreditos { get; set; }

    public virtual DbSet<EstadoUsuario> EstadoUsuarios { get; set; }

    public virtual DbSet<FormaPago> FormaPagos { get; set; }

    public virtual DbSet<Garante> Garantes { get; set; }

    public virtual DbSet<Genero> Generos { get; set; }

    public virtual DbSet<LineaCredito> LineaCreditos { get; set; }

    public virtual DbSet<Marca> Marcas { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<PrecioLog> PrecioLogs { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Proveedor> Proveedors { get; set; }

    public virtual DbSet<ProveedorProducto> ProveedorProductos { get; set; }

    public virtual DbSet<Provincium> Provincia { get; set; }

    public virtual DbSet<Rmacliente> Rmaclientes { get; set; }

    public virtual DbSet<RmaclienteDetalle> RmaclienteDetalles { get; set; }

    public virtual DbSet<Rmaproveedor> Rmaproveedors { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<RolPermiso> RolPermisos { get; set; }

    public virtual DbSet<ScoringLog> ScoringLogs { get; set; }

    public virtual DbSet<SolicitudCredito> SolicitudCreditos { get; set; }

    public virtual DbSet<StockLog> StockLogs { get; set; }

    public virtual DbSet<SysModulo> SysModulos { get; set; }

    public virtual DbSet<TipoRmacliente> TipoRmaclientes { get; set; }

    public virtual DbSet<TipoTarjetum> TipoTarjeta { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuarioRol> UsuarioRols { get; set; }

    public virtual DbSet<VentaDetalle> VentaDetalles { get; set; }

    public virtual DbSet<Venta> Venta { get; set; }

    public virtual DbSet<VwProductosPreciosCalculado> VwProductosPreciosCalculados { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=theburycode;User Id=sa;Password=Kirishima1!;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AlertaLog>(entity =>
        {
            entity.Property(e => e.Estado).IsFixedLength();
            entity.Property(e => e.FechaGeneracion).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Alerta).WithMany(p => p.AlertaLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AlertaLog_Alerta");
        });

        modelBuilder.Entity<Alertum>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<ArchivoCredito>(entity =>
        {
            entity.Property(e => e.FechaCarga).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.SolicitudCredito).WithMany(p => p.ArchivoCreditos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ArchivoCredito_Solicitud");
        });

        modelBuilder.Entity<Banco>(entity =>
        {
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Tipo)
                .HasDefaultValue("R")
                .IsFixedLength();

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("FK_Categoria_Parent");
        });

        modelBuilder.Entity<Ciudad>(entity =>
        {
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Provincia).WithMany(p => p.Ciudads)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ciudad_Provincia");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.Property(e => e.EstadoCivil).IsFixedLength();
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Genero).IsFixedLength();

            entity.HasOne(d => d.Ciudad).WithMany(p => p.Clientes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cliente_Ciudad");

            entity.HasOne(d => d.EstadoCivilNavigation).WithMany(p => p.Clientes).HasConstraintName("FK_Cliente_EstadoCivil");

            entity.HasOne(d => d.GeneroNavigation).WithMany(p => p.Clientes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cliente_Genero");
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.Property(e => e.Estado).IsFixedLength();
            entity.Property(e => e.Fecha).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Banco).WithMany(p => p.Compras).HasConstraintName("FK_Compra_Banco");

            entity.HasOne(d => d.EstadoNavigation).WithMany(p => p.Compras).HasConstraintName("FK_Compra_Estado");

            entity.HasOne(d => d.FormaPago).WithMany(p => p.Compras)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Compra_FormaPago");

            entity.HasOne(d => d.Proveedor).WithMany(p => p.Compras)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Compra_Proveedor");

            entity.HasOne(d => d.TipoTarjeta).WithMany(p => p.Compras).HasConstraintName("FK_Compra_TipoTarjeta");
        });

        modelBuilder.Entity<CompraDetalle>(entity =>
        {
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Subtotal).HasComputedColumnSql("([Cantidad]*[PrecioUnitario])", true);

            entity.HasOne(d => d.Compra).WithMany(p => p.CompraDetalles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompraDetalle_Compra");

            entity.HasOne(d => d.Producto).WithMany(p => p.CompraDetalles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompraDetalle_Producto");
        });

        modelBuilder.Entity<Configuracion>(entity =>
        {
            entity.HasOne(d => d.Modulo).WithMany(p => p.Configuracions).HasConstraintName("FK_Configuracion_Modulo");
        });

        modelBuilder.Entity<Conyuge>(entity =>
        {
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Genero).IsFixedLength();

            entity.HasOne(d => d.Cliente).WithMany(p => p.Conyuges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Conyuge_Cliente");

            entity.HasOne(d => d.GeneroNavigation).WithMany(p => p.Conyuges).HasConstraintName("FK_Conyuge_Genero");
        });

        modelBuilder.Entity<Cotizacion>(entity =>
        {
            entity.Property(e => e.Fecha).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Cotizacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cotizacion_Cliente");
        });

        modelBuilder.Entity<CotizacionDetalle>(entity =>
        {
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Subtotal).HasComputedColumnSql("([Cantidad]*[PrecioUnit])", true);

            entity.HasOne(d => d.Cotizacion).WithMany(p => p.CotizacionDetalles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CotizacionDetalle_Cotizacion");

            entity.HasOne(d => d.Producto).WithMany(p => p.CotizacionDetalles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CotizacionDetalle_Producto");
        });

        modelBuilder.Entity<Cuota>(entity =>
        {
            entity.Property(e => e.Estado)
                .HasDefaultValue("P")
                .IsFixedLength();
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.MontoTotal).HasComputedColumnSql("(([MontoCapital]+[MontoInteres])+[MontoMora])", true);

            entity.HasOne(d => d.EstadoNavigation).WithMany(p => p.Cuota)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuota_Estado");

            entity.HasOne(d => d.LineaCredito).WithMany(p => p.Cuota)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuota_LineaCredito");
        });

        modelBuilder.Entity<DocumentoCliente>(entity =>
        {
            entity.Property(e => e.FechaCarga).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Cliente).WithMany(p => p.DocumentoClientes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DocumentoCliente_Cliente");
        });

        modelBuilder.Entity<DomicilioLaboral>(entity =>
        {
            entity.Property(e => e.ClienteId).ValueGeneratedNever();
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.CiudadLaboral).WithMany(p => p.DomicilioLaborals)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DomicilioLaboral_Ciudad");

            entity.HasOne(d => d.Cliente).WithOne(p => p.DomicilioLaboral)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DomicilioLaboral_Cliente");
        });

        modelBuilder.Entity<DomicilioParticular>(entity =>
        {
            entity.Property(e => e.ClienteId).ValueGeneratedNever();
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Cliente).WithOne(p => p.DomicilioParticular)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DomicilioParticular_Cliente");
        });

        modelBuilder.Entity<EstadoCivil>(entity =>
        {
            entity.Property(e => e.Codigo).IsFixedLength();
        });

        modelBuilder.Entity<EstadoCompra>(entity =>
        {
            entity.Property(e => e.Codigo).IsFixedLength();
        });

        modelBuilder.Entity<EstadoCuota>(entity =>
        {
            entity.Property(e => e.Codigo).IsFixedLength();
        });

        modelBuilder.Entity<EstadoEntrega>(entity =>
        {
            entity.Property(e => e.Codigo).IsFixedLength();
        });

        modelBuilder.Entity<EstadoProducto>(entity =>
        {
            entity.Property(e => e.Codigo).IsFixedLength();
        });

        modelBuilder.Entity<EstadoRma>(entity =>
        {
            entity.Property(e => e.Codigo).IsFixedLength();
        });

        modelBuilder.Entity<EstadoRmadetalle>(entity =>
        {
            entity.Property(e => e.Codigo).IsFixedLength();
        });

        modelBuilder.Entity<EstadoSolicitudCredito>(entity =>
        {
            entity.Property(e => e.Codigo).IsFixedLength();
        });

        modelBuilder.Entity<EstadoUsuario>(entity =>
        {
            entity.Property(e => e.Codigo).IsFixedLength();
        });

        modelBuilder.Entity<FormaPago>(entity =>
        {
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<Garante>(entity =>
        {
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Genero).IsFixedLength();

            entity.HasOne(d => d.Cliente).WithMany(p => p.Garantes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Garante_Cliente");

            entity.HasOne(d => d.GeneroNavigation).WithMany(p => p.Garantes).HasConstraintName("FK_Garante_Genero");
        });

        modelBuilder.Entity<Genero>(entity =>
        {
            entity.Property(e => e.Codigo).IsFixedLength();
        });

        modelBuilder.Entity<LineaCredito>(entity =>
        {
            entity.Property(e => e.Estado)
                .HasDefaultValue("A")
                .IsFixedLength();
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.SolicitudCredito).WithMany(p => p.LineaCreditos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LineaCredito_SolicitudCredito");
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Tipo)
                .HasDefaultValue("M")
                .IsFixedLength();

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("FK_Marca_Parent");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<PrecioLog>(entity =>
        {
            entity.Property(e => e.FechaCambio).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Producto).WithMany(p => p.PrecioLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrecioLog_Producto");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.EstadoProducto)
                .HasDefaultValue("A")
                .IsFixedLength();
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IvaPct).HasDefaultValue(2100m);
            entity.Property(e => e.StockActual).HasDefaultValue(0);
            entity.Property(e => e.StockMinimo).HasDefaultValue(0);

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_Categoria");

            entity.HasOne(d => d.EstadoProductoNavigation).WithMany(p => p.Productos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_Estado");

            entity.HasOne(d => d.Marca).WithMany(p => p.ProductoMarcas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_Marca");

            entity.HasOne(d => d.Submarca).WithMany(p => p.ProductoSubmarcas).HasConstraintName("FK_Producto_Submarca");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<ProveedorProducto>(entity =>
        {
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Producto).WithMany(p => p.ProveedorProductos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProveedorProducto_Producto");

            entity.HasOne(d => d.Proveedor).WithMany(p => p.ProveedorProductos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProveedorProducto_Proveedor");
        });

        modelBuilder.Entity<Provincium>(entity =>
        {
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<Rmacliente>(entity =>
        {
            entity.Property(e => e.Estado).IsFixedLength();
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.FechaSolicitud).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Tipo).IsFixedLength();

            entity.HasOne(d => d.EstadoNavigation).WithMany(p => p.Rmaclientes).HasConstraintName("FK_RMACliente_Estado");

            entity.HasOne(d => d.TipoNavigation).WithMany(p => p.Rmaclientes).HasConstraintName("FK_RMACliente_Tipo");

            entity.HasOne(d => d.Venta).WithMany(p => p.Rmaclientes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RMACliente_Venta");
        });

        modelBuilder.Entity<RmaclienteDetalle>(entity =>
        {
            entity.Property(e => e.Estado).IsFixedLength();
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.EstadoNavigation).WithMany(p => p.RmaclienteDetalles).HasConstraintName("FK_RMAClienteDetalle_Estado");

            entity.HasOne(d => d.Producto).WithMany(p => p.RmaclienteDetalles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RMAClienteDetalle_Producto");

            entity.HasOne(d => d.Rmacliente).WithMany(p => p.RmaclienteDetalles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RMAClienteDetalle_RMA");
        });

        modelBuilder.Entity<Rmaproveedor>(entity =>
        {
            entity.Property(e => e.Estado).IsFixedLength();
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.EstadoNavigation).WithMany(p => p.Rmaproveedors).HasConstraintName("FK_RMAProveedor_Estado");

            entity.HasOne(d => d.Producto).WithMany(p => p.Rmaproveedors)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RMAProveedor_Producto");

            entity.HasOne(d => d.Proveedor).WithMany(p => p.Rmaproveedors)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RMAProveedor_Proveedor");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<RolPermiso>(entity =>
        {
            entity.Property(e => e.FechaAsignacion).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Permiso).WithMany(p => p.RolPermisos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolPermiso_Permiso");

            entity.HasOne(d => d.Rol).WithMany(p => p.RolPermisos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolPermiso_Rol");
        });

        modelBuilder.Entity<ScoringLog>(entity =>
        {
            entity.Property(e => e.FechaCambio).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Cliente).WithMany(p => p.ScoringLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ScoringLog_Cliente");
        });

        modelBuilder.Entity<SolicitudCredito>(entity =>
        {
            entity.Property(e => e.Estado).IsFixedLength();
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.FechaSolicitud).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Cliente).WithMany(p => p.SolicitudCreditos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudCredito_Cliente");

            entity.HasOne(d => d.EstadoNavigation).WithMany(p => p.SolicitudCreditos).HasConstraintName("FK_SolicitudCredito_Estado");
        });

        modelBuilder.Entity<StockLog>(entity =>
        {
            entity.Property(e => e.FechaMovimiento).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Producto).WithMany(p => p.StockLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockLog_Producto");
        });

        modelBuilder.Entity<TipoRmacliente>(entity =>
        {
            entity.Property(e => e.Codigo).IsFixedLength();
        });

        modelBuilder.Entity<TipoTarjetum>(entity =>
        {
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.Property(e => e.Estado).IsFixedLength();
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.EstadoNavigation).WithMany(p => p.Usuarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Estado");
        });

        modelBuilder.Entity<UsuarioRol>(entity =>
        {
            entity.Property(e => e.FechaAsignacion).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Rol).WithMany(p => p.UsuarioRols)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsuarioRol_Rol");

            entity.HasOne(d => d.Usuario).WithMany(p => p.UsuarioRols)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsuarioRol_Usuario");
        });

        modelBuilder.Entity<VentaDetalle>(entity =>
        {
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Subtotal).HasComputedColumnSql("([Cantidad]*[PrecioUnit])", true);

            entity.HasOne(d => d.Producto).WithMany(p => p.VentaDetalles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VentaDetalle_Producto");

            entity.HasOne(d => d.Venta).WithMany(p => p.VentaDetalles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VentaDetalle_Venta");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.Property(e => e.EstadoEntrega).IsFixedLength();
            entity.Property(e => e.Fecha).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Banco).WithMany(p => p.Venta).HasConstraintName("FK_Venta_Banco");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Venta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Venta_Cliente");

            entity.HasOne(d => d.EstadoEntregaNavigation).WithMany(p => p.Venta).HasConstraintName("FK_Venta_EstadoEntrega");

            entity.HasOne(d => d.FormaPago).WithMany(p => p.Venta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Venta_FormaPago");

            entity.HasOne(d => d.TipoTarjeta).WithMany(p => p.Venta).HasConstraintName("FK_Venta_TipoTarjeta");
        });

        modelBuilder.Entity<VwProductosPreciosCalculado>(entity =>
        {
            entity.ToView("vw_ProductosPreciosCalculados");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
