using RateMe.Models.LocalDbModels;

namespace RateMe.Repositories;

public class ElementsRepository
{
    internal async Task Add(int subId, ElementLocal elem)
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
    
    internal async Task Add(int subId, IEnumerable<ElementLocal> elems)
    {
        await using SubjectsContext context = new();
        SubjectLocal? subject = await context.Subjects.FindAsync(subId);

        if (subject == null)
        {
            return;
        }

        foreach (ElementLocal elem in elems)
        {
            elem.ElementId = 0;
            subject.Elements.Add(elem);
        }
        
        await context.SaveChangesAsync();
    }
    
    internal async Task Remove(ElementLocal elem)
    {
        await using SubjectsContext context = new();
        context.Elements.Remove(elem);
        await context.SaveChangesAsync();
    }

    internal async Task Remove(IEnumerable<ElementLocal> elems)
    {
        await using SubjectsContext context = new();
        context.Elements.RemoveRange(elems);
        await context.SaveChangesAsync();
    }

    internal async Task UpdateRemoteKeys(Dictionary<int, int> localRemoteKeys)
    {
        await using SubjectsContext context = new();
        
        foreach (ElementLocal elem in context.Elements)
        {
            if (localRemoteKeys.ContainsKey(elem.ElementId))
            {
                elem.RemoteId = localRemoteKeys[elem.ElementId];
            }
        }

        await context.SaveChangesAsync();
    }
}