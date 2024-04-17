using JWTAuth.WebApi.Interface;
using JWTAuth.WebApi.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace JWTAuth.WebApi.Repository
{
    public class VendedorRepository : IVendedor
    {
        readonly DatabaseContext _dbContext;

        public VendedorRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Vendedor> GetVendedorDetails()
        {
            try
            {
                return _dbContext.Vendedor.ToList();
            }
            catch
            {
                throw;
            }
        }

        public Vendedor GetVendedorDetails(int id)
        {
            try
            {
                Vendedor? vendedor = _dbContext.Vendedor.Find(id);
                if (vendedor != null)
                {
                    return vendedor;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch
            {
                throw;
            }
        }

        public void AddVendedor(Vendedor vendedor)
        {
            try
            {
                _dbContext.Vendedor.Add(vendedor);
                _dbContext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void UpdateVendedor(Vendedor vendedor)
        {
            try
            {
                _dbContext.Entry(vendedor).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public Vendedor DeleteVendedor(int id)
        {
            try
            {
                Vendedor? vendedor = _dbContext.Vendedor.Find(id);

                if (vendedor != null)
                {
                    _dbContext.Vendedor.Remove(vendedor);
                    _dbContext.SaveChanges();
                    return vendedor;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch
            {
                throw;
            }
        }
        public string CompararAcumulados(int vendedorId)
        {
            try
            {
                var result = _dbContext.Database.ExecuteSqlRaw("CompararAcumulados @VendedorID",
                    new SqlParameter("VendedorID", vendedorId) // Pasa el ID de vendedor como parámetro
                );

                // Asumiendo que 'CompararAcumulados' devuelve una cadena, podrías convertir el resultado a una cadena aquí.
                var resultadoComoCadena = result.ToString();
                return resultadoComoCadena;
            }
            catch (Exception ex)
            {
                // Manejar excepciones si es necesario
                throw;
            }
        }





        public bool CheckVendedor(int id)
        {
            return _dbContext.Vendedor.Any(e => e.id == id);
        }
    }
}
