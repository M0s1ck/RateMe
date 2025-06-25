using System.Windows;
using RateMe.Models.LocalDbModels;
using RateMeShared.Dto;

namespace RateMe.Repositories;

public class SubjectsRepository
{
    internal async Task<Dictionary<int, SubjectLocal>> GetSubjectsNoRemote()
    {
        await using SubjectsContext context = new();
        Dictionary<int, SubjectLocal> subs = [];

        foreach (SubjectLocal sub in context.Subjects)
        {
            if (sub.RemoteId == 0)
            {
                subs[sub.SubjectId] = sub;
            }
        }

        return subs;
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

        ControlElementId[] elemIds = subjId.Elements.OrderBy(el => el.LocalId).ToArray();

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