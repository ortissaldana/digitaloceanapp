namespace JWTAuth.WebApi.Models
{
    public class Tarjetas
    {
        public int id { get; set; }
        public int? tarjeta { get; set; }
        public decimal? saldo { get; set; }
        public int? uso { get; set; }

    }
}
