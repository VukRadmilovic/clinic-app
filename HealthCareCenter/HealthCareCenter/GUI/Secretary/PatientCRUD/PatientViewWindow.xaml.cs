﻿using System;
using System.Windows;
using HealthCareCenter.Core;
using HealthCareCenter.Core.Appointments.Repository;
using HealthCareCenter.Core.HealthRecords;
using HealthCareCenter.Core.Patients;
using HealthCareCenter.Core.Patients.Controllers;
using HealthCareCenter.Core.Referrals.Repositories;
using HealthCareCenter.Core.Referrals.Services;

namespace HealthCareCenter.Secretary
{
    /// <summary>
    /// Interaction logic for PatientViewWindow.xaml
    /// </summary>
    public partial class PatientViewWindow : Window
    {
        private readonly Patient _patient;
        private readonly HealthRecord _record;

        private readonly PatientViewController _controller;

        public PatientViewWindow()
        {
            InitializeComponent();
        }

        public PatientViewWindow(Patient patient, HealthRecord record)
        {
            _patient = patient;
            _record = record;
            InitializeComponent();

            _controller = new PatientViewController();
        }

        private void InitializeWindow()
        {
            idTextBox.Text = _patient.ID.ToString();
            firstNameTextBox.Text = _patient.FirstName;
            usernameTextBox.Text = _patient.Username;
            lastNameTextBox.Text = _patient.LastName;
            passwordTextBox.Text = _patient.Password;
            birthDatePicker.SelectedDate = _patient.DateOfBirth;
            isBlockedCheckBox.IsChecked = _patient.IsBlocked;
            blockedByTextBox.Text = _patient.BlockedBy.ToString();
            heightTextBox.Text = _record.Height.ToString();
            weightTextBox.Text = _record.Weight.ToString();
            previousDiseasesListBox.ItemsSource = _record.PreviousDiseases;
            allergensListBox.ItemsSource = _record.Allergens;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            InitializeWindow();
        }

        private void BlockButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _controller.Block(_patient);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            isBlockedCheckBox.IsChecked = true;
            blockedByTextBox.Text = Blocker.Secretary.ToString();
            MessageBox.Show("Patient successfully blocked.");
        }

        private void UnblockButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _controller.Unblock(_patient);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            isBlockedCheckBox.IsChecked = false;
            blockedByTextBox.Text = Blocker.None.ToString();
            MessageBox.Show("Patient successfully unblocked.");
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            PatientEditWindow window = new PatientEditWindow(_patient, _record);
            window.ShowDialog();
            InitializeWindow();
            previousDiseasesListBox.Items.Refresh();
            allergensListBox.Items.Refresh();
        }

        private void ViewChangeRequestsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewChangeRequestsWindow window = new ViewChangeRequestsWindow(_patient);
            window.ShowDialog();
        }

        private void ViewReferralsButton_Click(object sender, RoutedEventArgs e)
        {
            PatientReferralsWindow window = new PatientReferralsWindow(
                _patient, 
                new ReferralsService(
                    new ReferralRepository(),
                    new AppointmentRepository()));
            window.ShowDialog();
        }

        private void ScheduleUrgentAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            ScheduleUrgentAppointmentWindow window = new ScheduleUrgentAppointmentWindow(_patient);
            window.ShowDialog();
        }
    }
}
