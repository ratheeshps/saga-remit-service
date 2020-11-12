using remitservice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace remitservice.Repository
{
    public interface IRemitterDataAccess
    {
        Task<List<RemitterModel>> GetRemitterList();
        Task<Guid> SaveRemitter(RemitterModel model);
        Task<bool> DeleteRemitter( Guid id);
        Task<bool> UpdateRemitter( Guid id,string status);
        Task<RemitterModel> GetRemitterById(Guid id);
    }
}
