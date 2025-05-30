using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec
{
    public class AllPropMecPossibleCorrs
    {
        private readonly SortedList<string, PropMecPossibleCorrsPerLitoGroup> _corrs;

        public IList<PropMecPossibleCorrsPerLitoGroup> PossibleCorrs => _corrs.Values;

        public AllPropMecPossibleCorrs()
        {
            _corrs = new SortedList<string, PropMecPossibleCorrsPerLitoGroup>();
        }

        public bool AddPropMecPossibleCorrs(PropMecPossibleCorrsPerLitoGroup possibleCorrs)
        {
            if (_corrs.ContainsKey(possibleCorrs.GrupoLitologico.Nome))
                return false;

            _corrs.Add(possibleCorrs.GrupoLitologico.Nome, possibleCorrs);
            return true;
        }
    }
}
