﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HealthCareCenter.Core.Equipment.Repositories;
using HealthCareCenter.Core.Equipment.Services;
using HealthCareCenter.Core.Medicine.Services;
using HealthCareCenter.Core.HealthRecords;
using HealthCareCenter.Core.Medicine.Repositories;
using HealthCareCenter.Core.Notifications.Repositories;
using HealthCareCenter.Core.Notifications.Services;
using HealthCareCenter.Core.Rooms;
using HealthCareCenter.Core.Rooms.Repositories;
using HealthCareCenter.Core.Rooms.Services;
using HealthCareCenter.Core.Surveys.Controllers;
using HealthCareCenter.Core.Surveys.Models;
using HealthCareCenter.Core.Surveys.Services;
using HealthCareCenter.Core.Users.Models;
using HealthCareCenter.Core.Surveys.Repositories;
using HealthCareCenter.Core.Users.Services;
using HealthCareCenter.Core.Users;
using HealthCareCenter.Core.Appointments.Repository;

namespace HealthCareCenter
{
    /// <summary>
    /// Interaction logic for DoctorSurveysOverviewWindow.xaml
    /// </summary>
    public partial class DoctorSurveysOverviewWindow : Window
    {
        private Manager _signedManager;
        private string[] _doctorSurveysHeader = { "DoctorID", "PatientID", "Comment", "Rating" };
        private string[] _best3DoctorsHeader = { "DoctorID", "First Name", "Second Name", "Rating" };
        private string[] _worst3DoctorsHeader = { "DoctorID", "First Name", "Second Name", "Rating" };
        private DoctorSurveyOverviewController _controller;

        private readonly IRoomService _roomService;
        private readonly IHospitalRoomUnderConstructionService _hospitalRoomUnderConstructionService;

        private readonly IHospitalRoomForRenovationService _hospitalRoomForRenovationService;
        private readonly IRenovationScheduleService _renovationScheduleService;

        private readonly IEquipmentRearrangementService _equipmentRearrangementService;
        private readonly IDoctorSurveyRatingService _doctorSurveyRatingService;

        private readonly IMedicineCreationRequestService _medicineCreationRequestService;

        public DoctorSurveysOverviewWindow(Manager manager, IRoomService roomService, IHospitalRoomUnderConstructionService hospitalRoomUnderConstructionService, IHospitalRoomForRenovationService hospitalRoomForRenovationService, IRenovationScheduleService renovationScheduleService, IEquipmentRearrangementService equipmentRearrangementService, IDoctorSurveyRatingService doctorSurveyRatingService, IMedicineCreationRequestService medicineCreationRequestService)
        {
            _roomService = roomService;
            _hospitalRoomUnderConstructionService = hospitalRoomUnderConstructionService;
            _hospitalRoomForRenovationService = hospitalRoomForRenovationService;
            _renovationScheduleService = renovationScheduleService;
            _equipmentRearrangementService = equipmentRearrangementService;
            _doctorSurveyRatingService = doctorSurveyRatingService;
            _medicineCreationRequestService = medicineCreationRequestService;

            _signedManager = manager;
            _controller = new DoctorSurveyOverviewController(
                doctorSurveyRatingService, 
                new DoctorSurveyRatingRepository(),
                new DoctorService(
                    new DoctorSearchService(
                        new UserRepository()),
                    new UserRepository()));
            InitializeComponent();
            FillDataGridDoctorsSurveys();
            FillDataGridBest3Dctors();
            FillDataGridWorst3Doctors();
        }

        private void AddDataGridHeader(DataGrid dataGrid, string[] header)
        {
            foreach (string label in header)
            {
                DataGridTextColumn column = new DataGridTextColumn();
                column.Header = label;
                column.Binding = new Binding(label.Replace(' ', '_'));
                dataGrid.Columns.Add(column);
            }
        }

        private void AddDataGridRow(DataGrid dataGrid, string[] header, List<string> equipmentAttributesToDisplay)
        {
            dynamic row = new ExpandoObject();

            for (int i = 0; i < header.Length; i++)
            {
                ((IDictionary<String, Object>)row)[header[i].Replace(' ', '_')] = equipmentAttributesToDisplay[i];
            }

            dataGrid.Items.Add(row);
        }

