using JWTAuth.WebApi.Interface;
using JWTAuth.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace JWTAuth.WebApi.Repository
{
    public class VentasRepository : IAVentas
    {
        readonly DatabaseContext _dbContext;

        public VentasRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Ventas> GetVentasDetails()
        {
            try
            {
                return _dbContext.Ventas.ToList();
            }
            catch
            {
                throw;
            }
        }

        public Ventas GetVentasDetails(int id)
        {
            try
            {
                Ventas? venta = _dbContext.Ventas.Find(id);
                if (venta != null)
                {
                    return venta;
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

        public void AddVentas(Ventas venta)
        {
            try
            {
                _dbContext.Ventas.Add(venta);
                _dbContext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void UpdateVentas(Ventas venta)
        {
            try
            {
                _dbContext.Entry(venta).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public Ventas DeleteVentas(int id)
        {
            try
            {
                Ventas? venta = _dbContext.Ventas.Find(id);

                if (venta != null)
                {
                    _dbContext.Ventas.Remove(venta);
                    _dbContext.SaveChanges();
                    return venta;
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

        public bool CheckVentas(int id)
        {
            return _dbContext.Ventas.Any(e => e.id == id);
        }
    }
}
