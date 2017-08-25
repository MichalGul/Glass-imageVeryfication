using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageVerification.Model
{
    /// <summary>
    /// Customer data model for managing data in the database
    /// </summary>
    public class Customer : INotifyPropertyChanged
    {
        private int _customerId;
        private string _customerName;
        private string _customerSurname;
        private string _customerEmail;
        private double _pupilDistance;
        private double _faceWidth;
        private double _templeWidth;
        private double _rightEyeNoseDistance;
        private double _leftEyeNoseDistance;
        private double _profileNoseEarDistance;
        private double _profileNoseEyeDistance;
        private byte[] _frontImage;
        private byte[] _profileImage;
        private bool _approved;

        public int CustomerId
        {
            get
            {
                return _customerId;
            }

            set
            {
                _customerId = value;
                RaisePropertyChanged();
            }
        }

        public string CustomerName
        {
            get
            {
                return _customerName;
            }

            set
            {
                _customerName = value;
                RaisePropertyChanged();
            }
        }

        public string CustomerSurname
        {
            get
            {
                return _customerSurname;
            }

            set
            {
                _customerSurname = value;
                RaisePropertyChanged();
            }
        }

        public string CustomerEmail
        {
            get
            {
                return _customerEmail;
            }

            set
            {
                _customerEmail = value;
                RaisePropertyChanged();
            }
        }

        public double PupilDistance
        {
            get
            {
                return _pupilDistance;
            }

            set
            {
                _pupilDistance = value;
                RaisePropertyChanged();
            }
        }

        public double FaceWidth
        {
            get
            {
                return _faceWidth;
            }

            set
            {
                _faceWidth = value;
                RaisePropertyChanged();
            }
        }

        public double TempleWidth
        {
            get
            {
                return _templeWidth;
            }

            set
            {
                _templeWidth = value;
                RaisePropertyChanged();
            }
        }

        public double RightEyeNoseDistance
        {
            get
            {
                return _rightEyeNoseDistance;
            }

            set
            {
                _rightEyeNoseDistance = value;
                RaisePropertyChanged();
            }
        }

        public double LeftEyeNoseDistance
        {
            get
            {
                return _leftEyeNoseDistance;
            }

            set
            {
                _leftEyeNoseDistance = value;
                RaisePropertyChanged();
            }
        }

        public double ProfileNoseEarDistance
        {
            get
            {
                return _profileNoseEarDistance;
            }

            set
            {
                _profileNoseEarDistance = value;
                RaisePropertyChanged();
            }
        }

        public byte[] FrontImage
        {
            get
            {
                return _frontImage;
            }

            set
            {
                _frontImage = value;
                RaisePropertyChanged();
            }
        }

        public byte[] ProfileImage
        {
            get
            {
                return _profileImage;
            }

            set
            {
                if (value == null)
                {
                    byte[] emptyArray = null;
                    _profileImage = emptyArray;
                }
                else
                {
                    _profileImage = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Approved
        {
            get
            {
                return _approved;
            }

            set
            {
                _approved = value;
                RaisePropertyChanged();
            }
        }

        public double ProfileNoseEyeDistance
        {
            get
            {
                return _profileNoseEyeDistance;
            }

            set
            {
                _profileNoseEyeDistance = value;
                RaisePropertyChanged();
            }
        }



       /// <summary>
       /// Implementing INotify interface
       /// </summary>
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
