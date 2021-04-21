﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Enterprice_incidents.Ef;
using static Enterprice_incidents.Ef.DataClass;
using Enterprice_incidents.Windows;

namespace Enterprice_incidents
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            IncidentListView.ItemsSource = Context.View_Incidents.ToList();

            // Создание листов с содержимым ComboBox'ов

            List<Incident_Type> incidentTypeSort = Context.Incident_Type.ToList();
            incidentTypeSort.Insert(0, new Incident_Type() { ImportanceOfIncident = "All" });
            IncidentType_Cmb.ItemsSource = incidentTypeSort;
            IncidentType_Cmb.DisplayMemberPath = "ImportanceOfIncident";
            IncidentType_Cmb.SelectedIndex = 0;

            List<string> dateSort = new List<string>() { "---", "Возрастанию", "Убыванию" };

            IncidentType_Cmb.ItemsSource = incidentTypeSort;
            IncidentType_Cmb.SelectedIndex = 0;

            DateTime_Cmb.ItemsSource = dateSort;
            DateTime_Cmb.SelectedIndex = 0;

        }

        public void Filter()
        {
            var comboboxFilter = Context.View_Incidents.ToList();

            switch (IncidentType_Cmb.SelectedIndex)
            {
                case 0:

                    break;

                case 1:
                    comboboxFilter = comboboxFilter.Where(i => i.ImportanceOfIncident == "Trivial").ToList();
                    break;

                case 2:
                    comboboxFilter = comboboxFilter.Where(i => i.ImportanceOfIncident == "Important").ToList();
                    break;

                case 3:
                    comboboxFilter = comboboxFilter.Where(i => i.ImportanceOfIncident == "Critical").ToList();
                    break;

                case 4:
                    comboboxFilter = comboboxFilter.Where(i => i.ImportanceOfIncident == "Special Importance").ToList();
                    break;
            }

            switch (DateTime_Cmb.SelectedIndex)
            {
                case 0:

                    break;

                case 1:
                    comboboxFilter = comboboxFilter.OrderBy(i => i.DateOfIncident).ToList();
                    break;

                case 2:
                    comboboxFilter = comboboxFilter.OrderByDescending(i => i.DateOfIncident).ToList();
                    break;
            }

            IncidentListView.ItemsSource = comboboxFilter;
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            CreationOfNewIncident creationOfNewIncident = new CreationOfNewIncident();
            creationOfNewIncident.ShowDialog();
            IncidentListView.ItemsSource = Context.View_Incidents.ToList();
            this.Show();
        }

        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            if (IncidentListView.SelectedItem is null)
            {
                MessageBox.Show("Выберите запись для удаления", "Инцидент не выбран", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            else if (IncidentListView.SelectedItem is View_Incidents view_Incidents)
            {
                var result = MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Context.Incidents_History.Remove(Context.Incidents_History.
                        Where(i => i.Id == view_Incidents.Id).FirstOrDefault());
                    Context.SaveChanges();

                    MessageBox.Show("Запись удалена.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                    IncidentListView.ItemsSource = Context.View_Incidents.ToList();
                }

                else if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
        }

        private void IncidentType_Cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void DateTime_Cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Context.ChangeTracker.Entries().ToList().ForEach(i => i.Reload());
                IncidentListView.ItemsSource = Context.View_Incidents.ToList();
            }
        }
    }
}
