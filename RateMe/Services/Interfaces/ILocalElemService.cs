using RateMe.Models.LocalDbModels;

namespace RateMe.Services.Interfaces;

public interface ILocalElemService
{
    Task AddLocal(int subId, ElementLocal elem);
    Task AddLocals(int subId, IEnumerable<ElementLocal> elems);
    Task RemoveLocal(ElementLocal elem);
    Task RemoveLocals(IEnumerable<ElementLocal> elems);
}