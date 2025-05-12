using System.Windows;
using RateMe.Models.LocalDbModels;
using RateMeShared.Dto;

namespace RateMe.Repositories;

public class SubjectsRepository
{
    public async Task UpdateRemoteKeys(List<SubjectId> subjIds)
    {
        SubjectsContext context = new();
        
        foreach (SubjectId subjId in subjIds)
        {
            SubjectLocal? subj = await context.Subjects.FindAsync(subjId.LocalId);

            if (subj == null)
            {
                MessageBox.Show($"Lost subject with LocalId={subjId.LocalId}");
                continue;
            }

            subj.RemoteId = subjId.RemoteId;

            ControlElementLocal[] elems = context.Elements.Where(el => el.SubjectId == subj.SubjectId).
                                        OrderBy(el => el.ElementId).ToArray();

            ControlElementId[] elemIds = subjId.Elements.OrderBy(el => el.LocalId).ToArray();
            
            // Updating keys if no data was lost
            if (elems.Length == elemIds.Length)
            {
                for (int i = 0; i < elems.Length; ++i)
                {
                    elems[i].RemoteId = elemIds[i].RemoteId;
                }
            }
            else if (elems.Length < elemIds.Length) // In case some data was lost 
            {
                int cnt = 0;

                foreach (ControlElementId elemId in elemIds)
                {
                    if (elems[cnt].ElementId == elemId.LocalId)
                    {
                        elems[cnt].RemoteId = elemId.RemoteId;
                        ++cnt;
                    }

                    if (cnt == elems.Length)
                    {
                        break;
                    }
                }
            }
            else
            {
                int cnt = 0;
            
                foreach (ControlElementLocal elem in elems)
                {
                    if (elem.ElementId == elemIds[cnt].LocalId)
                    {
                        elem.RemoteId = elemIds[cnt].RemoteId;
                        ++cnt;
                    }

                    if (cnt == elemIds.Length)
                    {
                        break;
                    }
                }
            }

            await context.SaveChangesAsync();
        }
    }
}