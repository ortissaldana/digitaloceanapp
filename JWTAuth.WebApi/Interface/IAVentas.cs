using JWTAuth.WebApi.Models;
using System.Collections.Generic;

namespace JWTAuth.WebApi.Interface
{
    public interface IAVentas
    {
        List<Ventas> GetVentasDetails();

        Ventas GetVentasDetails(int id);

        void AddVentas(Ventas venta);

        bool CheckVentas(int id);
    }
}
