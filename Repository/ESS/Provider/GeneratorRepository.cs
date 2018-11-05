using System;
using Repository.ESS.Domain;
using Support.EntityFramework;

namespace Repository.ESS.Provider
{
    public class GeneratorRepository : GenericRepository<Generator>
    {
        public GeneratorRepository() : base(new ESSContext()) { }

        public override void Create(Generator generator)
        {
            if (generator == null)
            {
                throw new ArgumentNullException();
            }

            base.Create(generator);
        }
    }
}
