using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RateMe.DataUtils.Models
{
    public class Formula : ObservableCollection<ControlElement>
    {
        private static Regex regex = new(@"((0[.,]\d+)\s*[*∙]\s*(.+?))\s*([\+\n]|(\. )|$)");
        

        public Formula(string sFormula) : base()
        {
            MatchCollection matches = regex.Matches(sFormula);

            if (matches.Count == 0)
            {
                throw new ArgumentOutOfRangeException("Couldn't handle the formula from pud");
            }

            foreach (Match match in matches)
            {
                string name = match.Groups[3].Value;
                double.TryParse(match.Groups[2].Value, NumberStyles.Any, CultureInfo.CurrentCulture, out double weight);

                ControlElement elem = new ControlElement(name, weight);
                double w = elem.Weight;
                Add(elem);
            }
        }
    }
}
