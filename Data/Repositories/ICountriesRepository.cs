using System.Collections.Generic;
using WeighForce.Models;

namespace WeighForce.Data.Repositories
{
    public interface ICountriesRepository
    {
        IEnumerable<Country> GetAllCountries();
        Country GetCountry(long id);
        bool Delete(long id, string userId);
        Country AddCountry(Country country);
        Country UpdateCountry(Country country);
    }
}