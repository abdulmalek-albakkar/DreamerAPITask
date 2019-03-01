using Dreamer.SqlServer.Database;
using System;

namespace Dreamer.SharedContext.Repository
{
    public class DreamerRepository : IDisposable
    {
        public DreamerDbContext DreamerDbContext { get; }
        public DreamerRepository()
        {

        }
        public DreamerRepository(DreamerDbContext dreamerDbContext)
        {
            DreamerDbContext = dreamerDbContext;
        }
        public void Dispose()
        {
            if (DreamerDbContext != null)
            {
                try
                {
                    DreamerDbContext.Dispose();
                }
                catch { }
            }
        }
    }
}
