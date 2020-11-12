using remitservice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace remitservice.Repository
{
    public class RemitterData : IRemitterDataAccess
    {
        public Task<bool> DeleteRemitter(Guid guid)
        {
            try
            {
                using (var db = new SymexDbContext())
                {
                    var remitter = db.remitterData.FirstOrDefault(x => x.ID == guid);
                    remitter.Status = "InActive";
                    db.Update<RemitterModel>(remitter);
                    db.SaveChanges();
                    return Task.Run(()=>true);
                }
            }
            catch (Exception)
            {

                    return Task.Run(()=>false);
            }
         
        }

        public Task<RemitterModel> GetRemitterById(Guid id)
        {
            using (var db = new SymexDbContext())
            {
                return Task.Run(()=>db.remitterData.FirstOrDefault(x=>x.ID==id && x.Status=="Active"));
            }
        }

        public Task<bool> UpdateRemitter(Guid id, string status)
        {
            try
            {
                using (var db = new SymexDbContext())
                {
                    var remitter = db.remitterData.FirstOrDefault(x => x.ID == id);
                    remitter.Status =status;
                    db.Update<RemitterModel>(remitter);
                    db.SaveChanges();
                    return Task.Run(() => true);
                }
            }
            catch (Exception)
            {

                return Task.Run(() => false);
            }
        }

        Task< List<RemitterModel>> IRemitterDataAccess.GetRemitterList()
        {
           using (var db=new SymexDbContext())
            {
                return Task.Run(()=>db.remitterData.Where(x=>x.Status=="Active").ToList());
            }
        }

        Task<Guid> IRemitterDataAccess.SaveRemitter(RemitterModel model)
        {
            using (var db = new SymexDbContext())
            {
                db.Add<RemitterModel>(model);
                db.SaveChanges();
                return Task.Run(() => model.ID);
            }
        }
    }
}
