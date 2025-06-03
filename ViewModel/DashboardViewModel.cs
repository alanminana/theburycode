// ViewModels/DashboardViewModel.cs
namespace theburycode.ViewModels
{
    public class DashboardViewModel
    {
        // Resumen general
        public int TotalClientes { get; set; }
        public int ClientesNuevosMes { get; set; }
        public int TotalProductos { get; set; }
        public int ProductosStockBajo { get; set; }

        // Ventas
        public decimal VentasHoy { get; set; }
        public decimal VentasSemana { get; set; }
        public decimal VentasMes { get; set; }
        public int CantidadVentasHoy { get; set; }

        // Créditos
        public int CreditosPendientes { get; set; }
        public int CuotasVencerSemana { get; set; }
        public decimal MontoTotalCreditos { get; set; }

        // Top productos
        public List<TopProductoViewModel> TopProductos { get; set; } = new();

        // Últimas operaciones
        public List<UltimaOperacionViewModel> UltimasVentas { get; set; } = new();

        // Alertas
        public List<AlertaViewModel> Alertas { get; set; } = new();
    }

    public class TopProductoViewModel
    {
        public string Nombre { get; set; } = string.Empty;
        public int CantidadVendida { get; set; }
        public decimal MontoTotal { get; set; }
    }

    public class UltimaOperacionViewModel
    {
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
    }

    public class AlertaViewModel
    {
        public string Tipo { get; set; } = string.Empty; // "warning", "danger", "info"
        public string Mensaje { get; set; } = string.Empty;
        public string? Url { get; set; }
    }
    public class WidgetDto
    {
        public string Nombre { get; set; } = string.Empty;
    }

    public class AlertaDto
    {
        public string Tipo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }

    public class NotificacionDto
    {
        public string Tipo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }
}