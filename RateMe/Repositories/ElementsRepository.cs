using RateMe.Models.LocalDbModels;

namespace RateMe.Repositories;

public class ElementsRepository
{
    internal async Task Add(int subId, ControlElementLocal elem)
    {
        await using SubjectsContext context = new();
        SubjectLocal? subj = await context.Subjects.FindAsync(subId);
        
        if (subj == null)
        {
            return;
        }
        
        subj.Elements.Add(elem);
        await context.SaveChangesAsync();
    }
}