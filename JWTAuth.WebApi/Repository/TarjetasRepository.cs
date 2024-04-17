using JWTAuth.WebApi.Interface;
using JWTAuth.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace JWTAuth.WebApi.Repository
{
    public class TarjetaRepository :ITarjetas
    {
        readonly DatabaseContext _dbContext;

        public TarjetaRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Tarjetas> GetTarjetasDetails()
        {
            try
            {
                return _dbContext.Tarjetas.ToList();
            }
            catch
            {
                throw;
            }
        }

        public Tarjetas GetTarjetasDetails(int id)
        {
            try
            {
                Tarjetas? tarjeta = _dbContext.Tarjetas.Find(id);
                if (tarjeta != null)
                {
                    return tarjeta;
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

        public void AddTarjetas(Tarjetas tarjeta)
        {
            try
            {
                _dbContext.Tarjetas.Add(tarjeta);
                _dbContext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void UpdateTarjetas(Tarjetas tarjeta)
        {
            try
            {
                _dbContext.Entry(tarjeta).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public Tarjetas DeleteTarjetas(int id)
        {
            try
            {
                Tarjetas? tarjeta = _dbContext.Tarjetas.Find(id);

                if (tarjeta != null)
                {
                    _dbContext.Tarjetas.Remove(tarjeta);
                    _dbContext.SaveChanges();
                    return tarjeta;
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

        public bool CheckTarjetas(int id)
        {
            return _dbContext.Tarjetas.Any(e => e.id == id);
        }
    }
}
