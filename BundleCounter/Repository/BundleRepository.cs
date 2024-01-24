using BundleCounter.Models.Requests;

namespace BundleCounter.Repository
{
    public class BundleRepository : BaseRepository<Bundle>
    {
        public BundleRepository(AppDBContext dbContext) : base(dbContext)
        {
            
        }

        public IQueryable<Bundle> FindAll()
        {
            return dbContext.Bundles;
        }

        public Bundle? FindBy(int id)
        {
            return dbContext.Bundles.SingleOrDefault(u => u.Id == id);
        }
    }
}