        private void FillDataGridDoctorsSurveys()
        {
            AddDataGridHeader(DataGridSurveys, _doctorSurveysHeader);
            List<DoctorSurveyRating> doctorSurveys = _controller.GetDoctorSurveys();
            foreach (DoctorSurveyRating survey in doctorSurveys)
            {
                AddDataGridRow(DataGridSurveys, _doctorSurveysHeader, survey.ToList());
            }
        }

        private void FillDataGridBest3Dctors()
        {
            AddDataGridHeader(DataGridBest3Doctors, _best3DoctorsHeader);

            foreach (List<string> doctor in _controller.GetBest3Doctors())
            {
                AddDataGridRow(DataGridBest3Doctors, _best3DoctorsHeader, doctor);
            }
        }

        private void FillDataGridWorst3Doctors()
        {
            AddDataGridHeader(DataGridWorst3Doctors, _worst3DoctorsHeader);
            foreach (List<string> doctor in _controller.GetWorst3Doctors())
            {
                AddDataGridRow(DataGridWorst3Doctors, _worst3DoctorsHeader, doctor);
            }
        }

        private void ShowWindow(Window window)
        {
            window.Show();
            Close();
        }

        private void CrudHospitalRoomMenuItemClick(object sender, RoutedEventArgs e)
        {
            ShowWindow(new CrudHospitalRoomWindow(
                _signedManager,
                new NotificationService(
                    new NotificationRepository(),
                    new HealthRecordService(
                        new HealthRecordRepository()),
                    new MedicineInstructionService(
                            new MedicineInstructionRepository()),
                        new MedicineService(
                            new MedicineRepository()),
                    new HospitalRoomService(
                        new AppointmentRepository(),
                        new HospitalRoomForRenovationService(
                            new HospitalRoomForRenovationRepository()),
                        new HospitalRoomRepository())),
                new RoomService(
                    new StorageRepository(),
                    new EquipmentService(
                        new EquipmentRepository()),
                    new HospitalRoomUnderConstructionService(
                        new HospitalRoomUnderConstructionRepository()),
                    new HospitalRoomForRenovationService(
                        new HospitalRoomForRenovationRepository()),
                    new HospitalRoomService(
                        new AppointmentRepository(),
                        new HospitalRoomForRenovationService(
                            new HospitalRoomForRenovationRepository()),
                        new HospitalRoomRepository())),
                new HospitalRoomUnderConstructionService(
                    new HospitalRoomUnderConstructionRepository()),
                new HospitalRoomForRenovationService(
                    new HospitalRoomForRenovationRepository()),
                new RenovationScheduleService(
                    new RoomService(
                        new StorageRepository(),
                        new EquipmentService(
                            new EquipmentRepository()),
                        new HospitalRoomUnderConstructionService(
                            new HospitalRoomUnderConstructionRepository()),
                        new HospitalRoomForRenovationService(
                            new HospitalRoomForRenovationRepository()),
                        new HospitalRoomService(
                            new AppointmentRepository(),
                            new HospitalRoomForRenovationService(
                                new HospitalRoomForRenovationRepository()),
                            new HospitalRoomRepository())),
                    new HospitalRoomUnderConstructionService(
                        new HospitalRoomUnderConstructionRepository()),
                    new HospitalRoomForRenovationService(
                            new HospitalRoomForRenovationRepository()),
                    new RenovationScheduleRepository(),
                    new HospitalRoomService(
                        new AppointmentRepository(),
                        new HospitalRoomForRenovationService(
                            new HospitalRoomForRenovationRepository()),
                        new HospitalRoomRepository())),
                new EquipmentRearrangementService(
                    new RoomService(
                        new StorageRepository(),
                        new EquipmentService(
                            new EquipmentRepository()),
                        new HospitalRoomUnderConstructionService(
                            new HospitalRoomUnderConstructionRepository()),
                        new HospitalRoomForRenovationService(
                            new HospitalRoomForRenovationRepository()),
                        new HospitalRoomService(
                            new AppointmentRepository(),
                            new HospitalRoomForRenovationService(
                                new HospitalRoomForRenovationRepository()),
                            new HospitalRoomRepository())),
                    new EquipmentService(
                        new EquipmentRepository()),
                    new HospitalRoomUnderConstructionService(
                        new HospitalRoomUnderConstructionRepository())),
                new DoctorSurveyRatingService(
                    new DoctorSurveyRatingRepository(),
                    new UserRepository()),
                new MedicineCreationRequestService(
                    new MedicineCreationRequestRepository())));
        }

