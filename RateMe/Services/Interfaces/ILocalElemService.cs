using RateMe.Models.LocalDbModels;

namespace RateMe.Services.Interfaces;

public interface ILocalElemService
{
    Task AddLocal(int subId, ControlElementLocal elem);
    Task AddLocals(int subId, IEnumerable<ControlElementLocal> elems);
    Task RemoveLocal(ControlElementLocal elem);
    Task RemoveLocals(IEnumerable<ControlElementLocal> elems);
}