namespace BundleCounter.Repository
{
    public class BaseRepository<T>
    {
        protected AppDBContext dbContext;

        public BaseRepository(AppDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Insert(T entity)
        {
            if (entity == null) return;
            dbContext.Add(entity);
            dbContext.SaveChanges();
        }

        public void InsertAll(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                if (entity != null) dbContext.Add(entity);
            }
            dbContext.SaveChanges();
        }

        public void Update(T item)
        {
            if (item == null) return;
            dbContext.Update(item);
            dbContext.SaveChanges();
        }

        public void Remove(T item)
        {
            if (item == null) return;
            dbContext.Remove(item);
            dbContext.SaveChanges();
        }
        public void RemoveAll(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                if (entity != null) dbContext.Remove(entity);
            }
            dbContext.SaveChanges();
        }
    }
}