        private void EquipmentReviewMenuItemClick(object sender, RoutedEventArgs e)
        {
            ShowWindow(new HospitalEquipmentReviewWindow(_signedManager, _roomService, _hospitalRoomUnderConstructionService, _hospitalRoomForRenovationService, _renovationScheduleService, _equipmentRearrangementService, _doctorSurveyRatingService, _medicineCreationRequestService));
        }

        private void ArrangingEquipmentItemClick(object sender, RoutedEventArgs e)
        {
            ShowWindow(new ArrangingEquipmentWindow(_signedManager, _roomService, _hospitalRoomUnderConstructionService, _hospitalRoomForRenovationService, _renovationScheduleService, _equipmentRearrangementService, _doctorSurveyRatingService, _medicineCreationRequestService));
        }

        private void SimpleRenovationItemClick(object sender, RoutedEventArgs e)
        {
            ShowWindow(new HospitalRoomRenovationWindow(_signedManager, _roomService, _hospitalRoomUnderConstructionService, _hospitalRoomForRenovationService, _renovationScheduleService, _equipmentRearrangementService, _doctorSurveyRatingService, _medicineCreationRequestService));
        }

        private void ComplexRenovationMergeItemClick(object sender, RoutedEventArgs e)
        {
            ShowWindow(new ComplexHospitalRoomRenovationMergeWindow(_signedManager, _roomService, _hospitalRoomUnderConstructionService, _hospitalRoomForRenovationService, _renovationScheduleService, _equipmentRearrangementService, _doctorSurveyRatingService, _medicineCreationRequestService));
        }

        private void ComplexRenovationSplitItemClick(object sender, RoutedEventArgs e)
        {
            ShowWindow(new ComplexHospitalRoomRenovationSplitWindow(_signedManager, _roomService, _hospitalRoomUnderConstructionService, _hospitalRoomForRenovationService, _renovationScheduleService, _equipmentRearrangementService, _doctorSurveyRatingService, _medicineCreationRequestService));
        }

        private void CreateMedicineClick(object sender, RoutedEventArgs e)
        {
            ShowWindow(new MedicineCreationRequestWindow(_signedManager, _roomService, _hospitalRoomUnderConstructionService, _hospitalRoomForRenovationService, _renovationScheduleService, _equipmentRearrangementService, _doctorSurveyRatingService, _medicineCreationRequestService));
        }

        private void ReffusedMedicineClick(object sender, RoutedEventArgs e)
        {
            ShowWindow(new ChangedMedicineCreationRequestWindow(_signedManager, _roomService, _hospitalRoomUnderConstructionService, _hospitalRoomForRenovationService, _renovationScheduleService, _equipmentRearrangementService, _doctorSurveyRatingService, _medicineCreationRequestService));
        }

        private void HealthcareSurveysClick(object sender, RoutedEventArgs e)
        {
            ShowWindow(new HealthcareSurveysOverviewWindow(_signedManager, _roomService, _hospitalRoomUnderConstructionService, _hospitalRoomForRenovationService, _renovationScheduleService, _equipmentRearrangementService, _doctorSurveyRatingService, _medicineCreationRequestService));
        }

        private void DoctorSurveysClick(object sender, RoutedEventArgs e)
        {
            ShowWindow(new DoctorSurveysOverviewWindow(_signedManager, _roomService, _hospitalRoomUnderConstructionService, _hospitalRoomForRenovationService, _renovationScheduleService, _equipmentRearrangementService, _doctorSurveyRatingService, _medicineCreationRequestService));
        }

        private void LogOffItemClick(object sender, RoutedEventArgs e)
        {
            ShowWindow(new LoginWindow());
        }
    }
}