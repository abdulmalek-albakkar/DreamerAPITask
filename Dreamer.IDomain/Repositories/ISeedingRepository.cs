namespace Dreamer.IDomain.Repositories
{
    public interface ISeedingRepository
    {
        /// <summary>
        /// Clear all database main tables data
        /// </summary>
        /// <returns>Succeeded or not</returns>
        bool Clear();

        /// <summary>
        /// Seed random data with random names and values which includes (Products, Categories, ProductCategories, and 2 users [customer + admin])
        /// </summary>
        /// <returns>Succeeded or not</returns>
        bool Seed();
    }
}
