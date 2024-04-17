using JWTAuth.WebApi.Models;
using System.Collections.Generic;

namespace JWTAuth.WebApi.Interface
{
    public interface IAlertas
    {
        public List<Alerta> GetAlertaDetails();

        public Alerta GetAlertaDetails(int id);

        public void AddAlerta(Alerta vendedor);

        public bool CheckAlerta(int id);
    }
}
