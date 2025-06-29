using System.Windows;
using Microsoft.EntityFrameworkCore;
using RateMe.Models.LocalDbModels;
using RateMeShared.Dto;

namespace RateMe.Repositories;

public class SubjectsRepository
{
    internal async Task<SubjectLocal[]> GetSubjectsNoRemote()
    {
        await using SubjectsContext context = new();
        SubjectLocal[] subsNoRemote = await context.Subjects.Include(s => s.Elements)
                                                      .Where(s => s.RemoteId == 0).ToArrayAsync();

        return subsNoRemote;
    }

    
    internal async Task<List<SubjectLocal>> GetAll()
    {
        await using SubjectsContext context = new();
        List<SubjectLocal> subjects = await context.Subjects.Include(s => s.Elements).ToListAsync();
        return subjects;
    }

    
    internal async Task Add(IEnumerable<SubjectLocal> subjs)
    {
        await using SubjectsContext context = new();
        context.Subjects.AddRange(subjs);
        await context.SaveChangesAsync();
    }

    
    internal async Task Add(SubjectLocal subj)
    {
        await using SubjectsContext context = new();
        context.Add(subj);
        await context.SaveChangesAsync();
    }
    
    
    internal async Task Remove(IEnumerable<SubjectLocal> subjs)
    {
        await using SubjectsContext context = new();
        context.Subjects.RemoveRange(subjs);
        await context.SaveChangesAsync();
    }
    
    
    internal async Task Remove(SubjectLocal subj)
    {
        await using SubjectsContext context = new();
        context.Subjects.Remove(subj);
        await context.SaveChangesAsync();
    }

    
    internal async Task Update(IEnumerable<SubjectLocal> subjects)
    {
        await using SubjectsContext context = new(); 
        
        foreach (SubjectLocal subj in subjects)
        {
            context.Subjects.Attach(subj);
            context.Entry(subj).State = EntityState.Modified;

            foreach (ControlElementLocal elem in subj.Elements)
            {
                context.Elements.Attach(elem);
                context.Entry(elem).State = EntityState.Modified;
            }
        }
        
        await context.SaveChangesAsync();
    }
    
    
    public async Task UpdateRemoteKeys(List<SubjectId> subjIds)
    {
        await using SubjectsContext context = new();

        foreach (SubjectId subjId in subjIds)
        {
            SubjectLocal? subj = await context.Subjects.FindAsync(subjId.LocalId);

            if (subj == null)
            {
                MessageBox.Show($"Lost subject with LocalId={subjId.LocalId}");
                continue;
            }

            subj.RemoteId = subjId.RemoteId;
            UpdateElemsRemoteKeys(context, subj, subjId);
        }
        await context.SaveChangesAsync();
    }

    
    private void UpdateElemsRemoteKeys(SubjectsContext context, SubjectLocal subj, SubjectId subjId)
    {
        ControlElementLocal[] elems = context.Elements.Where(el => el.SubjectId == subj.SubjectId)
            .OrderBy(el => el.ElementId).ToArray();

        ElementId[] elemIds = subjId.Elements.OrderBy(el => el.LocalId).ToArray();

        // Updating keys if no data was lost
        if (elems.Length == elemIds.Length)
        {
            for (int i = 0; i < elems.Length; ++i)
            {
                elems[i].RemoteId = elemIds[i].RemoteId;
            }
            return;
        }
        
        // In case some data was lost 
        int r = 0;
        int l = 0;

        while (r < elems.Length && l < elemIds.Length)
        {
            if (elems[r].ElementId == elemIds[l].LocalId)
            {
                elems[r++].RemoteId = elemIds[l++].RemoteId;
                continue;
            }

            if (elems[r].ElementId > elemIds[l].LocalId)
            {
                ++l;
            }
            else
            {
                ++r;
            }
        }
    }
}