﻿using HealthCareCenter.Model;
using HealthCareCenter.PatientGUI.Models;
using HealthCareCenter.PatientGUI.Stores;
using HealthCareCenter.PatientGUI.ViewModels;
using HealthCareCenter.Service;
using System;
using System.Collections.Generic;
using System.Windows;

namespace HealthCareCenter.PatientGUI.Commands
{
    internal class PriorityScheduleAppointmentCommand : CommandBase
    {
        private void SetupPriorityNotFound(PatientFunctionality patFunc)
        {
            _ = MessageBox.Show("Couldn't find appointments by priority, showing 3 closest to priority",
                    "My App", MessageBoxButton.OK, MessageBoxImage.Information);

            List<Appointment> similarToPriority = patFunc.GetAppointmentsSimilarToPriorites(
                _viewModel.IsDoctorPriority, _viewModel.ChosenDoctor.DoctorID, _viewModel.Patient.HealthRecordID, 
                _viewModel.ChosenDate, _viewModel.StartRange, _viewModel.EndRange);

            if (similarToPriority.Count == 0)
            {
                _ = MessageBox.Show("Couldn't find appointments similar to priority", "Configuration", MessageBoxButton.OK, MessageBoxImage.Warning);
                _navigationStore.CurrentViewModel = new MyAppointmentsViewModel(_navigationStore, _viewModel.Patient);
                return;
            }

            List<PriorityNotFoundChoiceViewModel> alternativeChoices = new List<PriorityNotFoundChoiceViewModel>();
            foreach (Appointment appointment in similarToPriority)
            {
                alternativeChoices.Add(new PriorityNotFoundChoiceViewModel(appointment));
            }

            _viewModel.PriorityNotFoundChoices = alternativeChoices;
        }

        private void PriorityFound()
        {
            PatientFunctionality patFunc = PatientFunctionality.GetInstance();

            if (_viewModel.ChosenDoctor == null)
            {
                _ = MessageBox.Show("Doctor not chosen", "Configuration", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_viewModel.ChosenDate.Date.CompareTo(DateTime.Now.Date) <= 0)
            {
                _ = MessageBox.Show("Invalid date", "Configuration", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!(_viewModel.StartRange < _viewModel.EndRange))
            {
                _ = MessageBox.Show("Invalid range", "Configuration", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Appointment newAppointment = patFunc.GetPriorityAppointment(
                _viewModel.IsDoctorPriority, Convert.ToInt32(_viewModel.ChosenDoctor.DoctorID), _viewModel.Patient.HealthRecordID,
                _viewModel.ChosenDate, _viewModel.StartRange, _viewModel.EndRange);

            if (newAppointment == null)
            {
                SetupPriorityNotFound(patFunc);
                return;
            }

            string appointmentDetails = "Doctor: " + newAppointment.DoctorID + ", Schedule: " + newAppointment.ScheduledDate.ToString("g");
            _ = MessageBox.Show("Appointment found: " + appointmentDetails);

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Schedule appointment?", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                patFunc.ScheduleAppointment(newAppointment);
                _navigationStore.CurrentViewModel = new MyAppointmentsViewModel(_navigationStore, _viewModel.Patient);
            }
        }

        private void PriorityNotFound()
        {
            PatientFunctionality patFunc = PatientFunctionality.GetInstance();

            if (_viewModel.PriorityNotFoundChoice == null)
            {
                _ = MessageBox.Show("Appointment not chosen", "Configuration", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int doctorID = _viewModel.PriorityNotFoundChoice.DoctorID;
            DateTime scheduleDate = _viewModel.PriorityNotFoundChoice.AppointmentDate;
            int hospitalRoomID = HospitalRoomService.GetAvailableRoomID(scheduleDate, Enums.RoomType.Checkup);
            if (hospitalRoomID == -1)
            {
                MessageBox.Show("No available rooms", "Configuration", MessageBoxButton.OK, MessageBoxImage.Warning);
                _navigationStore.CurrentViewModel = new MyAppointmentsViewModel(_navigationStore, _viewModel.Patient);
                return;
            }

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Schedule appointment?", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                patFunc.ScheduleAppointment(
                    scheduleDate, doctorID, _viewModel.Patient.HealthRecordID, hospitalRoomID);
                _navigationStore.CurrentViewModel = new MyAppointmentsViewModel(_navigationStore, _viewModel.Patient);
            }
        }

        public override void Execute(object parameter)
        {
            if (_viewModel.PriorityNotFoundChoices.Count == 0)
            {
                PriorityFound();
            }
            else
            {
                PriorityNotFound();
            }

        }

        private readonly PrioritySchedulingViewModel _viewModel;
        private readonly NavigationStore _navigationStore;

        public PriorityScheduleAppointmentCommand(PrioritySchedulingViewModel viewModel, NavigationStore navigationStore)
        {
            _viewModel = viewModel;
            _navigationStore = navigationStore;
        }
    }
}
