using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageVerification.Model
{
   public class FeaturePoint : INotifyPropertyChanged
    {

        private string _pointName;
        private Point _pointCoordinates;

        public string PointName
        {
            get { return _pointName; }
            set
            {
                _pointName = value;
                RaisePropertyChanged();
            }
        }

        public Point PointCoordinates
        {
            get
            {
                return _pointCoordinates;
            }

            set
            {
                _pointCoordinates = value;
                RaisePropertyChanged();
            }
        }

        // Obsługa zmiany wartości
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(
            [CallerMemberName] string caller = " ")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }

        }


    }
}
