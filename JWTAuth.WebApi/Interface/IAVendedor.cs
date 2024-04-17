using JWTAuth.WebApi.Models;
using System.Collections.Generic;

namespace JWTAuth.WebApi.Interface
{
    public interface IVendedor
    {
        public List<Vendedor> GetVendedorDetails();

        public Vendedor GetVendedorDetails(int id);

        public void AddVendedor(Vendedor vendedor);

        public bool CheckVendedor(int id);
        public string CompararAcumulados(int vendedorId);

    }
}
