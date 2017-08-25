using ImageVerification.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageVerification
{
    /// <summary>
    /// Interaction logic for TablesInfo.xaml
    /// </summary>
    public partial class TablesInfo : Window
    {

        PointsTableRepository pointsData;

        public TablesInfo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handling loading data in window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pointsData = new PointsTableRepository();

            dgridPoints.DataContext = pointsData.GetFrontPointsDataTable();
            dgridProfilePoints.DataContext = pointsData.GetProfilePointsDataTable();
           

            foreach (DataGridColumn column in dgridPoints.Columns)
                {                        
               // if you want to size ur column as per both header and cell content
                column.Width = 60;               
                }      
            
            }
           
    

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            this.Close();
        }

        private void dgridProfilePoints_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if(e.PropertyName=="WspSkalowania")
            {
                e.Column.Width = 115;
            }
            if (e.PropertyName =="WspSkalowaniaMax")
            {
                e.Column.Width = 115;
            }
            if (e.PropertyName == "Ucho_Nos")
            {
                e.Column.Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto);
            }
           
        }

        private void dgridPoints_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if(e.PropertyName == "WspSkalowania")
            {
                e.Column.Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto);
                
            }
            if (e.PropertyName == "WspSkalowaniaMax")
            {
                e.Column.Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto);

            }
        }
    }
}
