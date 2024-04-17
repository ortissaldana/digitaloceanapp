namespace JWTAuth.WebApi.Models
{
    public class Ventas
    {
        public int id { get; set; }
        public int? vendedor_id { get; set; }
        public int? tarjeta_id { get; set; }
        public DateTime? hora { get; set; }

    }
}
