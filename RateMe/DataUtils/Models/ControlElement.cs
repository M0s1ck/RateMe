using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateMe.DataUtils.Models
{
    public class ControlElement
    {
        public string Name 
        {
            get => _name;
            set
            {
                _name = value;
            }
        }


        public double Weight
        {
            get => _weight;
            set
            {
                _weight = value;
            }
        }

        public double Grade
        {
            get => _grade;
            set
            {
                _grade = value;
            }
        }

        private string _name;
        private double _weight;
        private double _grade;


        public ControlElement(string name, double weight) 
        {
            _name = name;
            _weight = weight;
            _grade = 0;
        }


    }
}
