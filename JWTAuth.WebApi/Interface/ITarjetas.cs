using JWTAuth.WebApi.Models;
using System.Collections.Generic;

namespace JWTAuth.WebApi.Interface
{
    public interface ITarjetas
    {
        public List<Tarjetas> GetTarjetasDetails();

        public Tarjetas GetTarjetasDetails(int id);

        public void AddTarjetas(Tarjetas vendedor);

        public bool CheckTarjetas(int id);
    }
}
