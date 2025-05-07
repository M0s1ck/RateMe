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
        private static Regex regex = new(@"((0[.,]\d+)\s*[*∗∙]\s*(.+?))\s*([\+\n]|(\. )|$)");
        
        public Formula() : base() { }

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
                string w = match.Groups[2].Value;
                w = w.Replace(',', '.');
                decimal.TryParse(w, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal weight);

                ControlElement elem = new ControlElement(name, weight);
                Add(elem);
            }

            this[0].ViewBorderRadius = new System.Windows.CornerRadius(3, 0, 0, 3);
            this[0].ViewBorderThickness = new System.Windows.Thickness(2, 2, 1, 2);
            this[^1].ViewBorderRadius = new System.Windows.CornerRadius(0, 3, 3, 0);
            this[^1].ViewBorderThickness = new System.Windows.Thickness(1, 2, 2, 2);
        }
    }
}
